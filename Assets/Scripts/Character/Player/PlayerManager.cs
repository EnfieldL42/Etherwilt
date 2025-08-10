using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerManager : CharacterManager
{
    [Header("DEBUG MENU")]
    public bool respawnCharacter = false;


    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerInteractionManager playerInteractionManager;
    [HideInInspector] public PlayerEffectsManager playerEffectsManager;

    [HideInInspector] private bool sceneLoaded = false;
    protected override void Awake()
    {
        base.Awake(); //ok so its like an awake that happens after the main awake in the parent class(charactermanager) /(not too sure why this is useful just yet)


        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInteractionManager = GetComponent<PlayerInteractionManager>();
        playerEffectsManager = GetComponent<PlayerEffectsManager>();

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

        DebugMenu();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void OnDisable()
    {
        base.OnDisable();


    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        if(IsOwner)
        {
            PlayerInputManager.instance.player = this;
            PlayerCamera.instance.player = this;
            WorldSaveGameManager.instance.player = this;

            //update health and stamina when vitality/endurance stats changes
            playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;


            //updates ui stat bar when health/stamina changes
            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

        }

        if (!IsOwner)
        {
            characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHPChanged;
        }

        //STATS
        playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

        //LOCK ON
        playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
        playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;

        //EQUIPMENT
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
        playerNetworkManager.isBlocking.OnValueChanged += playerNetworkManager.OnIsBlockingOnChanged;
        playerNetworkManager.currentQuickSlotItemID.OnValueChanged += playerNetworkManager.OnCurrentQuickSlotItemIDChange;
        playerNetworkManager.isChugging.OnValueChanged += playerNetworkManager.OnIsChuggingChanged;
        int weaponID = playerNetworkManager.currentWeaponBeingUsed.Value;
        playerNetworkManager.OnCurrentWeaponBeingUsedIDChange(weaponID, weaponID);

        //FLAGS
        playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;
        


        //for clients, reload character data for newly instantiated characters
        if (IsOwner && !IsServer)
        {
            LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
        }

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

        if (IsOwner)
        {
            //update health and stamina when vitality/endurance stats changes
            playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;


            //updates ui stat bar when health/stamina changes
            playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
            playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;

        }

        if (!IsOwner)
        {
            characterNetworkManager.currentHealth.OnValueChanged -= characterUIManager.OnHPChanged;
        }

        //STATS
        playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;

        //LOCK ON
        playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
        playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;

        //EQUIPMENT
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
        playerNetworkManager.currentQuickSlotItemID.OnValueChanged -= playerNetworkManager.OnCurrentQuickSlotItemIDChange;
        playerNetworkManager.isChugging.OnValueChanged -= playerNetworkManager.OnIsChuggingChanged;
        playerNetworkManager.isBlocking.OnValueChanged -= playerNetworkManager.OnIsBlockingOnChanged;

        //FLAGS
        playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        WorldGameSessionManager.instance.AddPlayerToActivePlayerList(this);

        if (!IsServer && IsOwner)
        {
            foreach (var player in WorldGameSessionManager.instance.players)
            {
                if(player != this)
                {
                    player.LoadOtherPlayerCharacterWhenJoiningServer();
                }
            }
        }
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if(IsOwner)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendYouDiePopUp();    
        }    

        //check for players that are alive, if 0 then respawn


        return base.ProcessDeathEvent(manuallySelectDeathAnimation);



    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if(IsOwner)
        {

            isDead.Value = false;

            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
            //restore mana

            //play rebirth effects
            playerAnimatorManager.PlayTargetActionAnimation("Empty", true);
        }
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
        currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;

        currentCharacterData.vitality = playerNetworkManager.vitality.Value;
        currentCharacterData.endurance = playerNetworkManager.endurance.Value;




        if (playerInventoryManager.rightHandWeaponIndex <= 0)
        {
        }
        else
        {
            playerNetworkManager.currentRightHandWeaponID.Value = 0;
        }

        currentCharacterData.rightHandWeaponIndex = playerInventoryManager.rightHandWeaponIndex;
        currentCharacterData.rightWeapon01 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInRightHandSlots[0]);
        currentCharacterData.rightWeapon02 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInRightHandSlots[1]);
        currentCharacterData.rightWeapon03 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInRightHandSlots[2]);

        currentCharacterData.leftHandWeaponIndex = playerInventoryManager.leftHandWeaponIndex;
        currentCharacterData.leftWeapon01 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInLeftHandSlots[0]);
        currentCharacterData.leftWeapon02 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInLeftHandSlots[1]);
        currentCharacterData.leftWeapon03 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInLeftHandSlots[2]);



        currentCharacterData.itemsInInventory = playerInventoryManager.itemsInInventory;


    }

    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        playerNetworkManager.characterName.Value = currentCharacterData.characterName;


        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;

        playerNetworkManager.vitality.Value = currentCharacterData.vitality;
        playerNetworkManager.endurance.Value = currentCharacterData.endurance;

        playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnLevel(playerNetworkManager.vitality.Value);
        playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnLevel(playerNetworkManager.endurance.Value);
        playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
        playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
        //PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);

        playerInventoryManager.rightHandWeaponIndex = currentCharacterData.rightHandWeaponIndex;
        playerInventoryManager.weaponsInRightHandSlots[0] = currentCharacterData.rightWeapon01.GetWeapon();
        playerInventoryManager.weaponsInRightHandSlots[1] = currentCharacterData.rightWeapon02.GetWeapon();
        playerInventoryManager.weaponsInRightHandSlots[2] = currentCharacterData.rightWeapon03.GetWeapon();

        playerInventoryManager.leftHandWeaponIndex = currentCharacterData.leftHandWeaponIndex;
        playerInventoryManager.weaponsInLeftHandSlots[0] = currentCharacterData.leftWeapon01.GetWeapon();
        playerInventoryManager.weaponsInLeftHandSlots[1] = currentCharacterData.leftWeapon02.GetWeapon();
        playerInventoryManager.weaponsInLeftHandSlots[2] = currentCharacterData.leftWeapon03.GetWeapon();



        //if (WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.rightWeapon01))
        //{
        //    WeaponItem rightweapon01 = Instantiate(WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.rightWeapon01));
        //    playerInventoryManager.weaponsInRightHandSlots[0] = rightweapon01;
        //}
        //else
        //{
        //    playerInventoryManager.weaponsInRightHandSlots[0] = null;
        //}

        //if (WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.rightWeapon02))
        //{
        //    WeaponItem rightweapon02 = Instantiate(WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.rightWeapon02));
        //    playerInventoryManager.weaponsInRightHandSlots[1] = rightweapon02;
        //}
        //else
        //{
        //    playerInventoryManager.weaponsInRightHandSlots[1] = null;
        //}

        //if (WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.rightWeapon03))
        //{

        //    WeaponItem rightweapon03 = Instantiate(WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.rightWeapon03));
        //    playerInventoryManager.weaponsInRightHandSlots[2] = rightweapon03;
        //}
        //else
        //{
        //    playerInventoryManager.weaponsInRightHandSlots[2] = null;
        //}



        //if (WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.leftWeapon01))
        //{
        //    WeaponItem leftweapon01 = Instantiate(WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.leftWeapon01));
        //    playerInventoryManager.weaponsInLeftHandSlots[0] = leftweapon01;
        //}
        //else
        //{
        //    playerInventoryManager.weaponsInLeftHandSlots[0] = null;
        //}


        //if (WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.leftWeapon02))
        //{
        //    WeaponItem leftweapon02 = Instantiate(WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.leftWeapon02));
        //    playerInventoryManager.weaponsInLeftHandSlots[1] = leftweapon02;
        //}
        //else
        //{
        //    playerInventoryManager.weaponsInLeftHandSlots[1] = null;
        //}


        //if (WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.leftWeapon03))
        //{
        //    WeaponItem leftweapon03 = Instantiate(WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.leftWeapon02));
        //    playerInventoryManager.weaponsInLeftHandSlots[2] = leftweapon03;

        //}
        //else
        //{
        //    playerInventoryManager.weaponsInLeftHandSlots[2] = null;
        //}




        if (currentCharacterData.rightHandWeaponIndex >= 0)
        {
            playerInventoryManager.currentRightHandWeapon = playerInventoryManager.weaponsInRightHandSlots[currentCharacterData.rightHandWeaponIndex];
            playerNetworkManager.currentRightHandWeaponID.Value = playerInventoryManager.weaponsInRightHandSlots[currentCharacterData.rightHandWeaponIndex].itemID;


            //playerInventoryManager.rightHandWeaponIndex = currentCharacterData.rightHandWeaponIndex;
        }
        else
        {
            playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;

            //playerInventoryManager.rightHandWeaponIndex = -1;
            //playerNetworkManager.currentRightHandWeaponID.Value = 0;

        }


        if (currentCharacterData.leftHandWeaponIndex >= 0)
        {
            playerInventoryManager.currentLeftHandWeapon = playerInventoryManager.weaponsInLeftHandSlots[currentCharacterData.leftHandWeaponIndex];
            playerNetworkManager.currentLeftHandWeaponID.Value = playerInventoryManager.weaponsInLeftHandSlots[currentCharacterData.leftHandWeaponIndex].itemID;

            //playerInventoryManager.leftHandWeaponIndex = currentCharacterData.leftHandWeaponIndex;
        }
        else
        {
            playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;

            //playerInventoryManager.leftHandWeaponIndex = -1;
            //playerNetworkManager.currentLeftHandWeaponID.Value = 0;
        }


        playerInventoryManager.itemsInInventory = currentCharacterData.itemsInInventory;
    }

    public void LoadOtherPlayerCharacterWhenJoiningServer()
    {
        //sync weapons
        playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
        playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);

        //sync block status
        playerNetworkManager.OnIsBlockingOnChanged(false, playerNetworkManager.isBlocking.Value);

        //armor

        //lock on
        if(playerNetworkManager.isLockedOn.Value)
        {
            playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
        }
    }

    public virtual void EnableIsPerformingAction()
    {
        playerNetworkManager.isJumping.Value = true;
        isPerformingAction = true;
        applyRootMotion = true;
        canRotate = false;
        canMove = false;
    }

    public virtual void EnableFootIK()
    {
        if (footIK != null)
        {
            footIK.enableBodyPositioning = true;
        }
    }

    public virtual void DisableFootIK()
    {
        if (footIK != null)
        {
            footIK.enableBodyPositioning = false;
        }
    }

    private void DebugMenu()
    {
        if(respawnCharacter && isDead.Value)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }
        else
        {
            respawnCharacter = false; 
        }

    }


}
