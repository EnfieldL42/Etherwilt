using UnityEngine;


public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamange;

    [Header("Damage")]
    public float physicalDamage = 0; //usually split into standard, strike, slash and piece
    public float magicDamage = 0;
    private float finalDamage = 0;

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false;


    //MAYBE TODO build ups

    [Header("Animations")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlaySFX = true;
    public AudioClip elementalDamageSoundFX; //to add extra sound on top of the slashing sfx, if the weapon is enchanted,etc..

    [Header("Direction Damage Taken From")]
    public float angleHitFrom; //to determine what damage anim to play (move backwards, forwards, sideways..)
    public Vector3 contactPoint;//to determine where the blood fx instantiates


    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);

        if(character.isDead.Value)
        {
            return;
        }


    }
}
