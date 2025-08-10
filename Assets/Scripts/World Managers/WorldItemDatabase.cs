using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase instance;

    public WeaponItem unarmedWeapon;

    [Header("Weapons")]
    //list of every weapon in the game
    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

    [Header("Items")]
    //list of every item in the game
    [SerializeField] List<Item> items = new List<Item>();

    [Header("Quick Slot")]
    //list of every item in the game
    [SerializeField] List<QuickSlotItem> quickSlotItems = new List<QuickSlotItem>();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        //add all weapons to list of items
        foreach (var weapon in weapons)
        {
            items.Add(weapon);

        }

        foreach (var item in quickSlotItems)
        {
            items.Add(item);
        }

        //assign all of the items a unique item id
        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }




    }

    public WeaponItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
    }

    public QuickSlotItem GetQuickSlotItemByID(int ID)
    {
        return quickSlotItems.FirstOrDefault(item => item.itemID == ID);
    }


    //Item Serialization

    public WeaponItem GetWeaponFromSerializedData(SerializableWeapon serializableWeapon)
    {
        WeaponItem weapon = null;

        if(GetWeaponByID(serializableWeapon.itemID))
        {
            weapon = Instantiate(GetWeaponByID(serializableWeapon.itemID));
        }

        if (weapon == null)
        {
            return Instantiate(unarmedWeapon);
        }

        return weapon;
    }

    public FlaskItem GetFlaskFromSerializedData(FlaskItem serializableFlask)
    {
        FlaskItem flask = null;

        if (GetQuickSlotItemByID(serializableFlask.itemID))
        {
            flask = Instantiate(GetQuickSlotItemByID(serializableFlask.itemID)) as FlaskItem;
        }

        return flask;
    }

    public QuickSlotItem GetQuickSlotItemFromSerializedData(SerializableQuickSlotItem serializableQuickSlotItem)
    {
        QuickSlotItem quickSlotItem = null;

        if(GetQuickSlotItemByID(serializableQuickSlotItem.itemID))
        {
            quickSlotItem = Instantiate(GetQuickSlotItemByID(serializableQuickSlotItem.itemID));
            quickSlotItem.itemAmount = serializableQuickSlotItem.itemAmount;
        }

        return quickSlotItem;

    }



}
