using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumeables/Flask")]
public class FlaskItem : QuickSlotItem
{
    [Header("Flask Type")]
    [SerializeField] bool healthFlask = true;

    [Header("Restoration Value")]
    [SerializeField] int flaskRestoration = 50;

    [Header("Empty Item")]
    [SerializeField] GameObject emptyFlaskItem;

    public override bool CanIUseThisItem(PlayerManager player)
    {

        if(healthFlask && player.playerNetworkManager.remainingHealthFlasks.Value <= 0)
        {
            return false; 
        }
        if (!healthFlask && player.playerNetworkManager.remainingHealthFlasks.Value <= 0)
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

        player.playerEffectsManager.activeQuickSlotItemFX = Instantiate(itemModel, player.playerEquipmentManager.rightHandWeaponSlot.transform);


        if(player.IsOwner)
        {
            player.playerAnimatorManager.PlayTargetActionAnimation(useItemAnimation, true, false, true, true, false);
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
