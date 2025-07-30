using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.Text;

public class AIEarthGuardianTailCombatManager : AICharacterCombatManager
{
    AIEarthGuardianCharacterManager earthGuardianManager;


    [Header("Body")]
    public AIEarthGuardianBodyCombatManager secondBody;

    //will have to add motible colliders depending on where the damage is comming fromt
    [Header("Damage Colliders")]
    [SerializeField] EarthGuardianAEODamageCollider aoeCollider;
    [SerializeField] EarthGuardianBodyDamageCollider[] stabdamageCollider;
    [SerializeField] EarthGuardianBodyDamageCollider[] slamdamageCollider;
    public float AEOEffectRadius = 1.5f;

    [Header("Colliders")]
    [SerializeField] Collider[] tailColliders;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] int basePoiseDamage = 25;
    [SerializeField] float attackSlamDamageModifier = 1.6f;
    [SerializeField] float attackStabDamageModifier = 1.0f;
    [SerializeField] float attackStabHeavyDamageModifier = 2f;
    [SerializeField] float attackSwingDamageModifier = 1.4f;
    [SerializeField] float attackSwipeDamageModifier = 1.4f;
    public float AOEDamage = 25;


    [Header("Rigging Refresh")]
    [SerializeField] RigBuilder[] rig;
    [SerializeField] Rig rigWeight;
    [SerializeField] MultiPositionConstraint[] positionConstraints;


    [Header("Separation and Orbiting")]
    [SerializeField] private float minSeparation = 5f;
    [SerializeField] private float orbitStrength = 0.5f;

    [Header("VFX")]
    public GameObject earthGuardianVFX;


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
            secondBody = FindFirstObjectByType<AIEarthGuardianBodyCombatManager>();
        }

        if (currentTarget != null && secondBody != null)
        {
            if (!character.isPerformingAction)
            {
                AvoidOverlapWhileOrbitingPlayer(currentTarget, secondBody, character.characterController, minSeparation, orbitStrength);
            }
        }

        if(!character.isPerformingAction && rigWeight.weight == 0 && !earthGuardianManager.sleepState)
        {
            FadeRigWeight(1f);
        }

    }



    // Set Damage Values
    public void SetAttackSlamDamage()
    {
        foreach (var collider in slamdamageCollider)
        {
            if (collider != null)
            {
                collider.physicalDamage = (int)(baseDamage * attackSlamDamageModifier);
                collider.poiseDamage = (int)(basePoiseDamage * attackSlamDamageModifier);
            }
        }
    }
    public void SetAttackStabDamage()
    {
        foreach (var collider in stabdamageCollider)
        {
            if (collider != null)
            {
                collider.physicalDamage = (int)(baseDamage * attackStabDamageModifier);
                collider.poiseDamage = (int)(basePoiseDamage * attackStabDamageModifier);
            }
        }
        //stabdamageCollider.physicalDamage = (int)(baseDamage * attackStabDamageModifier);
    }
    public void SetAttackStabHeavyDamage()
    {
        foreach (var collider in stabdamageCollider)
        {
            if (collider != null)
            {
                collider.physicalDamage = (int)(baseDamage * attackStabHeavyDamageModifier);
                collider.poiseDamage = (int)(basePoiseDamage * attackStabHeavyDamageModifier);
            }
        }
        //.physicalDamage = (int)(baseDamage * attackStabHeavyDamageModifier);
    }
    public void SetAttackSwingDamage()
    {
        foreach (var collider in slamdamageCollider)
        {
            if (collider != null)
            {
                collider.physicalDamage = (int)(baseDamage * attackSwingDamageModifier);
                collider.poiseDamage = (int)(basePoiseDamage * attackSwingDamageModifier);
            }
        }
    }
    public void SetAttackSwipeDamage()
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
    public void OpenStabDamageColliders()
    {

        foreach (var collider in stabdamageCollider)
        {
            if (collider != null)
            {
                collider.EnableDamageCollider();
            }
        }

        earthGuardianManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(earthGuardianManager.earthGuardianSoundFXManager.attackingWhooshes));

    }
    public void CloseStabDamageColliders()
    {
        foreach (var collider in stabdamageCollider)
        {
            if (collider != null)
            {
                collider.DisableDamageCollider();
            }
        }
    }
    public void OpenSlamDamageColliders()
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
    public void CloseSlamDamageColliders()
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
        foreach (var collider in tailColliders)
        {
            if (collider != null)
            {
                collider.isTrigger = false;
            }
        }
    }
    public void TurnOnCollidersOnTriggers()
    {
        foreach (var collider in tailColliders)
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

    }
    public void ActivateAOEEffect()
    {
        aoeCollider.HeavyStabEffect();
    }
    public void AvoidOverlapWhileOrbitingPlayer(CharacterManager currentTarget, AIEarthGuardianBodyCombatManager secondBody, CharacterController characterController, float minSeparation, float orbitStrength)
    {

        Transform targetTransform = currentTarget.transform;
        Transform secondBodyTransform = secondBody.transform;

        Vector3 toOther = secondBodyTransform.position - transform.position;
        float distance = toOther.magnitude;

        if (distance < minSeparation)
        {
            Vector3 toPlayer = targetTransform.position - transform.position;

            // Determine whether second body is to the left or right of this object relative to the player
            Vector3 toSecondBodyFromPlayer = secondBodyTransform.position - targetTransform.position;
            Vector3 toThisFromPlayer = transform.position - targetTransform.position;

            float direction = Mathf.Sign(Vector3.SignedAngle(toThisFromPlayer, toSecondBodyFromPlayer, Vector3.up));

            // Rotate *away* from the second body around the player
            Vector3 orbitDirection = Vector3.Cross(Vector3.up * direction, toPlayer).normalized;

            // Move slightly away from the second body
            Vector3 awayFromOther = -toOther.normalized;

            // Combine orbital movement and avoidance
            Vector3 moveDirection = Vector3.Lerp(awayFromOther, orbitDirection, 0.5f);

            // Remove any movement toward the player
            Vector3 toPlayerDir = toPlayer.normalized;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, toPlayerDir).normalized;

            characterController.Move(moveDirection * orbitStrength * Time.deltaTime);
        }
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
    public void ReRig()
    {
        foreach (var rigBuilder in rig)
        {
            if (rigBuilder != null)
                rigBuilder.Build();
        }
    }

}
