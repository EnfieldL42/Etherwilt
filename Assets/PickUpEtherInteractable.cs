using UnityEngine;

public class PickUpEtherInteractable : Interactable
{
    public int etherCount = 0;


    public override void Interact(PlayerManager player)
    {
        WorldSaveGameManager.instance.currentCharacterData.hasDeadSpot = false;
        player.playerStatsManager.AddEther(etherCount);
        Destroy(gameObject);
    }
}
