using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Off Hand Melee Action")]
public class OffHandMeleeAction : WeaponItemAction
{
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        //check for dual attack

        if(!playerPerformingAction.playerCombatManager.canBlock)
        {
            return;
        }

        if (playerPerformingAction.playerCombatManager.isUsingItem)
        {
            return;
        }

        if (playerPerformingAction.playerNetworkManager.isAttacking.Value)
        {
            //DISABLE blocking when using a short/medium spear block attack is allowed with light attacks
            if(playerPerformingAction.IsOwner)
            {
                playerPerformingAction.playerNetworkManager.isBlocking.Value = false;
            }

            return;
        }

        if(playerPerformingAction.playerNetworkManager.isBlocking.Value)
        {
            return;
        }

        if(playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerNetworkManager.isBlocking.Value = true;
        }
    }
}
