using UnityEngine;
using Unity.Netcode;

public class CharacterCombatManager : NetworkBehaviour
{
    protected CharacterManager character;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;

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
