using UnityEngine;

public class AIOgreCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] OgreMeleeDamageCollider rightHandDamageCollider;
    [SerializeField] OgreMeleeDamageCollider LeftHandDamageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;
    [SerializeField] float attack02DamageModifier = 1.4f;

    public void SetAttack01Damage()
    {
        rightHandDamageCollider.physicalDamage = (int)(baseDamage * attack01DamageModifier);
        LeftHandDamageCollider.physicalDamage = (int)(baseDamage * attack01DamageModifier);
    }

    public void SetAttack02Damage()
    {
        rightHandDamageCollider.physicalDamage = (int)(baseDamage * attack02DamageModifier);
        LeftHandDamageCollider.physicalDamage = (int)(baseDamage * attack02DamageModifier);
    }

    public void OpenRightHandDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void DisableRightHandDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void OpenLeftHandDamageCollider()
    {
        LeftHandDamageCollider.EnableDamageCollider();
    }

    public void DisableLeftHandDamageCollider()
    {
        LeftHandDamageCollider.DisableDamageCollider();
    }
}
