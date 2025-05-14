using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string light_Attack_01 = "Main_Light_Attack_01"; //main hand
    [SerializeField] string light_Attack_02 = "Main_Light_Attack_02"; 
    [SerializeField] string light_Attack_03 = "Main_Light_Attack_03"; 
    [SerializeField] string light_Attack_04 = "Main_Light_Attack_04"; 
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
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, light_Attack_02, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_02)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack03, light_Attack_03, true);

            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_03)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack04, light_Attack_04, true);

            }
        }
        //otherwise, just do regular attack
        else if(!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
        }

    }
}
