using UnityEngine;

public class ManualDamageCollider : DamageCollider
{
    [SerializeField] public AICharacterManager aiCharacter;
    [SerializeField] AICharacterCombatManager parentCombatManager;

    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponent<Collider>();
        aiCharacter = GetComponentInParent<AICharacterManager>();
        parentCombatManager = GetComponentInParent<AICharacterCombatManager>();
    }

    private void Start()
    {
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        //CharacterManager target = other.GetComponent<CharacterManager>();

        if (damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //check for friendly fire

            //check if target is blocking

            if (character.hasMultipleColliders)
            {
                // Prevent multiple hits from different colliders in the same attack
                if (parentCombatManager.damagedCharactersThisAttack.Contains(damageTarget))
                    return;
            }

            if (damageTarget.characterGroup == character.characterGroup)
            {
                return;
            }

            CheckForBlock(damageTarget);
            DamageTarget(damageTarget);
        }

        charactersDamaged.Add(damageTarget);
    }

    protected override void GetBlockedDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = aiCharacter.transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        //prevent dmg from being hit twice in a single attack -> add a lust that checks before applying dmg

        if (charactersDamaged.Contains(damageTarget))
        {
            return;
        }

        aiCharacter.aICharacterCombatManager.hasHitTargetDuringCombo = true;

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(aiCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

        if (damageTarget.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                aiCharacter.NetworkObjectId,
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
