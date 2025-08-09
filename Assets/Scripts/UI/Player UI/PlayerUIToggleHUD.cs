using UnityEngine;

public class PlayerUIToggleHUD : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerUIManager.instance.playerUIHudManager.ToggleHUD(false);
    }

    private void OnDisable()
    {
        PlayerUIManager.instance.playerUIHudManager.ToggleHUD(true);
    }
}
