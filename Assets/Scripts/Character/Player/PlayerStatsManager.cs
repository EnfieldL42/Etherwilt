using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        //we calculate here as on character creation, depending on class this will be calculated
        CalculateHealthBasedOnLevel(player.playerNetworkManager.vitality.Value);
        CalculateStaminaBasedOnLevel(player.playerNetworkManager.endurance.Value);

    }
}
