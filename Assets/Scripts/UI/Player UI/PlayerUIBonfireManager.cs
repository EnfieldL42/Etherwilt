using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerUIBonfireManager : PlayerUIMenu
{
    [SerializeField] Button teleportButton;
    [SerializeField] Button levelUpButton;

    public override void OpenMenu()
    {
        base.OpenMenu();
        PlayerUIManager.instance.bonfireWindowIsOpen = true;
        StartCoroutine(RenderNothing());        
    }

    public override void CloseMenu()
    {
        base.CloseMenu();
        PlayerUIManager.instance.bonfireWindowIsOpen = false;
        PlayerCamera.instance.GetComponentInChildren<UniversalAdditionalCameraData>().SetRenderer(0);
    }

    public void OpenTeleportLocationMenu()
    {
        CloseMenu();
        PlayerCamera.instance.GetComponentInChildren<UniversalAdditionalCameraData>().SetRenderer(4);
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
        PlayerCamera.instance.GetComponentInChildren<UniversalAdditionalCameraData>().SetRenderer(4);
        PlayerUIManager.instance.playerUILevelUpManager.OpenMenu();
    }

    public void CloseLevelUpMenu()
    {
        PlayerUIManager.instance.playerUILevelUpManager.CloseMenu();
        OpenMenu();
        levelUpButton.Select();
    }

    IEnumerator RenderNothing()
    {
        yield return new WaitForSeconds(0.25f);
        PlayerCamera.instance.GetComponentInChildren<UniversalAdditionalCameraData>().SetRenderer(4);
    }
}
