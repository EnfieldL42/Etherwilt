using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    [Header("Weapon Attack Modifiers")]
    public float light_Attack_01_Modifier;
    public float light_Attack_02_Modifier;
    public float light_Attack_03_Modifier;
    public float light_Attack_04_Modifier;
    public float heavy_Attack_01_Modifier;
    public float charged_Attack_01_Modifier;
    public float running_Attack_01_Modifier;
    public float rolling_Attack_01_Modifier;
    public float backstep_Attack_01_Modifier;

    protected override void Awake()
    {
        base.Awake();

        if(damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }
        damageCollider.enabled = false; //always be unabled and only let animation event allow collider to turn on

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

            if (damageTarget.characterGroup == character.characterGroup)
            {
                return;
            }

            DamageTarget(damageTarget);
        }
    }

    protected override void GetBlockedDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = characterCausingDamage.transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        //prevent dmg from being hit twice in a single attack -> add a lust that checks before applying dmg

        if (charactersDamaged.Contains(damageTarget))
        {
            return;
        }

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

        switch(characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.LightAttack02:
                ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                break;
            case AttackType.LightAttack03:
                ApplyAttackDamageModifiers(light_Attack_03_Modifier, damageEffect);
                break;
            case AttackType.LightAttack04:
                ApplyAttackDamageModifiers(light_Attack_04_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack01:
                ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack02:
                ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack03:
                ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.ChargedAttack01:
                ApplyAttackDamageModifiers(charged_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.ChargedAttack02:
                ApplyAttackDamageModifiers(charged_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.ChargedAttack03:
                ApplyAttackDamageModifiers(charged_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.RunningAttack01:
                ApplyAttackDamageModifiers(running_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.RollingAttack01:
                ApplyAttackDamageModifiers(rolling_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.BackstepAttack01:
                ApplyAttackDamageModifiers(backstep_Attack_01_Modifier, damageEffect);
                break;
            default:
                break;
        }

        //damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

        if(characterCausingDamage.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                characterCausingDamage.NetworkObjectId,
                damageEffect.physicalDamage,
                damageEffect.magicDamage,
                damageEffect.poiseDamage,
                damageEffect.angleHitFrom,
                damageEffect.contactPoint.x,
                damageEffect.contactPoint.y,
                damageEffect.contactPoint.z);

        }
    }

    private void ApplyAttackDamageModifiers(float attackModifier, TakeDamageEffect damage)
    {

        float weaponMastery = Mathf.Clamp(characterCausingDamage.characterNetworkManager.weaponMastery.Value, 0, 99);
        float magicMastery = Mathf.Clamp(characterCausingDamage.characterNetworkManager.magicMastery.Value, 0, 99);
        float breakerMastery = Mathf.Clamp(characterCausingDamage.characterNetworkManager.breakerMastery.Value, 0, 99);

        float weaponScale = 1f + (weaponMastery / 99f) * 1f;
        float magicScale = 1f + (magicMastery / 99f) * 1f;
        float breakerScale = 1f + (breakerMastery / 99f) * 1f;

        // --- apply to damage ---
        damage.physicalDamage = Mathf.RoundToInt(damage.physicalDamage * attackModifier * weaponScale);
        damage.magicDamage = Mathf.RoundToInt(damage.magicDamage * attackModifier * magicScale);
        damage.poiseDamage = Mathf.RoundToInt(damage.poiseDamage * attackModifier * breakerScale);

        Debug.Log("poise " + damage.poiseDamage);
        Debug.Log("physical  " + damage.physicalDamage);
        //if attack is fullt charged heavy, multiply charge modifier after normal modifiers have been calculated
    }
}
