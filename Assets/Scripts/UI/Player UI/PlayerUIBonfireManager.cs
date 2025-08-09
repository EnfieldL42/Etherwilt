using UnityEngine;

public class PlayerUIBonfireManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    public void OpenBonfireManagerMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        PlayerUIManager.instance.bonfireWindowIsOpen = true;

        menu.SetActive(true);
    }

    public void CloseBonfireManagerMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = false;
        PlayerUIManager.instance.bonfireWindowIsOpen = false;
        menu.SetActive(false);
    }

    public void OpenTeleportLocationMenu()
    {
        CloseBonfireManagerMenu();
        PlayerUIManager.instance.playerUITeleportLocationManager.OpenTeleportLocationManagerMenu();
    }
}
