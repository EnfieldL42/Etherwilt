using UnityEngine;
using System.Collections.Generic;
using System.Collections;


[CreateAssetMenu(menuName = "A.I/Dialogue")]
public class CharacterDialogue : ScriptableObject
{
    [Header("Dialogue Requirements")]
    public int requiredStageID = 0; 

    [Header("Greeting Dialogue")]
    [TextArea] public List<string> greetingDialogueString = new List<string>();
    public List<AudioClip> greetingDialogueAudio = new List<AudioClip>();
    private bool greetingHasPlayed = false;

    [Header("Core Dialogue")]
    [TextArea] public List<string> dialogueString = new List<string>();
    public List<AudioClip> dialogueAudio = new List<AudioClip>();
    public int dialogueIndex = 0;

    [Header("Farewell Dialogue")]
    [TextArea] public List<string> farewellDialogueString = new List<string>();
    public List<AudioClip> farewellDialogueAudio = new List<AudioClip>();
    private bool farewellHasPlayed = false;

    //optional settings
    //face Character
    //Kill on cancel
    //open menu on cancel
    //etc

    [Header("End Triggers")]
    [SerializeField] bool setStageIndex = false; //this will set whether the next dialogue will be different
    [SerializeField] int stageID = 0;

    public void PlayDialogueEvent(AICharacterManager aICharacter)
    {
        if (dialogueString.Count != dialogueAudio.Count)
        {
            return;
        }

        aICharacter.aICharacterSoundFXManager.dialogueIsPlaying = true;
        PlayerUIManager.instance.playerUIPopUpManager.SendDialoguePopUp(this, aICharacter);
    }

    public IEnumerator PlayerDialogueCoroutine(AICharacterManager aICharacter)
    {
        //play a random greeting dialogue, then wait the length of hat audio clip
        if (greetingDialogueAudio.Count != 0 && !greetingHasPlayed)
        {
            greetingHasPlayed = true;
            int randomGreetingDiaogueIndex = Random.Range(0, greetingDialogueAudio.Count);
            PlayerUIManager.instance.playerUIPopUpManager.SetDialoguePopUpSubtitles(greetingDialogueString[randomGreetingDiaogueIndex]);
            aICharacter.aICharacterSoundFXManager.PlaySoundFX(greetingDialogueAudio[randomGreetingDiaogueIndex]);
            yield return new WaitForSeconds(greetingDialogueAudio[randomGreetingDiaogueIndex].length + 1);
        }

        while (dialogueIndex < dialogueString.Count)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SetDialoguePopUpSubtitles(dialogueString[dialogueIndex]);
            aICharacter.aICharacterSoundFXManager.PlaySoundFX(dialogueAudio[dialogueIndex]);
            yield return new WaitForSeconds(dialogueAudio[dialogueIndex].length + 1);
            dialogueIndex++;
        }

        if (farewellDialogueAudio.Count != 0 && !farewellHasPlayed)
        {
            farewellHasPlayed = true;
            int randomFarewellDiaogueIndex = Random.Range(0, farewellDialogueAudio.Count);
            PlayerUIManager.instance.playerUIPopUpManager.SetDialoguePopUpSubtitles(farewellDialogueString[randomFarewellDiaogueIndex]);
            aICharacter.aICharacterSoundFXManager.PlaySoundFX(farewellDialogueAudio[randomFarewellDiaogueIndex]);
            yield return new WaitForSeconds(farewellDialogueAudio[randomFarewellDiaogueIndex].length + 1);
        }

        OnDialogueEnded(aICharacter);
        PlayerUIManager.instance.playerUIPopUpManager.EndDialoguePopUp();

        yield return null;
    }

    public void OnDialogueEnded(AICharacterManager aICharacter)
    {
        //do stuff with character dialogue scriptable is desired
        greetingHasPlayed = false;
        dialogueIndex = 0;

        if (setStageIndex)
        {
            WorldSaveGameManager.instance.SetStageOfDialogue(aICharacter.aICharacterSoundFXManager.characterDialogueID, stageID);
        }


        //do stuff with ai character if desired

        aICharacter.aICharacterSoundFXManager.OnCurrentDialogueEnded();
    }
    public void OnDialogueCancelled(AICharacterManager aICharacter)
    {

    }

}
