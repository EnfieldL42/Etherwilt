using UnityEngine;

public class PlayerUITutorialManager : MonoBehaviour
{
    public GameObject[] tutorialPopUp;

    public void SaveTutorialSeen(int tutorialID)
    {
        WorldSaveGameManager.instance.currentCharacterData.tutorialFinished[tutorialID] = true;
        WorldSaveGameManager.instance.SaveGame();
        PlayerInputManager.instance.playerControls.Enable();

        Time.timeScale = 1f;
    }
}
