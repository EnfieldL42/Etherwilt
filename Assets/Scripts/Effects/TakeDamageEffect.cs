using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0; //usually split into standard, strike, slash and piece
    public float magicDamage = 0;

    [Header("Final Damage")]
    protected int finalDamageDealt = 0;

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

        if (character.characterNetworkManager.isInvulnerable.Value)
        {
            return;
        }


        base.ProcessEffect(character);

        if(character.isDead.Value)
        {
            return;
        }

        CalculateDamage(character);
        PlayDirectonalBasedDamageAnimation(character);

        //check for build ups
        PlayDamageSFX(character);
        PlayDamageVFX(character);

        CalculateStanceDamage(character);
        //if character is ai, check for new target if chararcter caysubg damage is nearby


    }

    protected virtual void CalculateDamage(CharacterManager character)
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

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
        character.characterStatsManager.totalPoiseDamanage -= poiseDamage;   //subtract poise damage from character total
        character.characterCombatManager.previousPoiseDamageTaken = poiseDamage; //store previous poise damage taken for other interactions

        float remainingPoise = character.characterStatsManager.basePoiseDefense + character.characterStatsManager.offensivePoiseBonus + character.characterStatsManager.totalPoiseDamanage;

        if(remainingPoise <= 0)
        {
            poiseIsBroken = true;
        }

        character.characterStatsManager.poiseResetTimer = character.characterStatsManager.defaultPoiseResetTime;
    }

    protected void CalculateStanceDamage(CharacterManager character)
    {
        AICharacterManager aiCharacter = character as AICharacterManager;

        int stanceDamage = Mathf.RoundToInt(poiseDamage);

        if(aiCharacter != null)
        {
            aiCharacter.aICharacterCombatManager.DamageStance(stanceDamage);
        }
    }

    protected void PlayDamageVFX(CharacterManager character)
    {
        character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
    }

    protected void PlayDamageSFX(CharacterManager character)
    {
        AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

        character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
        character.characterSoundFXManager.PlayDamageGruntSoundFX();


    }

    protected void PlayDirectonalBasedDamageAnimation(CharacterManager character, bool isPerformingAction = true)
    {
        if (!character.IsOwner)
        {
            return;
        }

        if (character.isDead.Value)
        {
            return;
        }

        if (poiseIsBroken)
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                //play front anim
                damageAnimation = character.characterAnimatorManager.hit_Forward_Medium_01;
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                //play front anim
                damageAnimation = character.characterAnimatorManager.hit_Forward_Medium_01;
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                //play back anim
                damageAnimation = character.characterAnimatorManager.hit_Backward_Medium_01;
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                //play left anim
                damageAnimation = character.characterAnimatorManager.hit_Left_Medium_01;
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                //play right anim
                damageAnimation = character.characterAnimatorManager.hit_Right_Medium_01;
            }
        }
        else
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                //play front anim
                damageAnimation = character.characterAnimatorManager.forward_Ping_Damage;
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                //play front anim
                damageAnimation = character.characterAnimatorManager.forward_Ping_Damage;
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                //play back anim
                damageAnimation = character.characterAnimatorManager.backward_Ping_Damage;
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                //play left anim
                damageAnimation = character.characterAnimatorManager.left_Ping_Damage;
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                //play right anim
                damageAnimation = character.characterAnimatorManager.right_Ping_Damage;
            }

            character.characterAnimatorManager.lastDamageAnimationPlayer = damageAnimation;

            if(poiseIsBroken)
            {
                //if poise broken, stun character
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
            }
            else
            {
                //if not poise broken, just play hit animation with no restriction
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, false, false , true, true);

            }
        }
    }
}
