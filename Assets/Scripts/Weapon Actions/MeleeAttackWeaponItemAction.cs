using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Melee Attack Action")]
public class MeleeAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string melee_Attack_01 = "Melee_Light_Attack_01"; //main hand
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

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {

        if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.MeleeAttack01, melee_Attack_01, true);
        }
        if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
        {

        }
    }
}
