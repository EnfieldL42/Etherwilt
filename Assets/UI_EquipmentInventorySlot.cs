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

        switch (PlayerUIManager.instance.playerUIEquipmentManager.currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                //if current weapon in this slot is not an unarmed item, add it to the inventory
                WeaponItem currentWeapon = player.playerInventoryManager.weaponsInRightHandSlots[0];
                
                if(currentWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(currentWeapon);
                }
                //replace the weapon in that slot with our new weapon
                player.playerInventoryManager.weaponsInRightHandSlots[0] = currentItem as WeaponItem;
                //remove the new weapon from our inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);
                //re-equip new weapons if we are holding the current weapon in this slot
                if(player.playerInventoryManager.rightHandWeaponIndex == 0)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;
                }

                //refreshes the equipment menu
                PlayerUIManager.instance.playerUIEquipmentManager.OpenEquipmentManagerMenu();
                break;
            case EquipmentType.RightWeapon02:
                break;
            case EquipmentType.RightWeapon03:
                break;
            case EquipmentType.LeftWeapon01:
                break;
            case EquipmentType.LeftWeapon02:
                break;
            case EquipmentType.LeftWeapon03:
                break;
            default:
                break;
        }
    }    
}
