using UnityEngine;

public class WeaponItem : Item
{
    //animator controller override (change attakc animations based on weapon you are currently using)

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
    public float heavyAttackModifer01 = 1.4f;
    public float chargedAttackModifer01 = 2.0f;
    //light attach modifier
    //heavy attack modifier
    //critial damage modifier

    [Header("Stamina Costs Modifiers")]
    public int baseStaminaCost = 20;
    public float meleeAttackStaminaCostMultiplier = 0.9f;
    public float lightAttackStaminaCostMultiplier = 0.9f;
    public float heavyAttackStaminaCostMultiplier = 0.9f;
    public float chargedAttackStaminaCostMultiplier = 0.9f;
    //running attack stamina cost modifier (low importance)
    //heavy attack stamina cost modier


    [Header("Actions")]
    public WeaponItemAction oneHandedRBAction;
    public WeaponItemAction oneHandedRTAction;

    //ash of war (not happening)

    //blocking sounds

}
