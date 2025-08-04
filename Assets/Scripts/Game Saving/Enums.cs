using UnityEngine;

public class Enums : MonoBehaviour
{

}


public enum CharacterSlot
{
    CharacterSlot_01,
    CharacterSlot_02,
    CharacterSlot_03,
    CharacterSlot_04,
    CharacterSlot_05,
    NO_SLOT,
}

public enum CharacterGroup
{
    Team01,
    Team02,
}

public enum WeaponModelSlot
{
    RightHandWeaponSlot,
    LeftHandWeaponSlot,
    LeftHandShieldSlot,
}

public enum WeaponModelType
{
    Weapon,
    Shield,
}

public enum EquipmentType
{
    RightWeapon01, //0
    RightWeapon02, //1
    RightWeapon03, //2
    LeftWeapon01,  //3
    LeftWeapon02,  //4
    LeftWeapon03,  //5
}

public enum AttackType
{
    MeleeAttack01,
    LightAttack01,
    LightAttack02,
    LightAttack03,
    LightAttack04,
    HeavyAttack01,
    HeavyAttack02,
    HeavyAttack03,
    ChargedAttack01,
    ChargedAttack02,
    ChargedAttack03,
    RunningAttack01,
    RollingAttack01,
    BackstepAttack01,
}

public enum DamageIntensity
{
    Normal,
} //Damage animation intensity

public enum ItemPickUpType
{
    WorldSpawn,
    CharacterDrop,
}



