using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01"; //main hand
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

        if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
        }
        if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
        {

        }
    }
}
