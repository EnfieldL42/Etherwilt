using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerUIMenu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    public virtual void OpenMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        WorldSoundFXManager.instance.PlayOpenMenuSound();
        menu.SetActive(true);
        menu.GetComponent<CanvasGroup>().DOFade(1f, 0.25f);
        menu.GetComponent<CanvasGroup>().interactable = true;
    }

    public virtual void CloseMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.GetComponent<CanvasGroup>().interactable = false;
        //menu.SetActive(false);
        StartCoroutine(FadeMenu());
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        player.canMove = true;
        player.canRotate = true;

    }

    IEnumerator FadeMenu()
    {
        menu.GetComponent<CanvasGroup>().interactable = false;
        menu.GetComponent<CanvasGroup>().DOFade(0f, 0.25f);
        yield return new WaitForSeconds(0.5f);
        //  PlayerUIManager.instance.menuWindowIsOpen = false;
        if(!PlayerUIManager.instance.menuWindowIsOpen)
        {
            menu.SetActive(false);
        }
        yield return null;
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
        PlayerCamera.instance.GetComponentInChildren<UniversalAdditionalCameraData>().SetRenderer(0);
        StartCoroutine(FadeMenu());
    }
}
