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


    //weapon modifiers
    //light attach modifier
    //heavy attack modifier
    //critial damage modifier

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
    //running attack stamina cost modifier (low importance)
    //light attack stamina cost modifier
    //heavy attack stamina cost modier


    //item based actions (RB, RT, LB, LT)

    //ash of war (not happening)

    //blocking sounds

}
