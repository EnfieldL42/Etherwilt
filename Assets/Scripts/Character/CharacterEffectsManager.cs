using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    //process instant effcts (dmg, heals)

    //process timed effects (poison, buil ups)

    //process static effects (adding remoing buffs from talsmans etc)

    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;
    [SerializeField] GameObject criticalBloodSplatterVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }

    public void PlayCriticalBloodSplatterVFX(Vector3 contactPoint)
    {
        if(bloodSplatterVFX != null)//if we manually have placed a blood splatter on this model, play this version (this is used so if we have dfferent enemies dont have blood you can put somethng different)
        {
            GameObject bloodSplatter = Instantiate(criticalBloodSplatterVFX, contactPoint, Quaternion.identity);



        }
        else //use the default/generic version
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.criticalBloodSplatterVFX, contactPoint, Quaternion.identity);

        }
    }
    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        if(bloodSplatterVFX != null)//if we manually have placed a blood splatter on this model, play this version (this is used so if we have dfferent enemies dont have blood you can put somethng different)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);



        }
        else //use the default/generic version
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);

        }
    }
}
