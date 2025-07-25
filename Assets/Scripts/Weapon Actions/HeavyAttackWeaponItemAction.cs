using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01"; //main hand
    [SerializeField] string heavy_Attack_02 = "Main_Heavy_Attack_02";
    [SerializeField] string heavy_Attack_03 = "Main_Heavy_Attack_03";
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (!playerPerformingAction.IsOwner)
        {
            return;
        }


        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        //check for stops like stamina

        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }
        if (!playerPerformingAction.isGrounded)
        {
            return;
        }

        PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            //perform attack based on previous attack
            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack02, heavy_Attack_02, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == heavy_Attack_02)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack03, heavy_Attack_03, true);

            }
        }
        //otherwise, just do regular attack
        else if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
        }

    }
}
