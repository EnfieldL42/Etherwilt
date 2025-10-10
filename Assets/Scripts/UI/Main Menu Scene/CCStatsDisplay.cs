using TMPro;
using UnityEngine;

public class CCStatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI vitality;
    public TextMeshProUGUI endurance;
    public TextMeshProUGUI strength;
    public TextMeshProUGUI dexterity;
    public TextMeshProUGUI weaponMastery;
    public TextMeshProUGUI magicMastery;
    public TextMeshProUGUI breakerMastery;
    public TextMeshProUGUI tankMastery;

    // Update is called once per frame

    public void UpdateStats(PlayerManager player)
    {
        vitality.text = player.playerNetworkManager.health.Value.ToString();
        endurance.text = player.playerNetworkManager.endurance.Value.ToString();
        strength.text = player.playerNetworkManager.strength.Value.ToString();
        dexterity.text = player.playerNetworkManager.dexterity.Value.ToString();
        weaponMastery.text = player.playerNetworkManager.weaponMastery.Value.ToString();
        magicMastery.text = player.playerNetworkManager.magicMastery.Value.ToString();
        breakerMastery.text = player.playerNetworkManager.breakerMastery.Value.ToString();
        tankMastery.text = player.playerNetworkManager.tankMastery.Value.ToString();
    }
}
