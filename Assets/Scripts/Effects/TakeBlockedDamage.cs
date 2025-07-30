using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Blocked Damage")]
public class TakeBlockedDamage : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0; //usually split into standard, strike, slash and piece
    public float magicDamage = 0;
    private int finalDamageDealt = 0;

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false;

    [Header("Stamina")]
    public float staminaDamage = 0;
    public float finalStaminaDamage = 0;


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

        if (character.characterNetworkManager.isInvulnerable.Value)
        {
            return;
        }


        base.ProcessEffect(character);

        Debug.Log("Hit was blocked");

        CalculateDamage(character);
        CalculateStamanaDamage(character);
        PlayDirectonalBasedBlockingAnimation(character);

        //check for build ups
        PlayDamageSFX(character);
        PlayDamageVFX(character);

        //if character is ai, check for new target if chararcter caysubg damage is nearby

        CheckForGuardBreak(character);
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner)
        {
            return;
        }

        if (characterCausingDamage != null)
        {
            //check for gmd modifiers and modify base dmg (physical and magic buffs)
        }

        character.canTakeDmgAnimation = false;
        character.actionTimer = 0f; //reset action timer so character can take dmg again

        //check character for flat dmg reduction and subtract from damage

        //check for character armor absorptions, subtract te percentage from dmg

        //add all dmg types together and apply dmg

        Debug.Log("Original Physical Damage: " + physicalDamage);

        physicalDamage -= (physicalDamage * (character.characterStatsManager.blockingPhyicalAbsorption / 100));
        magicDamage -= (magicDamage * (character.characterStatsManager.blockingMagicAbsorption / 100));

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        Debug.Log("Final Physical Damage: " + finalDamageDealt);

        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

    }

    private void CalculateStamanaDamage(CharacterManager character)
    {
        if (!character.IsOwner)
        {
            return;
        }

        finalStaminaDamage = staminaDamage;

        float staminaDamageAbsorption = finalStaminaDamage * (character.characterStatsManager.blockingStability / 100);
        float staminaDamageAfterAbsorption = finalStaminaDamage - staminaDamageAbsorption;

        character.characterNetworkManager.currentStamina.Value -= staminaDamageAfterAbsorption;
    }

    private void CheckForGuardBreak(CharacterManager character)
    {
        //play sfx
        if (!character.IsOwner)
        {
            return;
        }

        if (character.characterNetworkManager.currentStamina.Value <= 0)
        {
            character.characterAnimatorManager.PlayTargetActionAnimation("Guard_Break_01", true);
            character.characterNetworkManager.isBlocking.Value = false;
  
        }
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        //vfx based on material
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        //sfx based on material
        character.characterSoundFXManager.PlayBlockSoundFX();

    }

    private void PlayDirectonalBasedBlockingAnimation(CharacterManager character, bool isPerformingAction = true)
    {
        if (!character.IsOwner)
        {
            return;
        }

        if (character.isDead.Value)
        {
            return;
        }

        DamageIntensity damageIntensity = WorldUtilityManager.instance.GetDamageIntensityBasedOnPoiseDamage(poiseDamage);


        switch(damageIntensity)
        {
            case DamageIntensity.Normal:
                damageAnimation = "block_Normal_01";
                break;
            default:
                damageAnimation = "block_Normal_01";
                break;
        }


        character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
       
    }
}
