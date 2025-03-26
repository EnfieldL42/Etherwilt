using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamange;

    [Header("Damage")]
    public float physicalDamage = 0; //usually split into standard, strike, slash and piece
    public float magicDamage = 0;
    private int finalDamageDealt = 0;

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
        //check for invulnerability

        CalculateDamage(character);

        //check direction of dmg origin
        //play dmg anim
        //check for build ups
        //play dmg sfx
        //play dmg vfx

        //if character is ai, check for new target if chararcter caysubg damage is nearby
    }

    private void CalculateDamage(CharacterManager character)
    {
        if(!character.IsOwner)
        {
            return;
        }

        if(characterCausingDamange != null)
        {
            //check for gmd modifiers and modify base dmg (physical and magic buffs)
        }

        //check character for flat dmg reduction and subtract from damage

        //check for character armor absorptions, subtract te percentage from dmg

        //add all dmg types together and apply dmg

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage);

        if(finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;



    }

}
