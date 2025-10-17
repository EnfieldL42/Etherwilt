using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIButtonStartGameSound : MonoBehaviour, ISelectHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayStartGameSound);
    }

    private void PlayStartGameSound()
    {
        WorldSoundFXManager.instance.PlayUIStartGameSound(1);
    }

    public void OnSelect(BaseEventData eventData)
    {
        WorldSoundFXManager.instance.PlayUISwitchSound(1);
    }
}
