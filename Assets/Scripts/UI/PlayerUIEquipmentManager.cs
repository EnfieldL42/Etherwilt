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
        RefreshWeaponSlotIcons();
    }

    public void CloseEquipmentManagerMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }

    private void RefreshWeaponSlotIcons()
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
            OpenEquipmentManagerMenu();
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

}
