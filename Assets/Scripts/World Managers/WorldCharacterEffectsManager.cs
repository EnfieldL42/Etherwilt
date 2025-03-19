using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;

    [SerializeField] List<InstantCharacterEffect> instantEffects; 

    public void Awake()
    {
        if ((instance == null))
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GenerateEffectsIDs();
    }


    private void GenerateEffectsIDs()
    {
        for(int i = 0; i < instantEffects.Count; ++i)
        {
            instantEffects[i].instantEffectID = i;
        }
    }

}
