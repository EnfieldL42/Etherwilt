using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    [Header("Weapons")]
    public WeaponItem currentRightHandWeapon;
    public WeaponItem currentLeftHandWeapon;

    [Header("Quick Slots")]
    public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
    public int rightHandWeaponIndex = 0;
    public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
    public int leftHandWeaponIndex = 0;

    [Header("Inventory")]
    public List<Item> itemsInInventory;

    public void AddItemToInventory(Item item)
    {
        itemsInInventory.Add(item);
    }

    public void  RemoveItemFromInventory(Item item)
    {
        itemsInInventory.Remove(item);

        for(int i = itemsInInventory.Count - 1; i > -1; i--)
        {
            if(itemsInInventory[i] == null)
            {
                itemsInInventory.RemoveAt(i);
            }
        }
    }
}
