using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.Text;

public class AIEarthGuardianCombatManager : AICharacterCombatManager
{
    //will have to add motible colliders depending on where the damage is comming fromt
    [Header("Damage Colliders")]
    [SerializeField] EarthGuardianTailDamageCollider bitedamageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;
    [SerializeField] float attack02DamageModifier = 1.4f; 
    [SerializeField] float attack03DamageModifier = 1.6f;

    [Header("Rigging Refresh")]
    public RigBuilder[] rig;
    [SerializeField] private MultiPositionConstraint[] positionConstraints;


    private void Start()
    {
        positionConstraints = GetComponentsInChildren<MultiPositionConstraint>();
    }

    private void Update()
    {
        if (hasChangedTarget.Value)
        {
            hasChangedTarget.Value = false;
            SetAimTarget();
        }
    }
    public void SetAttack01Damage()
    {
        bitedamageCollider.physicalDamage = (int)(baseDamage * attack01DamageModifier);
    }

    public void SetAttack02Damage()
    {
        bitedamageCollider.physicalDamage = (int)(baseDamage * attack02DamageModifier);
    }

    public void SetAttack03Damage()
    {
        bitedamageCollider.physicalDamage = (int)(baseDamage * attack03DamageModifier);
    }

    public void OpenBiteDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        bitedamageCollider.EnableDamageCollider();
    }

    public void CloseBiteDamageCollider()
    {
        bitedamageCollider.DisableDamageCollider();
    }

    public void ActivateBorrow()
    {

    }

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

}
