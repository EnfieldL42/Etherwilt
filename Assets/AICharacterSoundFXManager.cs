using UnityEngine;

public class AICharacterSoundFXManager : CharacterSoundFXManager
{
    [Header("Dialogue SFX")]
    //character dialogue ID
    public GameObject interactableDialogueCollider;
    //current character dialogue (will be scriptable object)
    //optional farewell dialogue
    public bool dialogueIsPlaying = false;
    //optional conversation target to look at with ik



    public void PlayCurrentDialogueEvent()
    {

    }

    //generic farewell dialogue that can be changed with different farewell sets
    public void PlayFarewellDialogueEvent()
    {

    }

    public void CancelCurrentDialogueEvent()
    {

    }

    //used for specific calls when dialogue ends(npc dies, shop opens, etc)
    public void OnCurrentDialogueEnded()
    {

    }
}
