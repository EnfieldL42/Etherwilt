using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{

    public AICharacterCombatManager parentCombatManager;
    public CharacterManager character;

    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;

    [Header("Damage")]
    public int physicalDamage = 0;
    public int magicDamage = 0;

    [Header("Poise")]
    public float poiseDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    [Header("Block")]
    protected Vector3 directionFromAttackToDamageTarget;
    protected float dotValueFromAttackToDamageTarget;

    protected virtual void Awake()
    {

    }

    private void Start()
    {
        parentCombatManager = GetComponentInParent<AICharacterCombatManager>();
        character = GetComponentInParent<CharacterManager>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        //CharacterManager target = other.GetComponent<CharacterManager>();

        if (damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //check for friendly fire

            //check if target is blocking

            if(character.hasMultipleColliders)
            {
                // Prevent multiple hits from different colliders in the same attack
                if (parentCombatManager.damagedCharactersThisAttack.Contains(damageTarget))
                    return;
            }

            if(damageTarget.characterGroup == character.characterGroup)
            {
                return;
            }

            CheckForBlock(damageTarget);
            DamageTarget(damageTarget);
        }

        charactersDamaged.Add(damageTarget);
    }

    protected virtual void CheckForBlock(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
        {
            return;
        }

        GetBlockedDotValues(damageTarget);

        if (damageTarget.characterNetworkManager.isBlocking.Value && dotValueFromAttackToDamageTarget > 0.3f)
        {
            charactersDamaged.Add(damageTarget);

            TakeBlockedDamage damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedDamageEffect);

            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.staminaDamage = poiseDamage;
            damageEffect.contactPoint = contactPoint;

            damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

        }

    }

    protected virtual void GetBlockedDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        //prevent dmg from being hit twice in a single attack -> add a list that checks before applying dmg

        if (charactersDamaged.Contains(damageTarget)) //used to be charactersDamaged.Contains(damageTarget)
        {
            return;
        }

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contactPoint;

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear(); //reset list of character that got hit on that swing
    }
}
