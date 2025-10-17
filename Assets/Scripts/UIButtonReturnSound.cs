using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIButtonReturnSound : MonoBehaviour, ISelectHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayReturnSound);
    }

    private void PlayReturnSound()
    {
        WorldSoundFXManager.instance.PlayUIReturnSound(1);
    }

    public void OnSelect(BaseEventData eventData)
    {
        WorldSoundFXManager.instance.PlayUISwitchSound(1);
    }
}
