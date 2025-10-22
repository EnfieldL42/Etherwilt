using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] CanvasGroup[] canvasGroup;

    [Header("Stat Bars")]
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    [Header("Ether")]
    [SerializeField] float etherUpdateCountDelayTimer = 2.5f;
    private int pendingEtherToAdd;
    private Coroutine waitThenAddEtherCoroutines;
    [SerializeField] TextMeshProUGUI etherToAddText;
    [SerializeField] TextMeshProUGUI etherCountText;

    [Header("Quick Slots")]
    [SerializeField] Image rightWeaponQuickSlotIcon;
    [SerializeField] Image leftWeaponQuickSlotIcon;
    [SerializeField] Image quickSlotItemQuickSlotIcon;
    [SerializeField] TextMeshProUGUI quickSlotItemCount;

    [Header("Boss Health Bar")]
    public Transform bossHealthBarParent;
    public GameObject bossHealthBarObject;

    [Header("Lock On Target")]
    public LockOnUITracking lockOnTarget;

    [HideInInspector] public UI_Boss_HP_Bar currentBossHealthBar;

    public void ToggleHUD(bool status)
    {
        if (status && PlayerUIManager.instance.menuWindowIsOpen == false)
        {
            foreach (var canvas in canvasGroup)
            {
                //canvas.alpha = 1;
                canvas.DOFade(1f, 1f);
            }
        }
        else
        {
            foreach (var canvas in canvasGroup)
            {
                //canvas.alpha = 0;
                canvas.DOFade(0f, 1f);
            }
        }
    }

    public void ToggleHUDWithoutPopUps(bool status)
    {
        if (status)
        {
            //canvasGroup[0].alpha = 1;
            canvasGroup[0].DOFade(1f, 1f);
        }
        else
        {
            //canvasGroup[0].alpha = 0;
            canvasGroup[0].DOFade(0f, 1f);
        }
    }

    public void RefreshHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }

    public void SetEtherCount(int etherToAdd)
    {
        pendingEtherToAdd += etherToAdd;

        //wait for potentially more ether then add them all
        if (waitThenAddEtherCoroutines != null)
        {
            StopCoroutine(waitThenAddEtherCoroutines);
        }

        waitThenAddEtherCoroutines = StartCoroutine(WaitThenUpdateEtherCount());
    }

    public IEnumerator WaitThenUpdateEtherCount()
    {
        //wait for timer in case more ether are queued up
        float timer = etherUpdateCountDelayTimer;
        int etherToAdd = pendingEtherToAdd;

        if (etherToAdd >= 0)
        {
            etherToAddText.text = "+ " + etherToAdd.ToString();
        }
        else
        {
            etherToAddText.text = "- " + Mathf.Abs(etherToAdd).ToString();
        }

        etherToAddText.text = "+ " + etherToAdd.ToString();
        etherToAddText.enabled = true;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            //if more ether are queued up, re update total new ether count
            if (etherToAdd != pendingEtherToAdd)
            {
                etherToAdd = pendingEtherToAdd;
                etherToAddText.text = etherToAdd.ToString();
            }

            yield return null;
        }

        //update ether count, reset pending ether and hide pending ether
        etherToAddText.enabled = false;
        pendingEtherToAdd = 0;
        etherCountText.text = PlayerUIManager.instance.localPlayer.playerStatsManager.ether.ToString();

        yield return null;
    }

    public void SetNewHealthValue(int oldValue, int newValue)
    {
        healthBar.SetStat(Mathf.RoundToInt(newValue));

    }

    public void SetMaxHealthValue(int maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }

    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxStaminaValue(float maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }

    public void SetRightWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon = null;
            return;
        }

        if (weapon.itemIcon == null)
        {
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }
        rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
        rightWeaponQuickSlotIcon.enabled = true;

    }

    public void SetLeftWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon = null;
            return;
        }

        if (weapon.itemIcon == null)
        {
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }


        leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
        leftWeaponQuickSlotIcon.enabled = true;

    }

    public void SetQuickSlotItemQuickSlotIcon(QuickSlotItem quickSlotItem)
    {
        if (quickSlotItem == null)
        {
            quickSlotItemQuickSlotIcon.enabled = false;
            quickSlotItemQuickSlotIcon.sprite = null;
            quickSlotItemCount.enabled = false;
            return;
        }

        if (quickSlotItem.itemIcon == null)
        {
            quickSlotItemQuickSlotIcon.enabled = false;
            quickSlotItemQuickSlotIcon.sprite = null;
            quickSlotItemCount.enabled = false;
            return;
        }


        quickSlotItemQuickSlotIcon.sprite = quickSlotItem.itemIcon;
        quickSlotItemQuickSlotIcon.enabled = true;

        if (quickSlotItem.isConsumable)
        {
            quickSlotItemCount.text = quickSlotItem.GetCurrentAmount(PlayerUIManager.instance.localPlayer).ToString();
            quickSlotItemCount.enabled = true;
        }
        else
        {
            quickSlotItemCount.enabled = false;
        }

    }

    public void UpdateQuickSlotItemQuickSlotIcon()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        QuickSlotItem quickSlotItem = player.playerInventoryManager.currentQuickSlotItem;

        if (quickSlotItem == null)
        {
            return;
        }

        if (quickSlotItem.isConsumable)
        {
            quickSlotItemCount.text = quickSlotItem.GetCurrentAmount(player).ToString();
        }
    }

    public void ToggleLockOnUI(CharacterManager target)
    {
        if (target == null)
        {
            lockOnTarget.gameObject.SetActive(false);
        }
        else
        {
            lockOnTarget.gameObject.SetActive(true);
            lockOnTarget.SetTracking(target.GetComponentInChildren<LockOnTrasform>());
        }
    }
}
