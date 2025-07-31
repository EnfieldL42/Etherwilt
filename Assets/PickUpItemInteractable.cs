using UnityEngine;
using Unity.Netcode;

public class PickUpItemInteractable : Interactable
{
    //the two types of items pick ups are
    //world items - have unique save data
    //monster drops - no unique save data
    
    public ItemPickUpType pickUpType;

    [Header("Item")]
    [SerializeField] Item item;

    [Header("World Spawn Pick Up")]
    [SerializeField] int itemID;
    [SerializeField] bool hasBeenLooted = false;

    protected override void Start()
    {
        base.Start();

        if(pickUpType == ItemPickUpType.WorldSpawn)
        {
            CheckIfWorldItemWasAlreadyLooted();
        }

    }

    private void CheckIfWorldItemWasAlreadyLooted()
    {
        //hide item from non host players
        if(!NetworkManager.Singleton.IsHost)
        {
            gameObject.SetActive(false);
            return;
        }

        if(!WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey(itemID))
        {
            WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(itemID, false);
        }

        hasBeenLooted = WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted[itemID];

        if(hasBeenLooted)
        {
            gameObject.SetActive(false);
        }
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickUpItemSFX);
        player.playerInventoryManager.AddItemToInventory(item);

        PlayerUIManager.instance.playerUIPopUpManager.SentItemPopUp(item, 1);

        if(pickUpType == ItemPickUpType.WorldSpawn)
        {
            if(WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey((int)itemID))
            {
                WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Remove(itemID);
            }

            WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(itemID, true);
        }

        Destroy(gameObject);
    }
}
