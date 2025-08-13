using UnityEngine;
using UnityEngine.UI;

public class PlayerUITeleportLocationManager : PlayerUIMenu
{
    [Header("Teleport Locations")]
    [SerializeField] GameObject[] teleportLocations;

    public override void OpenMenu()
    {
        base.OpenMenu();

        CheckForUnlockedTeleports();
    }

    //public void OpenTeleportLocationManagerMenu()
    //{
    //    PlayerUIManager.instance.menuWindowIsOpen = true;
    //    PlayerUIManager.instance.bonfireWindowIsOpen = true;

    //    menu.SetActive(true);

    //    CheckForUnlockedTeleports();
    //}

    //public void CloseTeleportLocationManagerMenu()
    //{
    //    PlayerUIManager.instance.menuWindowIsOpen = false;
    //    PlayerUIManager.instance.bonfireWindowIsOpen = false;
    //    menu.SetActive(false);
    //}

    public void CheckForUnlockedTeleports()
    {
        bool hasFirstSelecteButton = false;


        for (int i = 0; i < teleportLocations.Length; i++)
        {
            for (int s = 0; s < WorldObjectManager.instance.bonfires.Count; s++)
            {
                if (WorldObjectManager.instance.bonfires[s].bonefireID == i)
                {
                    if (WorldObjectManager.instance.bonfires[s].isActivated.Value)
                    {
                        teleportLocations[i].SetActive(true);

                        if (!hasFirstSelecteButton)
                        {
                            hasFirstSelecteButton = true;
                            teleportLocations[i].GetComponent<Button>().Select();
                            teleportLocations[i].GetComponent<Button>().OnSelect(null);

                        }
                    }
                    else
                    {
                        teleportLocations[i].SetActive(false);
                    }
                }

            }
        }
    }

    public void TeleportToBonfire(int bonfireID)
    {
        for (int i = 0; i < WorldObjectManager.instance.bonfires.Count; i++)
        {
            if (WorldObjectManager.instance.bonfires[i].bonefireID == bonfireID)
            {
                WorldObjectManager.instance.bonfires[i].TeleportToBonfire();

                return;
            }
        }
    }
}
