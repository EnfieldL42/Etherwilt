using UnityEngine;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] UI_StatBar staminaBar;


    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(newValue);
    }

    public void SetMaxStaminaValue(float maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }
}
