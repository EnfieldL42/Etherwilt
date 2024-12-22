using System;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager characterManager;
    PlayerLocomotionManager playerLocomotionManager;

    public int endurance = 1;
    public float currentStamina = 0;
    public float maxStamina;


    [Header("StaminaRegenation")]
    [SerializeField] float staminaRegenerationAmount = 2;
    [SerializeField] private float staminaRegenarationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2;

    public event Action<float, float> OnStaminaChanged;


    protected virtual void Update()
    {
        SetStamina(currentStamina);
    }
    protected virtual void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }

    public void SetStamina(float newStamina)
    {
        float oldStamina = currentStamina;
        currentStamina = Mathf.Clamp(newStamina, 0, maxStamina); // Ensure stamina stays within valid bounds

        // Trigger the event with both old and new values
        OnStaminaChanged?.Invoke(oldStamina, currentStamina);

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
        if (playerLocomotionManager.isSprinting)
        {
            return;

        }

        if (characterManager.isPerformingAction)
        {
            return;
        }



        staminaRegenarationTimer += Time.deltaTime;

        if (staminaRegenarationTimer >= staminaRegenerationDelay)
        {
            if (currentStamina < maxStamina)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    currentStamina += staminaRegenerationAmount;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
    {

        if(newValue < oldValue)//check if we are using stamina or gaining stamina as we dont want to reset the timer when gaining stamina
        {
            print("timer reset");

        }

    }

}
