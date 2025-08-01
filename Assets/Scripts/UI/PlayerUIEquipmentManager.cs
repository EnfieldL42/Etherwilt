using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

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

    public void OpenEquipmentManagerMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        menu.SetActive(true);

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

}
