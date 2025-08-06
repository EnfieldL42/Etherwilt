using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumeables/Flask")]
public class FlaskItem : QuickSlotItem
{
    [Header("Flask Type")]
    public bool healthFlask = true;

    [Header("Restoration Value")]
    [SerializeField] int flaskRestoration = 50;

    [Header("Empty Item")]
    public GameObject emptyFlaskItem;
    public string emptyFlaskAnimation;

    public override bool CanIUseThisItem(PlayerManager player)
    {
        if(!player.playerCombatManager.isUsingItem && player.isPerformingAction)
        {
            return false;
        }

        if(player.playerNetworkManager.isAttacking.Value)
        {
            return false;
        }

        return true;
    }

    public override void AttemptToUseItem(PlayerManager player)
    {
        if (!CanIUseThisItem(player))
        {
            return;
        }


        if (healthFlask && player.playerNetworkManager.remainingHealthFlasks.Value <= 0)
        {


            if (player.playerCombatManager.isUsingItem)
            {
                return;
            }



            player.playerCombatManager.isUsingItem = true;


            if (player.IsOwner)
            {
                player.playerAnimatorManager.PlayTargetActionAnimation(emptyFlaskAnimation, false, false, true, true, false);
                player.playerNetworkManager.HideWeaponServerRpc();
            }

            Destroy(player.playerEffectsManager.activeQuickSlotItemFX);
            GameObject emptyFlask = Instantiate(emptyFlaskItem, player.playerEquipmentManager.rightHandWeaponSlot.transform);
            player.playerEffectsManager.activeQuickSlotItemFX = emptyFlask;
            return;
        }


        //check for chugging

        if(player.playerCombatManager.isUsingItem)
        {
            if (player.IsOwner)
            {
                player.playerNetworkManager.isChugging.Value = true;

                return;
            }
        }

        player.playerCombatManager.isUsingItem = true;
        player.playerEffectsManager.activeQuickSlotItemFX = Instantiate(itemModel, player.playerEquipmentManager.rightHandWeaponSlot.transform);


        if (player.IsOwner)
        {
            player.playerAnimatorManager.PlayTargetActionAnimation(useItemAnimation, false, false, true, true, false);
            player.playerNetworkManager.HideWeaponServerRpc();
        }
    }

    public override void SuccessfullyUseItem(PlayerManager player)
    {
        base.SuccessfullyUseItem(player);

        if(player.IsOwner)
        {
            if(healthFlask)
            {
                player.playerNetworkManager.currentHealth.Value += flaskRestoration;
                player.playerNetworkManager.remainingHealthFlasks.Value -= 1;
            }

        }

        if (healthFlask && player.playerNetworkManager.currentHealth.Value <= 0)
        {
            Destroy(player.playerEffectsManager.activeQuickSlotItemFX);
            GameObject emptyFlask = Instantiate(emptyFlaskItem, player.playerEquipmentManager.rightHandWeaponSlot.transform);
            player.playerEffectsManager.activeQuickSlotItemFX = emptyFlask;
        }

        PlayeHealingFX(player);
    }

    private void PlayeHealingFX(PlayerManager player)
    {
        Instantiate(WorldCharacterEffectsManager.instance.healingFlaskVFX, player.transform);
        player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.healingFlaskSFX);
            
    }
}
