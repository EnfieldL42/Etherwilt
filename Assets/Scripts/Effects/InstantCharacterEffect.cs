using UnityEngine;

public class InstantCharacterEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int instantEffectID;

    [Header("Anti Effect Spamming")]
    private float timer = 0f;

    public virtual void ProcessEffect(CharacterManager character)
    {
        timer =+ Time.deltaTime;

        if (timer < 0.1f)
        {
            return; // Prevent spamming the effect
        }
        else
        {
            timer = 0f; // Reset timer after processing the effect
        }

    }
    
}
