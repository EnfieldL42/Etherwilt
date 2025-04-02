using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] MeleeWeaponDamageCollider meleeWeaponDamageCollider;
    private void Awake()
    {
        meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
    {
        meleeWeaponDamageCollider.characterCausingDamage = characterWieldingWeapon;
        meleeWeaponDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeWeaponDamageCollider.magicDamage = weapon.magicDamage;
    }
}
