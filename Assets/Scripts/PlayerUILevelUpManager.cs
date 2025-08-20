using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerUILevelUpManager : PlayerUIMenu
{
    [Header("Levels")]
    [SerializeField] int[] playerLevels = new int[396];
    [SerializeField] int baseLevelCost = 100;
    [SerializeField] int totalLevelUpCost = 0;

    [Header("Character Stats")]
    [SerializeField] TextMeshProUGUI characterLevelText;
    [SerializeField] TextMeshProUGUI etherHeldText;
    [SerializeField] TextMeshProUGUI etherNeededText;
    [SerializeField] TextMeshProUGUI healthLevelText;
    [SerializeField] TextMeshProUGUI enduranceLevelText;
    [SerializeField] TextMeshProUGUI strengthLevelText;
    [SerializeField] TextMeshProUGUI dexterityLevelText;
    [SerializeField] TextMeshProUGUI weaponMasteryLevelText;
    [SerializeField] TextMeshProUGUI magicMasteryLevelText;
    [SerializeField] TextMeshProUGUI breakerMasteryLevelText;
    [SerializeField] TextMeshProUGUI tankMasteryLevelText;

    [Header("Project Character Stats")]
    [SerializeField] TextMeshProUGUI projectedCharacterLevelText;
    [SerializeField] TextMeshProUGUI projectedEtherHeldText;
    [SerializeField] TextMeshProUGUI projectedHealthLevelText;
    [SerializeField] TextMeshProUGUI projectedEnduranceLevelText;
    [SerializeField] TextMeshProUGUI projectedStrengthLevelText;
    [SerializeField] TextMeshProUGUI projectedDexterityLevelText;
    [SerializeField] TextMeshProUGUI projectedWeaponMasteryLevelText;
    [SerializeField] TextMeshProUGUI projectedMagicMasteryLevelText;
    [SerializeField] TextMeshProUGUI projectedBreakerMasteryLevelText;
    [SerializeField] TextMeshProUGUI projectedTankMasteryLevelText;

    [Header("Sliders")]
    public CharacterAttribute currentSelectedAtribute;
    public Slider healthSlider;
    public Slider enduranceSlider;
    public Slider strengthSlider;
    public Slider dexteritySlider;
    public Slider weaponMasterySlider;
    public Slider magicMasterySlider;
    public Slider breakerMasterySlider;
    public Slider tankMasterySlider;

    [Header("Buttons")]
    [SerializeField] Button confirmLevelsButton;

    private void Awake()
    {
        SetAllLevelsCost();
    }

    public override void OpenMenu()
    {
        base.OpenMenu();

        SetCurrentStats();
    }

    private void SetCurrentStats()
    {

        healthSlider.value = healthSlider.minValue;
        enduranceSlider.value = enduranceSlider.minValue;
        strengthSlider.value = strengthSlider.minValue;
        dexteritySlider.value = dexteritySlider.minValue;
        weaponMasterySlider.value = weaponMasterySlider.minValue;
        magicMasterySlider.value = magicMasterySlider.minValue;
        breakerMasterySlider.value = breakerMasterySlider.minValue;
        tankMasterySlider.value = tankMasterySlider.minValue;

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


        weaponMasteryLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.weaponMastery.Value.ToString();
        projectedWeaponMasteryLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.weaponMastery.Value.ToString();
        weaponMasterySlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.weaponMastery.Value;

        magicMasteryLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.magicMastery.Value.ToString();
        projectedMagicMasteryLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.magicMastery.Value.ToString();
        magicMasterySlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.magicMastery.Value;

        breakerMasteryLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.breakerMastery.Value.ToString();
        projectedBreakerMasteryLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.breakerMastery.Value.ToString();
        breakerMasterySlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.breakerMastery.Value;

        tankMasteryLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.tankMastery.Value.ToString();
        projectedTankMasteryLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.tankMastery.Value.ToString();
        tankMasterySlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.tankMastery.Value;

        healthSlider.Select();
        healthSlider.OnSelect(null);

    }

    public void UpdateSliderBasedOnCurrentlySelectedAttributes()
    {
        PlayerManager player = PlayerUIManager.instance.localPlayer;

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
            case CharacterAttribute.WeaponMastery:
                projectedWeaponMasteryLevelText.text = weaponMasterySlider.value.ToString();
                break;
            case CharacterAttribute.MagicMastery:
                projectedMagicMasteryLevelText.text = magicMasterySlider.value.ToString();
                break;
            case CharacterAttribute.BreakerMastery:
                projectedBreakerMasteryLevelText.text = breakerMasterySlider.value.ToString();
                break;
            case CharacterAttribute.TankMastery:
                projectedTankMasteryLevelText.text = tankMasterySlider.value.ToString();
                break;
            default:
                break;
        }

        CalculateLevelCost(
            Mathf.RoundToInt(PlayerUIManager.instance.localPlayer.characterStatsManager.CalculateCharacterLevelBasedOnAttributes()),
            Mathf.RoundToInt(PlayerUIManager.instance.localPlayer.characterStatsManager.CalculateCharacterLevelBasedOnAttributes(true)));

        projectedCharacterLevelText.text = player.characterStatsManager.CalculateCharacterLevelBasedOnAttributes(true).ToString();
        etherNeededText.text = totalLevelUpCost.ToString();

        //check cost
        if (totalLevelUpCost > player.playerStatsManager.ether)
        {
            //disable confirm button so you cant level up
            confirmLevelsButton.interactable = false;
            //change level up field text to red
        }
        else
        {
            confirmLevelsButton.interactable = true;
        }

        ChangeTextColorDependingOnCost();

    }

    public void ConfirmLevels()
    {
        PlayerManager player = PlayerUIManager.instance.localPlayer;

        //deduct cost from total ether
        player.playerStatsManager.ether -= totalLevelUpCost;
        //set new stats on player


        player.playerNetworkManager.health.Value = Mathf.RoundToInt(healthSlider.value);
        player.playerNetworkManager.endurance.Value = Mathf.RoundToInt(enduranceSlider.value);
        player.playerNetworkManager.strength.Value = Mathf.RoundToInt(strengthSlider.value);
        player.playerNetworkManager.dexterity.Value = Mathf.RoundToInt(dexteritySlider.value);
        player.playerNetworkManager.weaponMastery.Value = Mathf.RoundToInt(weaponMasterySlider.value);
        player.playerNetworkManager.magicMastery.Value = Mathf.RoundToInt(magicMasterySlider.value);
        player.playerNetworkManager.breakerMastery.Value = Mathf.RoundToInt(breakerMasterySlider.value);
        player.playerNetworkManager.tankMastery.Value = Mathf.RoundToInt(tankMasterySlider.value);

        SetCurrentStats();
        ChangeTextColorDependingOnCost();
        WorldSaveGameManager.instance.SaveGame();
    }

    public void SetAllLevelsCost()
    {
        for(int i = 0; i < playerLevels.Length; i++)
        {
            if (i == 0)
            {
                continue;
            }
            playerLevels[i] = baseLevelCost + (50 * i);
        }
    }

    public void CalculateLevelCost(int currentLevel, int projectedLevel)
    {
        int totalCost = 0;

        for(int i = 0; i < projectedLevel; i++)
        {
            if (i < currentLevel)
            {
                continue;
            }
            if (i > playerLevels.Length)
            {
                continue;
            }

            totalCost += playerLevels[i];
        }

        totalLevelUpCost = totalCost;

        projectedEtherHeldText.text = (PlayerUIManager.instance.localPlayer.playerStatsManager.ether - totalCost).ToString();

        if (totalCost > PlayerUIManager.instance.localPlayer.playerStatsManager.ether)
        {
            projectedEtherHeldText.color = Color.red;
        }
        else
        {
           projectedEtherHeldText.color = Color.white;
        }
    }

    public void ChangeTextColorDependingOnCost()
    {
        PlayerManager player = PlayerUIManager.instance.localPlayer;

        int projectedHealthLevel = Mathf.RoundToInt(healthSlider.value);
        int projectedEnduranceLevel = Mathf.RoundToInt(enduranceSlider.value);
        int projectedStrengthLevel = Mathf.RoundToInt(strengthSlider.value);
        int projectedDexterityLevel = Mathf.RoundToInt(dexteritySlider.value);
        int projectedWeaponMasteryLevel = Mathf.RoundToInt(weaponMasterySlider.value);
        int projectedMagicMasteryLevel = Mathf.RoundToInt(magicMasterySlider.value);
        int projectedBreakerMasteryLevel = Mathf.RoundToInt(breakerMasterySlider.value);
        int projectedTankMasteryLevel = Mathf.RoundToInt(tankMasterySlider.value);

        ChangeTextFieldToSpecificColorBasedOnStat(player, projectedHealthLevelText, player.playerNetworkManager.health.Value, projectedHealthLevel);
        ChangeTextFieldToSpecificColorBasedOnStat(player, projectedEnduranceLevelText, player.playerNetworkManager.endurance.Value, projectedEnduranceLevel);
        ChangeTextFieldToSpecificColorBasedOnStat(player, projectedStrengthLevelText, player.playerNetworkManager.strength.Value, projectedStrengthLevel);
        ChangeTextFieldToSpecificColorBasedOnStat(player, projectedDexterityLevelText, player.playerNetworkManager.dexterity.Value, projectedDexterityLevel);
        ChangeTextFieldToSpecificColorBasedOnStat(player, projectedWeaponMasteryLevelText, player.playerNetworkManager.weaponMastery.Value, projectedWeaponMasteryLevel);
        ChangeTextFieldToSpecificColorBasedOnStat(player, projectedMagicMasteryLevelText, player.playerNetworkManager.magicMastery.Value, projectedMagicMasteryLevel);
        ChangeTextFieldToSpecificColorBasedOnStat(player, projectedBreakerMasteryLevelText, player.playerNetworkManager.breakerMastery.Value, projectedBreakerMasteryLevel);
        ChangeTextFieldToSpecificColorBasedOnStat(player, projectedTankMasteryLevelText, player.playerNetworkManager.tankMastery.Value, projectedTankMasteryLevel);

        int projectedPlayerLevel = Mathf.RoundToInt(player.characterStatsManager.CalculateCharacterLevelBasedOnAttributes(true));
        int playerLevel = Mathf.RoundToInt(player.characterStatsManager.CalculateCharacterLevelBasedOnAttributes());


        if (projectedPlayerLevel == playerLevel)
        {
            projectedCharacterLevelText.color = Color.white;
            projectedEtherHeldText.color = Color.white;
            etherNeededText.color = Color.white;
        }

        if (totalLevelUpCost <= player.playerStatsManager.ether)
        {
            etherNeededText.color = Color.white;

            if (projectedPlayerLevel > playerLevel)
            {
                projectedEtherHeldText.color = Color.red;
                projectedCharacterLevelText.color = Color.blue;
            }
            else
            {
                etherNeededText.color = Color.red;

                if (projectedPlayerLevel > playerLevel)
                {
                    projectedCharacterLevelText.color = Color.red;
                }
            }

        }
        else
        {
            etherNeededText.color = Color.red;
        }
    }

    private void ChangeTextFieldToSpecificColorBasedOnStat(PlayerManager player, TextMeshProUGUI textField, int stat, int projectedStat)
    {
        if (projectedStat == stat)
        {
            textField.color = Color.white;
        }

        //can afford
        if (totalLevelUpCost <= player.playerStatsManager.ether)
        {
            etherNeededText.color = Color.white;

            if (projectedStat > stat)
            {
                textField.color = Color.blue;
            }
            else
            {
                textField.color = Color.white;
            }


        }
        //cant afford
        else
        {
            if (projectedStat > stat)
            {
                textField.color = Color.red;
            }
            else
            {
                textField.color = Color.white;
            }
        }
    }
}

