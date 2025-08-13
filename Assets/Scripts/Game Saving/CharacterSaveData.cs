using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//reference for every save file, not a monobehavior
public class CharacterSaveData
{
    [Header("Scene Index")]
    public int sceneIndex;

    [Header("Character Name")]
    public string characterName = "Character";

    [Header("Time Played")]
    public float secondsPlayed;

    //cannot use a vector 3 as its not serializable, serializable can only use float int strin and bool
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    [Header("Resources")]
    public int currentHealth;
    public float currentStamina;

    [Header("Stats")]
    public int vitality;
    public int endurance;
    public int strength;
    public int dexterity;

    [Header("Ether")]
    public int ether;

    [Header("Bonfires")]
    public SerializableDictionary<int, bool> bonfires; //int is the id of the bonfire, bool is whether it is activated or not

    [Header("Bosses")]
    public SerializableDictionary<int, bool> bossesAwakened;
    public SerializableDictionary<int, bool> bossesDefeated;

    [Header("World Items")]
    public SerializableDictionary<int, bool> worldItemsLooted;

    [Header("Inventory")]
    //Right Hand
    public int rightHandWeaponIndex;
    public SerializableWeapon rightWeapon01;
    public SerializableWeapon rightWeapon02;
    public SerializableWeapon rightWeapon03;

    //Left Hand
    public int leftHandWeaponIndex;
    public SerializableWeapon leftWeapon01;
    public SerializableWeapon leftWeapon02;
    public SerializableWeapon leftWeapon03;

    public int quickSlotIndex;
    public SerializableQuickSlotItem quickSlotItem01;
    public SerializableQuickSlotItem quickSlotItem02;
    public SerializableQuickSlotItem quickSlotItem03;

    //Health Flasks
    public int remainingHealthFlasks = 3;

    //inventory
    public List<SerializableWeapon> weaponsInInventory;
    public List<SerializableQuickSlotItem> quickSlotItemsInInventory;


    public CharacterSaveData()
    {
        bonfires = new SerializableDictionary<int, bool>();
        bossesAwakened = new SerializableDictionary<int, bool>();
        bossesDefeated = new SerializableDictionary<int, bool>();
        worldItemsLooted = new SerializableDictionary<int, bool>();

        weaponsInInventory = new List<SerializableWeapon>();
        quickSlotItemsInInventory = new List<SerializableQuickSlotItem>();
    }
}
