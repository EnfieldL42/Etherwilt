using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    //process instant effcts (dmg, heals)

    //process timed effects (poison, buil ups)

    //process static effects (adding remoing buffs from talsmans etc)

    CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }

}
