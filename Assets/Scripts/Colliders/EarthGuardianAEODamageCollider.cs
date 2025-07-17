using System.Collections.Generic;
using UnityEngine;

public class EarthGuardianAEODamageCollider : DamageCollider
{

    AIEarthGuardianCharacterManager eaCharacterManager;

    protected override void Awake()
    {
        base.Awake();
        eaCharacterManager = GetComponentInParent<AIEarthGuardianCharacterManager>();
    }

    public void HeavyStabEffect()
    {
        GameObject stabVFX = Instantiate(eaCharacterManager.tailCombatManager.earthGuardianVFX, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, eaCharacterManager.tailCombatManager.AEOEffectRadius, WorldUtilityManager.instance.GetCharacterLayers());
        List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        foreach (var collider in colliders)
        {
            CharacterManager character = collider.GetComponent<CharacterManager>();

            if (character != null)
            {
                if (charactersDamaged.Contains(character))
                {
                    continue;
                }
                if(character == eaCharacterManager)
                {
                    continue;
                }

                charactersDamaged.Add(character);

                if (character.IsOwner)
                {
                    //check for block

                    TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                    damageEffect.physicalDamage = eaCharacterManager.tailCombatManager.AOEDamage;
                    damageEffect.poiseDamage = eaCharacterManager.tailCombatManager.AOEDamage;

                    character.characterEffectsManager.ProcessInstantEffect(damageEffect);
                }
            }
        }
    }    
}
