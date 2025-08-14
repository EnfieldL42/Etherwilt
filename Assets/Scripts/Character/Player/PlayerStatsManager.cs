using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;

    [Header("Runes")]
    public int ether = 0;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        //we calculate here as on character creation, depending on class this will be calculated
        CalculateHealthBasedOnLevel(player.playerNetworkManager.health.Value);
        CalculateStaminaBasedOnLevel(player.playerNetworkManager.endurance.Value);

    }

    public void AddEther(int etherToAdd)
    {
        ether += etherToAdd;
        PlayerUIManager.instance.playerUIHudManager.SetEtherCount(etherToAdd);
    }

}
