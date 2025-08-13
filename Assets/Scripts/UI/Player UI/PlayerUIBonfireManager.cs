using UnityEngine;

public class PlayerUIBonfireManager : PlayerUIMenu
{
    //public override void OpenMenu()
    //{
    //    base.OpenMenu();
    //    PlayerUIManager.instance.bonfireWindowIsOpen = true;

    //}

    //public override void CloseMenu()
    //{
    //    base.CloseMenu();
    //    PlayerUIManager.instance.bonfireWindowIsOpen = false;

    //}

    public void OpenTeleportLocationMenu()
    {
        CloseMenu();
        PlayerUIManager.instance.playerUITeleportLocationManager.OpenMenu();
    }

    public void OpenLevelUpMenu()
    {
        CloseMenu();
        PlayerUIManager.instance.playerUILevelUpManager.OpenMenu();
    }
}
