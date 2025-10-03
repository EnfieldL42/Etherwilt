using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGroupFade : MonoBehaviour
{
    public CanvasGroup menu;
    public float fadeDuration;
    void OnEnable()
    {
        menu.DOFade(1, fadeDuration);
    }

    public void FadeOut()
    {
        menu.DOFade(0, fadeDuration);
        StartCoroutine(DisableSelf());
    }

    IEnumerator DisableSelf()
    {
        yield return new WaitForSeconds(fadeDuration);
        gameObject.SetActive(false);
        yield return null;
    }

}
