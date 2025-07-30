using UnityEngine;

public class AIWolfCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] WolfMeleeDamageCollider damageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] int basePoiseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;

    public void SetAttack01Damage()
    {
        damageCollider.physicalDamage = (int)(baseDamage * attack01DamageModifier);
        damageCollider.poiseDamage = (int)(basePoiseDamage * attack01DamageModifier);
    }
    
    public void OpenDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
        damageCollider.EnableDamageCollider();
    }

    public void DisableDamageCollider()
    {
        damageCollider.DisableDamageCollider();
    }
}
