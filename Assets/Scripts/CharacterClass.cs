using UnityEngine;


[System.Serializable]
public class CharacterClass
{
    [Header("Class Information")]
    public string className;

    [Header("Class Stats")]
    public int vitality = 10;
    public int endurance = 10;
    public int strength = 10;
    public int dexterity = 10;
    public int weaponMastery = 10;
    public int magicMastery = 10;
    public int breakerMastery = 10;
    public int tankMastery = 10;

    [Header("Class Weapons")]
    public WeaponItem[] mainHandWeapons = new WeaponItem[3];
    public WeaponItem[] offHandWeapons = new WeaponItem[3];

    [Header("Quick Slot Items")]
    public QuickSlotItem[] quickSlotItems = new QuickSlotItem[3];

    public void SetClass(PlayerManager player)
    {
        TitleScreenManager.instance.SetCharacterClass(player, vitality, endurance, strength, dexterity, weaponMastery, magicMastery, breakerMastery, tankMastery, mainHandWeapons, offHandWeapons, quickSlotItems);
    }
}
