using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIButtonConfirmSound : MonoBehaviour, ISelectHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayConfirmSound);
    }

    private void PlayConfirmSound()
    {
        WorldSoundFXManager.instance.PlayUIConfirmSound(1);
    }

    public void OnSelect(BaseEventData eventData)
    {
        WorldSoundFXManager.instance.PlayUISwitchSound(1);
    }
}
