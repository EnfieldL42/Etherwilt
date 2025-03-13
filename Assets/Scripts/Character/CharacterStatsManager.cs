using System;
using System.Globalization;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;


    [Header("StaminaRegenation")]
    [SerializeField] float staminaRegenerationAmount = 2;
    [SerializeField] private float staminaRegenarationTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2;
    private float staminaTickTimer = 0;

    protected virtual void Update()
    {

    }
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public float CalculateStaminaBasedOnLevel(int endurance)
    {
        float stamina = 0;

        //can change this logic to fit whatever stamina should be depending on endurance points
        stamina = endurance * 100;

        return stamina;
    }


    public virtual void RegenerateStamina()
    {
        if (!character.IsOwner)
        {
            return;
        }

        if (character.characterNetworkManager.isSprinting.Value)
        {
            return;
        }

        if (character.isPerformingAction)
        {
            return;
        }

        staminaRegenarationTimer += Time.deltaTime;

        if (staminaRegenarationTimer >= staminaRegenerationDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                }
            }
        }

    }

    public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
    {

        if(newValue < oldValue)
        {
            staminaRegenarationTimer = 0;

        }
    }

}
