using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.Text;

public class AIEarthGuardianBodyCombatManager : AICharacterCombatManager
{
    [HideInInspector] public AIEarthGuardianCharacterManager earthGuardianManager;

    [Header("Tail")]
    public AIEarthGuardianTailCombatManager secondBody;

    //will have to add motible colliders depending on where the damage is comming fromt
    [Header("Damage Colliders")]
    [SerializeField] EarthGuardianDamageCollider bitedamageCollider;
    [SerializeField] EarthGuardianDamageCollider[] slamdamageCollider;

    [Header("Colliders")]
    [SerializeField] Collider[] bodyColliders;

    [Header("Damage Modifiers")]
    [SerializeField] float attackBiteDamageModifier = 1.0f;
    [SerializeField] float attackSlamDamageModifier = 1.3f; 
    [SerializeField] float attackSwipeDamageModifier = 1.6f;

    [Header("Rigging Refresh")]
    [SerializeField] RigBuilder[] rig;
    [SerializeField] Rig rigWeight;
    [SerializeField] MultiPositionConstraint[] positionConstraints;

    [Header("VFX")]
    public BasicVfxSpawner slamImpactVFX;

    protected override void Awake()
    {
        base.Awake();

        earthGuardianManager = GetComponentInParent<AIEarthGuardianCharacterManager>();

    }

    private void Start()
    {
        ReRig();
    }

    private void Update()
    {
        if (secondBody == null)
        {
            secondBody = FindFirstObjectByType<AIEarthGuardianTailCombatManager>();
        }

        if (hasChangedTarget.Value)
        {
            hasChangedTarget.Value = false;
            SetAimTarget();
            SetSecondBodyTarget();
            FadeRigWeight(1f);
            secondBody.FadeRigWeight(1f);
        }

        if (earthGuardianManager.bossFightIsActive.Value)
        {
            SyncBodyHealth();
        }

    }

    // Set Damage Values
    public void SetBiteDamage()
    {
        bitedamageCollider.physicalDamage = (int)(baseDamage * attackBiteDamageModifier);
        bitedamageCollider.poiseDamage = (int)(basePoiseDamage * attackBiteDamageModifier);
    }
    public void SetSlamDamage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();// CAN CHANGE THIS TO BE MORE SPECIFIC

        foreach (var collider in slamdamageCollider)
        {
            if (collider != null)
            {
                collider.physicalDamage = (int)(baseDamage * attackSlamDamageModifier);
                collider.poiseDamage = (int)(basePoiseDamage * attackSlamDamageModifier);
            }
        }

    }
    public void SetSwipeDamage()
    {
        foreach (var collider in slamdamageCollider)
        {
            if (collider != null)
            {
                collider.physicalDamage = (int)(baseDamage * attackSwipeDamageModifier);
                collider.poiseDamage = (int)(basePoiseDamage * attackSwipeDamageModifier);
            }
        }
    }



    //Open and Close Colliders
    public void OpenBiteDamageCollider()
    {
        bitedamageCollider.EnableDamageCollider();
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();// CAN CHANGE THIS TO BE MORE SPECIFIC
        //earthGuardianManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(earthGuardianManager.earthGuardianSoundFXManager.attackingWhooshes));
    }
    public void CloseBiteDamageCollider()
    {
        bitedamageCollider.DisableDamageCollider();
    }
    public void OpenSlamSwipeDamageCollider()
    {

        foreach (var collider in slamdamageCollider)
        {
            if (collider != null)
            {
                collider.EnableDamageCollider();
            }
        }
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();// CAN CHANGE THIS TO BE MORE SPECIFIC
        //earthGuardianManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(earthGuardianManager.earthGuardianSoundFXManager.attackingWhooshes));

    }
    public void CloseSlamSwipeDamageCollider()
    {

        foreach (var collider in slamdamageCollider)
        {
            if (collider != null)
            {
                collider.DisableDamageCollider();
            }
        }
    }
    public void TurnOffCollidersOnTriggers()
    {
        foreach (var collider in bodyColliders)
        {
            if (collider != null)
            {
                collider.isTrigger = false;
            }
        }
    }
    public void TurnOnCollidersOnTriggers()
    {
        foreach (var collider in bodyColliders)
        {
            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }
    }
    public void PlayWhooshSound()
    {
        earthGuardianManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(earthGuardianManager.earthGuardianSoundFXManager.attackingWhooshes));
    }

    //Rigging Functions
    public void SetAimTarget()
    {
        hasChangedTarget.Value = false;

        if (currentTarget == null) return;

        Transform target = currentTarget.characterCombatManager.lockOnTransform;

        foreach (var constraint in positionConstraints)
        {
            if (constraint == null) continue;

            WeightedTransformArray data = new WeightedTransformArray();
            data.Clear();
            data.Add(new WeightedTransform(target, 1f));
            constraint.data.sourceObjects = data;
        }

        ReRig();
    }
    public void SetSecondBodyTarget()
    {
        secondBody.currentTarget = currentTarget;
        secondBody.SetAimTarget();
    }
    public void FadeRigWeight(float targetWeight)
    {
        StopCoroutine(FadeRigWeightRoutine(targetWeight, 0.75f));
        //StopAllCoroutines(); // optional: stop any previous blend in progress
        StartCoroutine(FadeRigWeightRoutine(targetWeight, 0.75f));
    }
    private IEnumerator FadeRigWeightRoutine(float targetWeight, float duration)
    {
        float startWeight = rigWeight.weight;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            rigWeight.weight = Mathf.Lerp(startWeight, targetWeight, t);
            yield return null;
        }

        rigWeight.weight = targetWeight; // make sure it's set exactly at the end
    }
    public void ReRig()
    {
        foreach (var rigBuilder in rig)
        {
            if (rigBuilder != null)
                rigBuilder.Build();
        }
    }

    public void SyncBodyHealth()
    {
        Unity.Netcode.NetworkVariable<int> thisBodyHP = GetComponentInParent<AIBossCharacterNetworkManager>().currentHealth;
        Unity.Netcode.NetworkVariable<int> secondBodyHP = secondBody.GetComponentInParent<AIBossCharacterNetworkManager>().currentHealth;

        if (thisBodyHP.Value > secondBodyHP.Value)
        {
            thisBodyHP.Value = secondBodyHP.Value;
        }
        else
        {
            return;
        }
    }

    public void ForceSecondBodyUnburrow()
    {
        if (secondBody != null)
        {
            secondBody.earthGuardianManager.ForceBurrowAttack();
        }
    }

    public void SlamImpactVFX()
    {
        slamImpactVFX.ActivateVFX();    
    }
}
