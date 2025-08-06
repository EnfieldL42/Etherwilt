using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class CharacterCombatManager : NetworkBehaviour
{
    protected CharacterManager character;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;

    [Header("Previous Poise Damage Take")]
    public float previousPoiseDamageTaken;

    [Header("Attack Target")]
    public CharacterManager currentTarget;
    public NetworkVariable<bool> hasChangedTarget = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    [Header("Attack Flags")]
    public bool canPerformRollingAttack = false;
    public bool canPerformBackstepAttack = false;
    public bool canBlock = true;

    [Header("Critical Attack")]
    private Transform riposteReceiverTransform;
    [SerializeField] float criticalAttackDistanceCheck = 0.7f;
    public int pendingCriticalDamage;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {

            if(newTarget != null)
            {
                currentTarget = newTarget;
                character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                hasChangedTarget.Value = true;
            }
            else
            {
                currentTarget = null;
            }
        
    }

    public virtual void AttemptCriticalAttack()
    {
        if(character.isPerformingAction)
        {
            return;
        }    

        //cant crit strike if out of stamina
        if(character.characterNetworkManager.currentStamina.Value <= 0)
        {
            return; 
        }

        //aims a raycast infront of player and check for potential targets to crit strike
        RaycastHit[] hits = Physics.RaycastAll(character.characterCombatManager.lockOnTransform.position, 
            character.transform.TransformDirection(Vector3.forward), criticalAttackDistanceCheck, WorldUtilityManager.instance.GetCharacterLayers());

        for(int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            CharacterManager targetCharacter = hit.transform.GetComponent<CharacterManager>();

            if(targetCharacter != null)
            {
                //checks if its the player the the one getting crit strike, move to next in array so it doesnt hit himself
                if(targetCharacter == character)
                {
                    continue;
                }

                //checks if its the same team or not
                if(!WorldUtilityManager.instance.CanIDamageThisTarget(character.characterGroup, targetCharacter.characterGroup))
                {
                    continue;
                }

                Vector3 directionFromCharacterToTarget = character.transform.position - targetCharacter.transform.position;
                float targetViewableAngle = Vector3.SignedAngle(directionFromCharacterToTarget, targetCharacter.transform.forward, Vector3.up);

                if(targetCharacter.characterNetworkManager.isRipostable.Value)
                {
                    if(targetViewableAngle >= -80 && targetViewableAngle <= 80)
                    {
                        AttemptRiposte(hit);
                        return;
                    }
                }
            }
        }

    }

    public virtual void AttemptRiposte(RaycastHit hit)
    {

    }

    public virtual void ApplyCriticalDamage()
    {

        character.characterEffectsManager.PlayCriticalBloodSplatterVFX(character.characterCombatManager.lockOnTransform.position);
        character.characterSoundFXManager.PlayCriticalStrikeSoundFX();

        if(character.IsOwner)
        {
  
            character.characterNetworkManager.currentHealth.Value -= pendingCriticalDamage;

        }

    }

    public IEnumerator ForceMoveEnemyCharacterToRipostePosition(CharacterManager enemyCharacter, Vector3 ripostePosition)
    {
        float timer = 0;

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;

            if(riposteReceiverTransform == null)
            {
                GameObject riposteTransformObject = new GameObject("Riposte Transform");
                riposteTransformObject.transform.parent = transform;
                riposteTransformObject.transform.position = Vector3.zero;
                riposteReceiverTransform = riposteTransformObject.transform;
            }

            riposteReceiverTransform.localPosition = ripostePosition;
            enemyCharacter.transform.position = riposteReceiverTransform.position;
            transform.rotation = Quaternion.LookRotation(-enemyCharacter.transform.forward);
            yield return null;
        }
    }

    public void EnableIsInvulnerable()
    {
        if(character.IsOwner)
        {
            character.characterNetworkManager.isInvulnerable.Value = true;
        }
    }

    public void DisableIsInvulnerable()
    {
        if(character.IsOwner)
        {
            character.characterNetworkManager.isInvulnerable.Value = false;
        }

    }

    public void EnableIsRipostable()
    {
        if(character.IsOwner)
        {
            character.characterNetworkManager.isRipostable.Value = true;
        }
    }

    public void EnableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;

    }

    public void DisableCanDoRollingAttack()
    {
        canPerformRollingAttack = false;

    }

    public void EnableCanDoBackstepAttack()
    {
        canPerformBackstepAttack = true;

    }

    public void DisableCanDoBackstepAttack()
    {
        canPerformBackstepAttack = false;
    }

    public virtual void EnableCanDoCombo()
    {

    }

    public virtual void DisableCanDoCombo()
    {

    }

}
