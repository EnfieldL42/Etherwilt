using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUILevelUpManager : PlayerUIMenu
{
    [Header("Character Stats")]
    [SerializeField] TextMeshProUGUI characterLevelText;
    [SerializeField] TextMeshProUGUI etherHeldText;
    [SerializeField] TextMeshProUGUI etherNeededText;
    [SerializeField] TextMeshProUGUI healthLevelText;
    [SerializeField] TextMeshProUGUI enduranceLevelText;
    [SerializeField] TextMeshProUGUI strengthLevelText;
    [SerializeField] TextMeshProUGUI dexterityLevelText;

    [Header("Project Character Stats")]
    [SerializeField] TextMeshProUGUI projectedCharacterLevelText;
    [SerializeField] TextMeshProUGUI projectedEtherHeldText;
    [SerializeField] TextMeshProUGUI projectedHealthLevelText;
    [SerializeField] TextMeshProUGUI projectedEnduranceLevelText;
    [SerializeField] TextMeshProUGUI projectedStrengthLevelText;
    [SerializeField] TextMeshProUGUI projectedDexterityLevelText;

    [Header("Sliders")]
    public CharacterAttribute currentSelectedAtribute;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider enduranceSlider;
    [SerializeField] Slider strengthSlider;
    [SerializeField] Slider dexteritySlider;

    public override void OpenMenu()
    {
        base.OpenMenu();

        SetCurrentStats();
    }

    private void SetCurrentStats()
    {
        //character level
        characterLevelText.text = PlayerUIManager.instance.localPlayer.characterStatsManager.CalculateCharacterLevelBasedOnAttributes().ToString();
        projectedCharacterLevelText.text = PlayerUIManager.instance.localPlayer.characterStatsManager.CalculateCharacterLevelBasedOnAttributes().ToString();

        //runes
        etherHeldText.text = PlayerUIManager.instance.localPlayer.playerStatsManager.ether.ToString();
        projectedEtherHeldText.text = PlayerUIManager.instance.localPlayer.playerStatsManager.ether.ToString();
        etherNeededText.text = "0";

        //attributes
        healthLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.health.Value.ToString();
        projectedHealthLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.health.Value.ToString();
        healthSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.health.Value;

        enduranceLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.endurance.Value.ToString();
        projectedEnduranceLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.endurance.Value.ToString();
        enduranceSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.endurance.Value;

        strengthLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.strength.Value.ToString();
        projectedStrengthLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.strength.Value.ToString();
        strengthSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.strength.Value;

        dexterityLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.dexterity.Value.ToString();
        projectedDexterityLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.dexterity.Value.ToString();
        dexteritySlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.dexterity.Value;

        healthSlider.Select();
        healthSlider.OnSelect(null);

    }

    public void UpdateSliderBasedOnCurrentlySelectedAttributes()
    {
        switch (currentSelectedAtribute)
        {
            case CharacterAttribute.Health:
                projectedHealthLevelText.text = healthSlider.value.ToString();
                break;
            case CharacterAttribute.Endurance:
                projectedEnduranceLevelText.text = enduranceSlider.value.ToString();
                break;
            case CharacterAttribute.Strength:
                projectedStrengthLevelText.text = strengthSlider.value.ToString();
                break;
            case CharacterAttribute.Dexterity:
                projectedDexterityLevelText.text = dexteritySlider.value.ToString();
                break;
            default:
                break;
        }

    }

    public void ConfirmLevels()
    {
        //calculate cost
        //change stat colors if you can afford it or not
        //deduct cost from total ether
        //set new stats on player


        PlayerManager player = PlayerUIManager.instance.localPlayer;

        player.playerNetworkManager.health.Value = Mathf.RoundToInt(healthSlider.value);
        player.playerNetworkManager.endurance.Value = Mathf.RoundToInt(enduranceSlider.value);
        player.playerNetworkManager.strength.Value = Mathf.RoundToInt(strengthSlider.value);
        player.playerNetworkManager.dexterity.Value = Mathf.RoundToInt(dexteritySlider.value);

        SetCurrentStats();
    }

}
