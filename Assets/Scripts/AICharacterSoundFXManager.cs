using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AICharacterSoundFXManager : CharacterSoundFXManager
{
    AICharacterManager aiCharacter;

    [Header("Dialogue")]
    public CharacterDialogueID characterDialogueID;
    public GameObject interactableDialogueCollider;
    public CharacterDialogue currentDialogue;
    public GameObject interactableDialogueObject;
    public bool dialogueIsPlaying = false;
    //optional conversation target to look at with ik

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
    }

    protected override void Start()
    {
        base.Start();

        if (characterDialogueID != CharacterDialogueID.NoDialogueID)
        {
            currentDialogue = WorldSaveGameManager.instance.GetCharacterDialogueByEnum(characterDialogueID);

            interactableDialogueObject = Instantiate(WorldAIManager.instance.dialogueInteractable, transform);
            NetworkObject networkObject = interactableDialogueObject.GetComponent<NetworkObject>();
            networkObject.Spawn();
            networkObject.TrySetParent(gameObject, true);
        }
    }

    public void PlayCurrentDialogueEvent()
    {
        if (currentDialogue == null)
        {
            return;
        }

        if(!dialogueIsPlaying)
        {
            currentDialogue.PlayDialogueEvent(aiCharacter);
        }
        else
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendNextDialoguePopUpInIndex(currentDialogue, aiCharacter);
        }
    }

    public void CancelCurrentDialogueEvent()
    {
        if (dialogueIsPlaying)
        {
            dialogueIsPlaying = false;
            PlayerUIManager.instance.playerUIPopUpManager.CancelDialoguePopUp(aiCharacter);
        }
    }

    //used for specific calls when dialogue ends(npc dies, shop opens, etc)
    public void OnCurrentDialogueEnded()
    {
        //get new dialogue based on stage id
        currentDialogue = WorldSaveGameManager.instance.GetCharacterDialogueByEnum(characterDialogueID);
    }
}
