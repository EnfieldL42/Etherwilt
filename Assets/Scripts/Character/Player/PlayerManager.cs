using UnityEngine;

public class PlayerManager : CharacterManager
{
    public PlayerLocomotionManager playerLocomotionManager;
    public PlayerAnimatorManager playerAnimatorManager;
    public PlayerStatsManager playerStatsManager;
    public PlayerUIHudManager playerUIHudManager;

    protected override void Awake()
    {
        base.Awake(); //ok so its like an awake that happens after the main awake in the parent class(charactermanager) /(not too sure why this is useful just yet)
        PlayerCamera.instance.player = this;
        PlayerInputManager.instance.player = this;


        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerUIHudManager = FindFirstObjectByType<PlayerUIHudManager>();

        playerStatsManager.OnStaminaChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
        playerStatsManager.OnStaminaChanged += playerStatsManager.ResetStaminaRegenTimer; //currently not working
        playerStatsManager.maxStamina = playerStatsManager.CalculateStaminaBasedOnLevel(playerStatsManager.endurance);
        playerStatsManager.currentStamina = playerStatsManager.CalculateStaminaBasedOnLevel(playerStatsManager.endurance);

        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerStatsManager.maxStamina);

    }

    protected override void LateUpdate()
    {
        if(!IsOwner)
        {
            return;
        }
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    protected override void Update()
    {
        base.Update();

        if(!IsOwner)//if we dont own the gameobject we cant control it
        {
            return;
        }

        playerLocomotionManager.HandleAllMovement();
        //playerStatsManager.RegenerateStamina();
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            PlayerCamera.instance.player = this;
        }    

    }

}
