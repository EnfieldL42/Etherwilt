using UnityEngine;

public class PlayerManager : CharacterManager
{
    public PlayerLocomotionManager playerLocomotionManager;
    public PlayerAnimatorManager playerAnimatorManager;
    public PlayerStatsManager playerStatsManager;
    public PlayerNetworkManager playerNetworkManager;

    protected override void Awake()
    {
        base.Awake(); //ok so its like an awake that happens after the main awake in the parent class(charactermanager) /(not too sure why this is useful just yet)


        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();



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
        playerStatsManager.RegenerateStamina();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsOwner)
        {
            PlayerInputManager.instance.player = this;
            PlayerCamera.instance.player = this;

            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;


            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStaminaBasedOnLevel(playerNetworkManager.endurance.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }    

    }

}
