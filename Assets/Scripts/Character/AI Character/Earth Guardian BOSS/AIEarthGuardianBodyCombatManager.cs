using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.Text;

public class AIEarthGuardianBodyCombatManager : AICharacterCombatManager
{
    AIEarthGuardianCharacterManager earthGuardianManager;

    [Header("Tail")]
    [SerializeField] AIEarthGuardianTailCombatManager secondBody;

    //will have to add motible colliders depending on where the damage is comming fromt
    [Header("Damage Colliders")]
    [SerializeField] EarthGuardianBodyDamageCollider bitedamageCollider;
    [SerializeField] EarthGuardianBodyDamageCollider[] slamdamageCollider;

    [Header("Colliders")]
    [SerializeField] Collider[] bodyColliders;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attackBiteDamageModifier = 1.0f;
    [SerializeField] float attackSlamDamageModifier = 1.3f; 
    [SerializeField] float attackSwipeDamageModifier = 1.6f;

    [Header("Rigging Refresh")]
    [SerializeField] RigBuilder[] rig;
    [SerializeField] Rig rigWeight;
    [SerializeField] MultiPositionConstraint[] positionConstraints;

    protected override void Awake()
    {
        base.Awake();

        earthGuardianManager = GetComponentInParent<AIEarthGuardianCharacterManager>();
    }

    private void Start()
    {
        foreach (var rigBuilder in rig)
        {
            if (rigBuilder != null)
                rigBuilder.Build();
        }
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
    }

    // Set Damage Values
    public void SetBiteDamage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();// CAN CHANGE THIS TO BE MORE SPECIFIC

        bitedamageCollider.physicalDamage = (int)(baseDamage * attackBiteDamageModifier);
    }
    public void SetSlamDamage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();// CAN CHANGE THIS TO BE MORE SPECIFIC

        foreach (var collider in slamdamageCollider)
        {
            if (collider != null)
            {
                collider.physicalDamage = (int)(baseDamage * attackSlamDamageModifier);
            }
        }

    }
    public void SetSwipeDamage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();// CAN CHANGE THIS TO BE MORE SPECIFIC
        foreach (var collider in slamdamageCollider)
        {
            if (collider != null)
            {
                collider.physicalDamage = (int)(baseDamage * attackSwipeDamageModifier);
            }
        }
    }



    //Open and Close Colliders
    public void OpenBiteDamageCollider()
    {
        bitedamageCollider.EnableDamageCollider();
        earthGuardianManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(earthGuardianManager.earthGuardianSoundFXManager.attackingWhooshes));
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

        earthGuardianManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(earthGuardianManager.earthGuardianSoundFXManager.attackingWhooshes));

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



    //Special Functions
    public void ActivateBorrow()
    {
        //character.characterController.enabled = false;
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

        foreach (var rigBuilder in rig)
        {
            if (rigBuilder != null)
                rigBuilder.Build();
        }
    }
    public void SetSecondBodyTarget()
    {
        secondBody.currentTarget = currentTarget;
        secondBody.SetAimTarget();
    }
    public void FadeRigWeight(float targetWeight)
    {
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



}
