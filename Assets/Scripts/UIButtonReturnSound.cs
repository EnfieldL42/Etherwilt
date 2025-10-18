using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIButtonReturnSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Make the EventSystem actually select this GameObject so Submit works.
        EventSystem.current.SetSelectedGameObject(gameObject);

        // Optional: also call Select() on the Selectable to ensure visuals are consistent
        var s = GetComponent<Selectable>();
        if (s != null) s.Select();
    }
}
