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

    [Header("Bonfires")]
    public SerializableDictionary<int, bool> bonfires; //int is the id of the bonfire, bool is whether it is activated or not

    [Header("Bosses")]
    public SerializableDictionary<int, bool> bossesAwakened;
    public SerializableDictionary<int, bool> bossesDefeated;

    [Header("World Items")]
    public SerializableDictionary<int, bool> worldItemsLooted;

    [Header("Inventory")]
    //current weapons
    //public WeaponItem currentRightHandWeapon;
    //public WeaponItem currentLeftHandWeapon;
    //quicklots
    public WeaponItem[] weaponsInRightHandSlots;
    public WeaponItem[] weaponsInLeftHandSlots;
    //public int rightHandWeaponIndex = 0;
    //public int leftHandWeaponIndex = 0;
    //inventory
    public List<Item> itemsInInventory;


    public CharacterSaveData()
    {
        bonfires = new SerializableDictionary<int, bool>();
        bossesAwakened = new SerializableDictionary<int, bool>();
        bossesDefeated = new SerializableDictionary<int, bool>();
        worldItemsLooted = new SerializableDictionary<int, bool>();
    }
}
