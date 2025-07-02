using UnityEngine;
using UnityEngine.TextCore.Text;

public class AISnakeBossCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] SnakeBossDamageCollider damageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;

    public void SetAttack01Damage()
    {
        damageCollider.physicalDamage = (int)(baseDamage * attack01DamageModifier);
    }

    public void OpenDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        damageCollider.EnableDamageCollider();
    }

    public void DisableDamageCollider()
    {
        damageCollider.DisableDamageCollider();
    }
}
