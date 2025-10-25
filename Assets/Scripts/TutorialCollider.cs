using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    [SerializeField] int tutorialID;
    [SerializeField] Collider col;


    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        if (WorldSaveGameManager.instance.currentCharacterData.tutorialFinished[tutorialID])
        {
            col.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WorldSoundFXManager.instance.PlayTutorialPopUpSound();
            PlayerUIManager.instance.playerUITutorialManager.tutorialPopUp[tutorialID].SetActive(true);
            col.enabled = false;
            PlayerInputManager.instance.playerControls.Disable();

            Time.timeScale = 0f;
        }
    }


}
