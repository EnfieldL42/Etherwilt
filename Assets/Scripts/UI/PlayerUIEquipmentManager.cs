using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

public class PlayerUIEquipmentManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    [Header("Weapon Slots")]
    [SerializeField] Image rightHandSlot1;
    [SerializeField] Image rightHandSlot2;
    [SerializeField] Image rightHandSlot3;
    [SerializeField] Image leftHandSlot1;
    [SerializeField] Image leftHandSlot2;
    [SerializeField] Image leftHandSlot3;

    [Header("Equipment Inventory")]
    public EquipmentType currentSelectedEquipmentSlot;
    [SerializeField] GameObject equipmentInventoryWindow;
    [SerializeField] GameObject equipmentInventorySlotPrefab;
    [SerializeField] Transform equipmentInventoryContentWindow;
    [SerializeField] Item currentSelectedItem;


    public void OpenEquipmentManagerMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        menu.SetActive(true);
        equipmentInventoryWindow.SetActive(false);
        ClearEquipmentInventory();
        RefreshEquipmentSlotIcons();
    }

    public void RefreshMenu()
    {
        ClearEquipmentInventory();
        RefreshEquipmentSlotIcons();
    }

    public void SelectLastSelectedEquipmentSlot()
    {
        Button lastSelectedButton = null;

        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                lastSelectedButton = rightHandSlot1.GetComponentInParent<Button>();
                break;
            case EquipmentType.RightWeapon02:
                lastSelectedButton = rightHandSlot2.GetComponentInParent<Button>();
                break;
            case EquipmentType.RightWeapon03:
                lastSelectedButton = rightHandSlot3.GetComponentInParent<Button>();
                break;
            case EquipmentType.LeftWeapon01:
                lastSelectedButton = leftHandSlot1.GetComponentInParent<Button>();
                break;
            case EquipmentType.LeftWeapon02:
                lastSelectedButton = leftHandSlot2.GetComponentInParent<Button>();
                break;
            case EquipmentType.LeftWeapon03:
                lastSelectedButton = leftHandSlot3.GetComponentInParent<Button>();
                break;
            default:
                break;
        }

        if (lastSelectedButton != null)
        {
            lastSelectedButton.Select();
            lastSelectedButton.OnSelect(null);
        }
    }

    public void CloseEquipmentManagerMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }

    private void RefreshEquipmentSlotIcons()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        WeaponItem rightHandWeapon01 = player.playerInventoryManager.weaponsInRightHandSlots[0];

        //Right weapon slot 1
        if(rightHandWeapon01.itemIcon != null)
        {
            rightHandSlot1.enabled = true;
            rightHandSlot1.sprite = rightHandWeapon01.itemIcon;
        }
        else
        {
            rightHandSlot1.enabled = false;

        }

        //Right weapon slot 2
        WeaponItem rightHandWeapon02 = player.playerInventoryManager.weaponsInRightHandSlots[1];

        if (rightHandWeapon02.itemIcon != null)
        {
            rightHandSlot2.enabled = true;
            rightHandSlot2.sprite = rightHandWeapon02.itemIcon;
        }
        else
        {
            rightHandSlot2.enabled = false;

        }

        //Right weapon slot 3
        WeaponItem rightHandWeapon03 = player.playerInventoryManager.weaponsInRightHandSlots[2];

        if (rightHandWeapon03.itemIcon != null)
        {
            rightHandSlot3.enabled = true;
            rightHandSlot3.sprite = rightHandWeapon03.itemIcon;
        }
        else
        {
            rightHandSlot3.enabled = false;

        }

        //Left weapon slot 1
        WeaponItem leftHandWeapon01 = player.playerInventoryManager.weaponsInLeftHandSlots[0];

        if (leftHandWeapon01.itemIcon != null)
        {
            leftHandSlot1.enabled = true;
            leftHandSlot1.sprite = leftHandWeapon01.itemIcon;
        }
        else
        {
            leftHandSlot1.enabled = false;

        }

        //Left weapon slot 2
        WeaponItem leftHandWeapon02 = player.playerInventoryManager.weaponsInLeftHandSlots[1];

        if (leftHandWeapon02.itemIcon != null)
        {
            leftHandSlot2.enabled = true;
            leftHandSlot2.sprite = leftHandWeapon02.itemIcon;
        }
        else
        {
            leftHandSlot2.enabled = false;

        }

        //Left weapon slot 3
        WeaponItem leftHandWeapon03 = player.playerInventoryManager.weaponsInLeftHandSlots[2];

        if (leftHandWeapon03.itemIcon != null)
        {
            leftHandSlot3.enabled = true;
            leftHandSlot3.sprite = leftHandWeapon03.itemIcon;
        }
        else
        {
            leftHandSlot3.enabled = false;

        }
    }

    private void ClearEquipmentInventory()
    {
        foreach(Transform item in equipmentInventoryContentWindow)
        {
            Destroy(item.gameObject);
        }

    }

    public void LoadEquipmentInventory()
    {
        equipmentInventoryWindow.SetActive(true);

        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                LoadWeaponInventory();
                break;
            case EquipmentType.RightWeapon02:
                LoadWeaponInventory();
                break;
            case EquipmentType.RightWeapon03:
                LoadWeaponInventory();
                break;
            case EquipmentType.LeftWeapon01:
                LoadWeaponInventory();
                break;
            case EquipmentType.LeftWeapon02:
                LoadWeaponInventory();
                break;
            case EquipmentType.LeftWeapon03:
                LoadWeaponInventory();
                break;
            default:
                break;
        }
    }

    private void LoadWeaponInventory()
    {
        List<WeaponItem> weaponsInInventory = new List<WeaponItem>();

        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        for (int i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            WeaponItem weapon = player.playerInventoryManager.itemsInInventory[i] as WeaponItem;

            if(weapon != null)
            {
                weaponsInInventory.Add(weapon);
            }
        }

        if(weaponsInInventory.Count <= 0)
        {
            RefreshMenu();
            return;
        }

        bool hasSelectedFirstInventorySlot = false;

        for (int i = 0; i < weaponsInInventory.Count; i++)
        {
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
                UI_EquipmentInventorySlot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlot>();
                equipmentInventorySlot.AddItem(weaponsInInventory[i]);

                if(!hasSelectedFirstInventorySlot)
                {
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }
    }

    public void SelectEquipmentSlot(int equipmentSlot)
    {
        currentSelectedEquipmentSlot = (EquipmentType)equipmentSlot;

    }

    public void UnequipSelectedEquip()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        Item unequipedItem;

        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:

                unequipedItem = player.playerInventoryManager.weaponsInRightHandSlots[0];
                if(unequipedItem != null)
                {
                    player.playerInventoryManager.weaponsInRightHandSlots[0] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);

                    if(unequipedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                if(player.playerInventoryManager.rightHandWeaponIndex == 0)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            case EquipmentType.RightWeapon02:

                unequipedItem = player.playerInventoryManager.weaponsInRightHandSlots[1];
                if (unequipedItem != null)
                {
                    player.playerInventoryManager.weaponsInRightHandSlots[1] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);

                    if (unequipedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                if (player.playerInventoryManager.rightHandWeaponIndex == 1)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            case EquipmentType.RightWeapon03:

                unequipedItem = player.playerInventoryManager.weaponsInRightHandSlots[2];
                if (unequipedItem != null)
                {
                    player.playerInventoryManager.weaponsInRightHandSlots[2] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);

                    if (unequipedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                if (player.playerInventoryManager.rightHandWeaponIndex == 2)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            case EquipmentType.LeftWeapon01:

                unequipedItem = player.playerInventoryManager.weaponsInLeftHandSlots[0];
                if (unequipedItem != null)
                {
                    player.playerInventoryManager.weaponsInLeftHandSlots[0] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);

                    if (unequipedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                if (player.playerInventoryManager.leftHandWeaponIndex == 0)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            case EquipmentType.LeftWeapon02:

                unequipedItem = player.playerInventoryManager.weaponsInLeftHandSlots[1];
                if (unequipedItem != null)
                {
                    player.playerInventoryManager.weaponsInLeftHandSlots[1] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);

                    if (unequipedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                if (player.playerInventoryManager.leftHandWeaponIndex == 1)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            case EquipmentType.LeftWeapon03:

                unequipedItem = player.playerInventoryManager.weaponsInLeftHandSlots[2];
                if (unequipedItem != null)
                {
                    player.playerInventoryManager.weaponsInLeftHandSlots[2] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);

                    if (unequipedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                if (player.playerInventoryManager.leftHandWeaponIndex == 2)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            default:
                break;
        }

        //refreshes screen
        RefreshMenu();
    }

}
