using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIMenu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    public virtual void OpenMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        WorldSoundFXManager.instance.PlayOpenMenuSound();
        menu.SetActive(true);
    }

    public virtual void CloseMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        player.canMove = true;
        player.canRotate = true;

    }

    public virtual void CloseMenuAfterFixedUpdate()
    {
        if (!menu.activeInHierarchy)
        {
            return;
        }
        StartCoroutine(WaitThenCloseMenu());
    }

    protected virtual IEnumerator WaitThenCloseMenu()
    {
        yield return new WaitForFixedUpdate();

        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }
}
