using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;

public class PlayerUIEquipmentManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    [Header("Weapon Slots")]
    [SerializeField] Image rightHandSlot01;
    private Button rightHandSlot01Button;
    [SerializeField] Image rightHandSlot02;
    private Button rightHandSlot02Button;
    [SerializeField] Image rightHandSlot03;
    private Button rightHandSlot03Button;
    [SerializeField] Image leftHandSlot01;
    private Button leftHandSlot01Button;
    [SerializeField] Image leftHandSlot02;
    private Button leftHandSlot02Button;
    [SerializeField] Image leftHandSlot03;
    private Button leftHandSlot03Button;

    [Header("Equipment Inventory")]
    public EquipmentType currentSelectedEquipmentSlot;
    [SerializeField] GameObject equipmentInventoryWindow;
    [SerializeField] GameObject equipmentInventorySlotPrefab;
    [SerializeField] Transform equipmentInventoryContentWindow;
    [SerializeField] Item currentSelectedItem;

    [Header("Quick Slots")]
    [SerializeField] Image quickSlot01EquipmentSlot;
    [SerializeField] TextMeshProUGUI quickSlot01Count;
    private Button quickSlot01Button;

    [SerializeField] Image quickSlot02EquipmentSlot;
    [SerializeField] TextMeshProUGUI quickSlot02Count;
    private Button quickSlot02Button;

    [SerializeField] Image quickSlot03EquipmentSlot;
    [SerializeField] TextMeshProUGUI quickSlot03Count;
    private Button quickSlot03Button;


    [Header("Equipment Slots Buttons")]
    [SerializeField] Button[] equipmentSlotButtons;


    private void Awake()
    {
        rightHandSlot01Button = rightHandSlot01.GetComponentInParent<Button>(true);
        rightHandSlot02Button = rightHandSlot02.GetComponentInParent<Button>(true);
        rightHandSlot03Button = rightHandSlot03.GetComponentInParent<Button>(true);

        leftHandSlot01Button = leftHandSlot01.GetComponentInParent<Button>(true);
        leftHandSlot02Button = leftHandSlot02.GetComponentInParent<Button>(true);
        leftHandSlot03Button = leftHandSlot03.GetComponentInParent<Button>(true);

        quickSlot01Button = quickSlot01EquipmentSlot.GetComponentInParent<Button>(true);
        quickSlot02Button = quickSlot02EquipmentSlot.GetComponentInParent<Button>(true);
        quickSlot03Button = quickSlot03EquipmentSlot.GetComponentInParent<Button>(true);
    }

    public void OpenEquipmentManagerMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        ToggleEquipmentButtons(true);
        menu.SetActive(true);
        equipmentInventoryWindow.SetActive(false);
        ClearEquipmentInventory();
        RefreshEquipmentSlotIcons();
    }

    public void RefreshMenu()
    {
        //EnableEquipmentButtons();
        ClearEquipmentInventory();
        RefreshEquipmentSlotIcons();
        PlayerUIManager.instance.playerUIEquipmentManager.SelectLastSelectedEquipmentSlot();
    }

    private void ToggleEquipmentButtons(bool isEnabled)
    {
        rightHandSlot01Button.enabled = isEnabled;
        rightHandSlot02Button.enabled = isEnabled;
        rightHandSlot03Button.enabled = isEnabled;

        leftHandSlot01Button.enabled= isEnabled;
        leftHandSlot02Button.enabled= isEnabled;
        leftHandSlot03Button.enabled= isEnabled;

        quickSlot01Button.enabled= isEnabled;
        quickSlot02Button.enabled= isEnabled;
        quickSlot03Button.enabled= isEnabled;
    }

    public void SelectLastSelectedEquipmentSlot()
    {
        Button lastSelectedButton = null;

        ToggleEquipmentButtons(true);

        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                lastSelectedButton = rightHandSlot01Button;
                break;
            case EquipmentType.RightWeapon02:
                lastSelectedButton = rightHandSlot02Button;
                break;
            case EquipmentType.RightWeapon03:
                lastSelectedButton = rightHandSlot03Button;
                break;
            case EquipmentType.LeftWeapon01:
                lastSelectedButton = leftHandSlot01Button;
                break;
            case EquipmentType.LeftWeapon02:
                lastSelectedButton = leftHandSlot02Button;
                break;
            case EquipmentType.LeftWeapon03:
                lastSelectedButton = leftHandSlot03Button;
                break;
            case EquipmentType.QuickSlot01:
                lastSelectedButton = quickSlot01Button;
                break;
            case EquipmentType.QuickSlot02:
                lastSelectedButton = quickSlot02Button;
                break;
            case EquipmentType.QuickSlot03:
                lastSelectedButton = quickSlot03Button;
                break;
            default:
                break;
        }

        if (lastSelectedButton != null)
        {
            lastSelectedButton.Select();
            lastSelectedButton.OnSelect(null);
        }

        equipmentInventoryWindow.SetActive(false);
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
            rightHandSlot01.enabled = true;
            rightHandSlot01.sprite = rightHandWeapon01.itemIcon;
        }
        else
        {
            rightHandSlot01.enabled = false;

        }

        //Right weapon slot 2
        WeaponItem rightHandWeapon02 = player.playerInventoryManager.weaponsInRightHandSlots[1];

        if (rightHandWeapon02.itemIcon != null)
        {
            rightHandSlot02.enabled = true;
            rightHandSlot02.sprite = rightHandWeapon02.itemIcon;
        }
        else
        {
            rightHandSlot02.enabled = false;

        }

        //Right weapon slot 3
        WeaponItem rightHandWeapon03 = player.playerInventoryManager.weaponsInRightHandSlots[2];

        if (rightHandWeapon03.itemIcon != null)
        {
            rightHandSlot03.enabled = true;
            rightHandSlot03.sprite = rightHandWeapon03.itemIcon;
        }
        else
        {
            rightHandSlot03.enabled = false;

        }

        //Left weapon slot 1
        WeaponItem leftHandWeapon01 = player.playerInventoryManager.weaponsInLeftHandSlots[0];

        if (leftHandWeapon01.itemIcon != null)
        {
            leftHandSlot01.enabled = true;
            leftHandSlot01.sprite = leftHandWeapon01.itemIcon;
        }
        else
        {
            leftHandSlot01.enabled = false;

        }

        //Left weapon slot 2
        WeaponItem leftHandWeapon02 = player.playerInventoryManager.weaponsInLeftHandSlots[1];

        if (leftHandWeapon02.itemIcon != null)
        {
            leftHandSlot02.enabled = true;
            leftHandSlot02.sprite = leftHandWeapon02.itemIcon;
        }
        else
        {
            leftHandSlot02.enabled = false;

        }

        //Left weapon slot 3
        WeaponItem leftHandWeapon03 = player.playerInventoryManager.weaponsInLeftHandSlots[2];

        if (leftHandWeapon03.itemIcon != null)
        {
            leftHandSlot03.enabled = true;
            leftHandSlot03.sprite = leftHandWeapon03.itemIcon;
        }
        else
        {
            leftHandSlot03.enabled = false;

        }

        //Quick Slot 1
        QuickSlotItem quickSlotEquipment01 = player.playerInventoryManager.quickSlotItemsInQuickSlots[0];

        if (quickSlotEquipment01 != null)
        {
            quickSlot01EquipmentSlot.enabled = true;
            quickSlot01EquipmentSlot.sprite = quickSlotEquipment01.itemIcon;

            if(quickSlotEquipment01.isConsumable)
            {
                quickSlot01Count.enabled = true;
                quickSlot01Count.text = quickSlotEquipment01.GetCurrentAmount(player).ToString();
            }
            else
            {
                quickSlot01Count.enabled = false;
            }
        }
        else
        {
            quickSlot01EquipmentSlot.enabled = false;
            quickSlot01Count.enabled = false;

        }

        //Quick Slot 2
        QuickSlotItem quickSlotEquipment02 = player.playerInventoryManager.quickSlotItemsInQuickSlots[1];

        if (quickSlotEquipment02 != null)
        {
            quickSlot02EquipmentSlot.enabled = true;
            quickSlot02EquipmentSlot.sprite = quickSlotEquipment02.itemIcon;


            if (quickSlotEquipment02.isConsumable)
            {
                quickSlot02Count.enabled = true;
                quickSlot02Count.text = quickSlotEquipment02.GetCurrentAmount(player).ToString();
            }
            else
            {
                quickSlot02Count.enabled = false;
            }
        }
        else
        {
            quickSlot02EquipmentSlot.enabled = false;
            quickSlot02Count.enabled = false;

        }

        //Quick Slot 3
        QuickSlotItem quickSlotEquipment03 = player.playerInventoryManager.quickSlotItemsInQuickSlots[2];

        if (quickSlotEquipment03 != null)
        {
            quickSlot03EquipmentSlot.enabled = true;
            quickSlot03EquipmentSlot.sprite = quickSlotEquipment03.itemIcon;


            if (quickSlotEquipment03.isConsumable)
            {
                quickSlot03Count.enabled = true;
                quickSlot03Count.text = quickSlotEquipment03.GetCurrentAmount(player).ToString();
            }
            else
            {
                quickSlot03Count.enabled = false;
            }
        }
        else
        {
            quickSlot03EquipmentSlot.enabled = false;
            quickSlot03Count.enabled = false;

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
            case EquipmentType.QuickSlot01:
                LoadQuickSlotInventory();
                break;
            case EquipmentType.QuickSlot02:
                LoadQuickSlotInventory();
                break;
            case EquipmentType.QuickSlot03:
                LoadQuickSlotInventory();
                break;
            default:
                break;
        }
    }

    private void LoadQuickSlotInventory()
    {
        //DisableEquipmentButtons();

        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        List<QuickSlotItem> itemsInInventory = new List<QuickSlotItem>();


        for (int i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            QuickSlotItem item = player.playerInventoryManager.itemsInInventory[i] as QuickSlotItem;

            if (item != null)
            {
                itemsInInventory.Add(item);
            }
        }

        if (itemsInInventory.Count <= 0)
        {
            equipmentInventoryWindow.SetActive(false);
            ToggleEquipmentButtons(true);
            RefreshMenu();
            //PlayerUIManager.instance.playerUIEquipmentManager.SelectLastSelectedEquipmentSlot();
            return;
        }

        bool hasSelectedFirstInventorySlot = false;

        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
                UI_EquipmentInventorySlot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlot>();
                equipmentInventorySlot.AddItem(itemsInInventory[i]);

                if (!hasSelectedFirstInventorySlot)
                {
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }
    }

    private void LoadWeaponInventory()
    {
        //DisableEquipmentButtons();
        ToggleEquipmentButtons(false);

        Debug.Log("got here");

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
            equipmentInventoryWindow.SetActive(false);
            ToggleEquipmentButtons(true);
            RefreshMenu();
            //PlayerUIManager.instance.playerUIEquipmentManager.SelectLastSelectedEquipmentSlot();
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
            case EquipmentType.QuickSlot01:

                unequipedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[0];
                if (unequipedItem != null)
                {
                    player.playerInventoryManager.quickSlotItemsInQuickSlots[0] = null;

                    if (unequipedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                player.playerInventoryManager.quickSlotItemsInQuickSlots[0] = null;

                if(player.playerInventoryManager.quickSlotItemIndex == 0)
                {
                    player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
                }

                break;
            case EquipmentType.QuickSlot02:

                unequipedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[1];
                if (unequipedItem != null)
                {
                    player.playerInventoryManager.quickSlotItemsInQuickSlots[1] = null;

                    if (unequipedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                player.playerInventoryManager.quickSlotItemsInQuickSlots[1] = null;

                if (player.playerInventoryManager.quickSlotItemIndex == 1)
                {
                    player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
                }

                break;

            case EquipmentType.QuickSlot03:

                unequipedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[2];
                if (unequipedItem != null)
                {
                    player.playerInventoryManager.quickSlotItemsInQuickSlots[2] = null;

                    if (unequipedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequipedItem);
                    }
                }

                player.playerInventoryManager.quickSlotItemsInQuickSlots[2] = null;

                if (player.playerInventoryManager.quickSlotItemIndex == 2)
                {
                    player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
                }

                break;
            default:
                break;
        }

        //refreshes screen
        RefreshMenu();
    }

    public void EnableEquipmentButtons()
    {
        foreach (Button button in equipmentSlotButtons)
        {
            button.interactable = true;
        }
    }

    public void DisableEquipmentButtons()
    {
        foreach (Button button in equipmentSlotButtons)
        {
            button.interactable = false;
        }
    }


}
