using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public void EndIntroCutscene()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
    }
}