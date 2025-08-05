using UnityEngine;

public class WeaponItem : Item
{
    [Header("Animation")]
    public AnimatorOverrideController weaponAnimator;

    [Header("Model Instantiation")]
    public WeaponModelType weaponModelType;

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Requirements")]
    public int strengthREQ = 0;
    public int dexREQ = 0;
    public int intREQ = 0;
    public int faithREQ = 0;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int magicDamage = 0;

    [Header("Weapon Base Poise Damage")]
    public float poiseDamage = 10;
    //offensive poise bonus dmg when attacking


    [Header("Damage Modifier")]
    public float lightAttackModifer01 = 1.1f;
    public float lightAttackModifer02 = 1.2f;
    public float lightAttackModifer03 = 1.3f;
    public float lightAttackModifer04 = 1.4f;

    public float heavyAttackModifer01 = 1.4f;
    public float heavyAttackModifer02 = 1.6f;
    public float heavyAttackModifer03 = 1.8f;

    public float chargedAttackModifer01 = 2.0f;
    public float chargedAttackModifer02 = 2.2f;
    public float chargedAttackModifer03 = 2.4f;

    public float runningAttackModifer01 = 1.2f;
    public float rollingAttackModifer01 = 1.1f;
    public float backstepAttackModifer01 = 1.1f;

    public float riposteAttackModifer01 = 3.3f;

    [Header("Stamina Costs Modifiers")]
    public int baseStaminaCost = 20;
    public float meleeAttackStaminaCostMultiplier = 1f;
    public float lightAttackStaminaCostMultiplier = 1f;
    public float heavyAttackStaminaCostMultiplier = 1.3f;
    public float chargedAttackStaminaCostMultiplier = 1.5f;
    public float runningAttackStaminaCostMultiplier = 1.1f;
    public float rollingAttackStaminaCostMultiplier = 1.1f;
    public float backstepAttackStaminaCostMultiplier = 1.1f;

    [Header("Weapon Blocking Absorption")]
    public float physicalBaseDamageAbsorption = 50;
    public float magiclBaseDamageAbsorption = 50;
    public float stability = 50;//the higher it is the less stamina you lose

    [Header("Actions")]
    public WeaponItemAction oneHandedRBAction;
    public WeaponItemAction oneHandedRTAction;
    public WeaponItemAction oneHandedLBAction;

    //ash of war (not happening)

    //blocking sounds

    [Header("SFX")]
    public AudioClip[] whooshes;
    public AudioClip[] blocking;

}
