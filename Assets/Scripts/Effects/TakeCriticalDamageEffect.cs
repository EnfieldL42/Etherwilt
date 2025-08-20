using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Critical Damage Effect")]
public class TakeCriticalDamageEffect : TakeDamageEffect
{
    public override void ProcessEffect(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value)
        {
            return;
        }

        if (character.isDead.Value)
        {
            return;
        }

        CalculateDamage(character);

        character.characterCombatManager.pendingCriticalDamage = finalDamageDealt;
    }

    protected override void CalculateDamage(CharacterManager character)
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

        character.characterStatsManager.totalPoiseDamanage -= poiseDamage;   //subtract poise damage from character total
        character.characterCombatManager.previousPoiseDamageTaken = poiseDamage; //store previous poise damage taken for other interactions

        float mastery = Mathf.Clamp(character.characterNetworkManager.tankMastery.Value, 0, 99);
        float tankModifier = 1f + (mastery / 99f) * 1f;

        float remainingPoise = (character.characterStatsManager.basePoiseDefense * tankModifier)
            + character.characterStatsManager.offensivePoiseBonus
            + character.characterStatsManager.totalPoiseDamanage;

        if (remainingPoise <= 0)
        {
            poiseIsBroken = true;
        }

        character.characterStatsManager.poiseResetTimer = character.characterStatsManager.defaultPoiseResetTime;
    }
}
