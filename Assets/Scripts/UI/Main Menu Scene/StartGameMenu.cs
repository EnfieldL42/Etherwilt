using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class StartGameMenu : MonoBehaviour
{
    public GameObject titleScreenUI;
    public TitleScreenManager titleScreenManager;
    public TextMeshProUGUI text;
    public float pulseTime;
    private IDisposable listener;
    private Sequence pulse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.DOFade(0, 0f);
        listener = InputSystem.onAnyButtonPress.CallOnce(OnButtonPress);
        pulse = DOTween.Sequence(text);
        pulse.Append(text.DOFade(1, pulseTime));
        pulse.SetLoops(-1, LoopType.Yoyo);
    }

    void OnButtonPress(InputControl button)
    {
        listener.Dispose();
        StartCoroutine(Fade());
        titleScreenManager.StartNetworkAsHost();
        WorldSoundFXManager.instance.PlayUIPressToStartGameSound();
    }

    IEnumerator Fade()  
    {
        pulse.Pause();
        Sequence fadeText = DOTween.Sequence(text);
        fadeText.Append(text.DOFade(1, 0.2f));
        fadeText.Insert(0f, text.DOGlowColor(Color.white, 0f));
        fadeText.Append(text.DOFade(0, 0.8f));
        fadeText.Insert(0.2f, text.DOGlowColor(new Color(0f, 0f, 0f, 0f), 0.8f));
        yield return new WaitForSecondsRealtime(1.2f);

        titleScreenUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
        yield return null;
    }
}
