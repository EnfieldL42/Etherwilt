using UnityEngine;
using UnityEngine.UI;

public class PlayerUIBonfireManager : PlayerUIMenu
{
    [SerializeField] Button teleportButton;
    [SerializeField] Button levelUpButton;

    public override void OpenMenu()
    {
        base.OpenMenu();
        PlayerUIManager.instance.bonfireWindowIsOpen = true;

    }

    public override void CloseMenu()
    {
        base.CloseMenu();
        PlayerUIManager.instance.bonfireWindowIsOpen = false;

    }

    public void OpenTeleportLocationMenu()
    {
        CloseMenu();
        PlayerUIManager.instance.playerUITeleportLocationManager.OpenMenu();
    }

    public void CloseTeleportLocationMenu()
    {
        PlayerUIManager.instance.playerUITeleportLocationManager.CloseMenu();
        OpenMenu();
        teleportButton.Select();
    }

    public void OpenLevelUpMenu()
    {
        CloseMenu();
        PlayerUIManager.instance.playerUILevelUpManager.OpenMenu();
    }

    public void CloseLevelUpMenu()
    {
        PlayerUIManager.instance.playerUILevelUpManager.CloseMenu();
        OpenMenu();
        levelUpButton.Select();
    }
}
