using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public MeleeWeaponDamageCollider meleeWeaponDamageCollider;
    public PlayerNetworkManager player;
    public ParticleSystem weaponTrailObject;
    private ParticleSystem.EmissionModule weaponTrail;
    private void Awake()
    {
        player = FindAnyObjectByType<PlayerNetworkManager>();
        meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        weaponTrail = weaponTrailObject.emission;
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
    {
        meleeWeaponDamageCollider.characterCausingDamage = characterWieldingWeapon;
        meleeWeaponDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeWeaponDamageCollider.magicDamage = weapon.magicDamage;
        meleeWeaponDamageCollider.poiseDamage = weapon.poiseDamage;


        meleeWeaponDamageCollider.light_Attack_01_Modifier = weapon.lightAttackModifer01;
        meleeWeaponDamageCollider.light_Attack_02_Modifier = weapon.lightAttackModifer02;
        meleeWeaponDamageCollider.light_Attack_03_Modifier = weapon.lightAttackModifer03;
        meleeWeaponDamageCollider.light_Attack_04_Modifier = weapon.lightAttackModifer04;

        meleeWeaponDamageCollider.heavy_Attack_01_Modifier = weapon.heavyAttackModifer01;
        meleeWeaponDamageCollider.heavy_Attack_01_Modifier = weapon.heavyAttackModifer02;
        meleeWeaponDamageCollider.heavy_Attack_01_Modifier = weapon.heavyAttackModifer03;

        meleeWeaponDamageCollider.charged_Attack_01_Modifier = weapon.chargedAttackModifer01;
        meleeWeaponDamageCollider.charged_Attack_01_Modifier = weapon.chargedAttackModifer02;
        meleeWeaponDamageCollider.charged_Attack_01_Modifier = weapon.chargedAttackModifer03;

        meleeWeaponDamageCollider.running_Attack_01_Modifier = weapon.runningAttackModifer01;
        meleeWeaponDamageCollider.rolling_Attack_01_Modifier = weapon.rollingAttackModifer01;
        meleeWeaponDamageCollider.backstep_Attack_01_Modifier = weapon.backstepAttackModifer01;
    }

    private void Update()
    {
        if(weaponTrailObject != null) 
        {
            //if (player.isAttacking.Value)
            //{
            //    weaponTrail.rateOverTime = 40;
            //}

            //else
            //{
            //    weaponTrail.rateOverTime = 0;
            //}

            if (meleeWeaponDamageCollider.damageCollider.enabled)
            {
                weaponTrail.rateOverTime = 40;
            }
            else
            {
                weaponTrail.rateOverTime = 0;
            }
        }
    }

}
