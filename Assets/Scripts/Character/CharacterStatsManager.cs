using System;
using System.Globalization;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Ether")]
    public int etherDroppedOnDeath = 50;

    [Header("StaminaRegenation")]
    [SerializeField] float staminaRegenerationAmount = 2;
    [SerializeField] private float staminaRegenarationTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2;
    private float staminaTickTimer = 0;

    [Header("Blocking Absorptions")]
    public float blockingPhyicalAbsorption;
    public float blockingMagicAbsorption;
    public float blockingStability;

    [Header("Poise")]
    public float totalPoiseDamanage;             //how much poise damage it has taken
    public float offensivePoiseBonus;            //poise bonus from using weapons
    public float basePoiseDefense;               //poise bonus from armor/talismans, etc.
    public float defaultPoiseResetTime = 8;      //time it takes for poise damage to reset (musnt be hit for the duration of the timer)
    public float poiseResetTimer = 0;            //current timer for poise reset


    protected virtual void Update()
    {
        HandlePoiseResetTimer();
    }
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void Start()
    {
 
    }


    public int CalculateHealthBasedOnLevel(int vitality)
    {
        int health = 10;

        //can change this logic to fit whatever stamina should be depending on endurance points
        health = vitality * 15;

        return health;
    }


    public float CalculateStaminaBasedOnLevel(int endurance)
    {
        float stamina = 10;

        //can change this logic to fit whatever stamina should be depending on endurance points
        stamina = endurance * 10;

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

    protected virtual void HandlePoiseResetTimer()
    {
        if(poiseResetTimer > 0)
        {
            poiseResetTimer -= Time.deltaTime;
        }
        else
        {
            totalPoiseDamanage = 0;
        }
    }


}
