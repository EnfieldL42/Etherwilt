using UnityEngine;
using UnityEngine.TextCore.Text;

public class EarthGuardianDamageCollider : DamageCollider
{
    [SerializeField] public AIBossCharacterManager bossCharacter;

    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponent<Collider>();
        bossCharacter = GetComponentInParent<AIBossCharacterManager>();
    }

    protected override void GetBlockedDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = bossCharacter.transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        //prevent dmg from being hit twice in a single attack -> add a lust that checks before applying dmg

        if (parentCombatManager.damagedCharactersThisAttack.Contains(damageTarget))
        {
            return;
        }

        parentCombatManager.damagedCharactersThisAttack.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(bossCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

        if (damageTarget.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                bossCharacter.NetworkObjectId,
                damageEffect.physicalDamage,
                damageEffect.magicDamage,
                damageEffect.poiseDamage,
                damageEffect.angleHitFrom,
                damageEffect.contactPoint.x,
                damageEffect.contactPoint.y,
                damageEffect.contactPoint.z);
        }
    }

}
