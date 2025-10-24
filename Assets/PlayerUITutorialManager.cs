using UnityEngine;

public class PlayerUITutorialManager : MonoBehaviour
{
    public GameObject[] tutorialPopUp;

    public void SaveTutorialSeen(int tutorialID)
    {
        WorldSaveGameManager.instance.currentCharacterData.tutorialFinished[tutorialID] = true;
        WorldSaveGameManager.instance.SaveGame();
        Time.timeScale = 1f;

        PlayerInputManager.instance.playerControls.Enable();
        PlayerInputManager.instance.cameraInput = Vector2.zero;

    }
}
