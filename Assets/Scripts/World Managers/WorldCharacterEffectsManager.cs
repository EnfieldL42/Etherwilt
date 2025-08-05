using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;

    [Header("VFX")]
    public GameObject bloodSplatterVFX;
    public GameObject criticalBloodSplatterVFX;

    [Header("Damage")]
    public TakeDamageEffect takeDamageEffect;
    public TakeBlockedDamage takeBlockedDamageEffect;
    public TakeCriticalDamageEffect takeCriticalDamageEffect;

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

        DontDestroyOnLoad(gameObject);
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
