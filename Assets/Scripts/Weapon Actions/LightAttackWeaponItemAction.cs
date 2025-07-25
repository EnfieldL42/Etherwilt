using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [Header("Light Attacks")]
    [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
    [SerializeField] string light_Attack_02 = "Main_Light_Attack_02"; 
    [SerializeField] string light_Attack_03 = "Main_Light_Attack_03"; 
    [SerializeField] string light_Attack_04 = "Main_Light_Attack_04";

    [Header("Running Attacks")]
    [SerializeField] string run_Attack_01 = "Main_Run_Attack_01";

    [Header("Rolling Attacks")]
    [SerializeField] string roll_Attack_01 = "Main_Roll_Attack_01";

    [Header("Backstep Attacks")]
    [SerializeField] string backstep_Attack_01 = "Main_Backstep_Attack_01";
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (!playerPerformingAction.IsOwner)
        {
            return;
        }


        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        //check for stops like stamina

        if(playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }
        if(!playerPerformingAction.isGrounded)
        {
            return;
        }
        //if sprinting, perform running attack
        if (playerPerformingAction.characterNetworkManager.isSprinting.Value)
        {
            PerformRunningtAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }
        //if rolling, perform rolling attack
        if (playerPerformingAction.characterCombatManager.canPerformRollingAttack)
        {
            PerformRollingtAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }
        if (playerPerformingAction.characterCombatManager.canPerformBackstepAttack)
        {
            PerformBackstepAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //of we are attack currentlly, and can combo, perform combo
        if(playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            //perform attack based on previous attack
            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack02, light_Attack_02, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_02)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack03, light_Attack_03, true);

            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_03)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack04, light_Attack_04, true);

            }
        }
        //otherwise, just do regular attack
        else if(!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, light_Attack_01, true);
        }

    }

    private void PerformRunningtAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are two handing, play two handed running attack
        //else tun one handed running attack

        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.RunningAttack01, run_Attack_01, true);
    }

    private void PerformRollingtAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are two handing, play two handed running attack
        //else tun one handed running attack
        playerPerformingAction.playerCombatManager.canPerformRollingAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.RollingAttack01, roll_Attack_01, true);
    }

    private void PerformBackstepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are two handing, play two handed running attack
        //else tun one handed running attack
        playerPerformingAction.playerCombatManager.canPerformBackstepAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.BackstepAttack01, backstep_Attack_01, true);
    }
}
