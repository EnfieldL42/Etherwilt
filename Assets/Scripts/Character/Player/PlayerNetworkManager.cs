using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    [Header("Character Name")]
    public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Equipment")]
    public NetworkVariable<int> currentWeaponBeingUsed = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void SetCharacterActionHand(bool rightHandedAction)
    {
        if(rightHandedAction)
        {
            isUsingRightHand.Value = true;
            isUsingLeftHand.Value = false;
        }
        else
        {
            isUsingRightHand.Value = false;
            isUsingLeftHand.Value = true;
        }    
    }

    public void SetNewMaxHealthValue(int oldVitality, int newVitality)
    {
        maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnLevel(newVitality);
        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
        currentHealth.Value = maxHealth.Value;
    }

    public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
    {
        maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnLevel(newEndurance);
        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
        currentStamina.Value = maxStamina.Value;
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadRightWeapon();

        if(player.IsOwner)
        {
            PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon(newID);
        }

    }

    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadLeftWeapon();

        if (player.IsOwner)
        {
            PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon(newID);
        }
    }

    public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        player.playerCombatManager.currentWeaponBeingUsed = newWeapon;
        if(player.IsOwner)
        {
            return;
        }

        if(player.playerCombatManager.currentWeaponBeingUsed != null)
        {
            player.playerAnimatorManager.UpdateAnimatorController(player.playerCombatManager.currentWeaponBeingUsed.weaponAnimator);
        }
    }

    public override void OnIsBlockingOnChanged(bool oldStatus, bool newStatus)
    {
        base.OnIsBlockingOnChanged(oldStatus, newStatus);

        if (IsOwner)
        {
            player.playerStatsManager.blockingPhyicalAbsorption = player.playerCombatManager.currentWeaponBeingUsed.physicalBaseDamageAbsorption;
            player.playerStatsManager.blockingMagicAbsorption = player.playerCombatManager.currentWeaponBeingUsed.magiclBaseDamageAbsorption;
        }
    }

    [ServerRpc]
    public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
    {
        if(IsServer)
        {
            NofifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
        }
    }


    [ClientRpc]
    public void NofifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
    {

        //we do not play the action again for the character who called it as they already have played it locally
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformWeaponBasedAction(actionID, weaponID);
        }
    }

    private void PerformWeaponBasedAction(int actionID, int weaponID)
    {
        WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

        if(weaponAction != null)
        {
            weaponAction.AttemptToPerformAction(player, WorldItemDatabase.instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.Log("ACTION IS NULL, CANNOT BE PERFORMED");
        }
    }
}
