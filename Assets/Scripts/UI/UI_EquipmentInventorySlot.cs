using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UI_EquipmentInventorySlot : MonoBehaviour
{
    public Image itemIcon;
    public Image highlightedIcon;
    [SerializeField] public Item currentItem;

    public void AddItem(Item item)
    {
        if(item == null)
        {
            itemIcon.enabled = false;
            return;
        }

        itemIcon.enabled = true;

        currentItem = item;
        itemIcon.sprite = item.itemIcon;
    }

    public void SelectSlot()
    {
        highlightedIcon.enabled = true;
    }

    public void DeselectSlot()
    {
        highlightedIcon.enabled = false;
    }

    public void EquipItem()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        Item equippedItem;

        switch (PlayerUIManager.instance.playerUIEquipmentManager.currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:

                //if current weapon in this slot is not an unarmed item, add it to the inventory
                equippedItem = player.playerInventoryManager.weaponsInRightHandSlots[0];
                
                if(equippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }
                //replace the weapon in that slot with our new weapon
                player.playerInventoryManager.weaponsInRightHandSlots[0] = this.currentItem as WeaponItem;
                //remove the new weapon from our inventory
                player.playerInventoryManager.RemoveItemFromInventory(this.currentItem);
                //re-equip new weapons if we are holding the current weapon in this slot
                if(player.playerInventoryManager.rightHandWeaponIndex == 0)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = this.currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            case EquipmentType.RightWeapon02:

                //if current weapon in this slot is not an unarmed item, add it to the inventory
                equippedItem = player.playerInventoryManager.weaponsInRightHandSlots[1];

                if (equippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }
                //replace the weapon in that slot with our new weapon
                player.playerInventoryManager.weaponsInRightHandSlots[1] = this.currentItem as WeaponItem;
                //remove the new weapon from our inventory
                player.playerInventoryManager.RemoveItemFromInventory(this.currentItem);
                //re-equip new weapons if we are holding the current weapon in this slot
                if (player.playerInventoryManager.rightHandWeaponIndex == 1)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = this.currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            case EquipmentType.RightWeapon03:

                //if current weapon in this slot is not an unarmed item, add it to the inventory
                equippedItem = player.playerInventoryManager.weaponsInRightHandSlots[2];

                if (equippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }
                //replace the weapon in that slot with our new weapon
                player.playerInventoryManager.weaponsInRightHandSlots[2] = this.currentItem as WeaponItem;
                //remove the new weapon from our inventory
                player.playerInventoryManager.RemoveItemFromInventory(this.currentItem);
                //re-equip new weapons if we are holding the current weapon in this slot
                if (player.playerInventoryManager.rightHandWeaponIndex == 2)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = this.currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            case EquipmentType.LeftWeapon01:

                //if current weapon in this slot is not an unarmed item, add it to the inventory
                equippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[0];

                if (equippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }
                //replace the weapon in that slot with our new weapon
                player.playerInventoryManager.weaponsInLeftHandSlots[0] = this.currentItem as WeaponItem;
                //remove the new weapon from our inventory
                player.playerInventoryManager.RemoveItemFromInventory(this.currentItem);
                //re-equip new weapons if we are holding the current weapon in this slot
                if (player.playerInventoryManager.leftHandWeaponIndex == 0)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = this.currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            case EquipmentType.LeftWeapon02:

                //if current weapon in this slot is not an unarmed item, add it to the inventory
                equippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[1];

                if (equippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }
                //replace the weapon in that slot with our new weapon
                player.playerInventoryManager.weaponsInLeftHandSlots[1] = this.currentItem as WeaponItem;
                //remove the new weapon from our inventory
                player.playerInventoryManager.RemoveItemFromInventory(this.currentItem);
                //re-equip new weapons if we are holding the current weapon in this slot
                if (player.playerInventoryManager.leftHandWeaponIndex == 1)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = this.currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            case EquipmentType.LeftWeapon03:

                //if current weapon in this slot is not an unarmed item, add it to the inventory
                equippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[2];

                if (equippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }
                //replace the weapon in that slot with our new weapon
                player.playerInventoryManager.weaponsInLeftHandSlots[2] = this.currentItem as WeaponItem;
                //remove the new weapon from our inventory
                player.playerInventoryManager.RemoveItemFromInventory(this.currentItem);
                //re-equip new weapons if we are holding the current weapon in this slot
                if (player.playerInventoryManager.leftHandWeaponIndex == 2)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = this.currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            case EquipmentType.QuickSlot01:

                equippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[0];

                if (equippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }

                player.playerInventoryManager.quickSlotItemsInQuickSlots[0] = currentItem as QuickSlotItem;
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                if(player.playerInventoryManager.quickSlotItemIndex == 0)
                {
                    player.playerNetworkManager.currentQuickSlotItemID.Value = currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            case EquipmentType.QuickSlot02:

                equippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[1];

                if (equippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }

                player.playerInventoryManager.quickSlotItemsInQuickSlots[1] = currentItem as QuickSlotItem;
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                if (player.playerInventoryManager.quickSlotItemIndex == 1)
                {
                    player.playerNetworkManager.currentQuickSlotItemID.Value = currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            case EquipmentType.QuickSlot03:

                equippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[2];

                if (equippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equippedItem);
                }

                player.playerInventoryManager.quickSlotItemsInQuickSlots[2] = currentItem as QuickSlotItem;
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                if (player.playerInventoryManager.quickSlotItemIndex == 2)
                {
                    player.playerNetworkManager.currentQuickSlotItemID.Value = currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();

                break;
            default:
                break;
        }

        //PlayerUIManager.instance.playerUIEquipmentManager.SelectLastSelectedEquipmentSlot();
    }    
}
