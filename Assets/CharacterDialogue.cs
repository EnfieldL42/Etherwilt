using UnityEngine;
using System.Collections.Generic;
using System.Collections;


[CreateAssetMenu(menuName = "A.I/Dialogue")]
public class CharacterDialogue : ScriptableObject
{
    [Header("Greeting Dialogue")]
    [TextArea] public List<string> greetingDialogueString = new List<string>();
    public List<AudioClip> greetingDialogueAudio = new List<AudioClip>();
    private bool greetingHasPlayed = false;

    [Header("Core Dialogue")]
    [TextArea] public List<string> dialogueString = new List<string>();
    public List<AudioClip> dialogueAudio = new List<AudioClip>();
    public int dialogueIndex = 0;

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
            Debug.Log("AUDIO CLIP COUNT DOES NOT MATCH DIALOGUE STRING COUNT ON " + aICharacter.characterName);
            return;
        }

        aICharacter.aICharacterSoundFXManager.dialogueIsPlaying = true;
        PlayerUIManager.instance.playerUIPopUpManager.BeginDialoguePopUp(this, aICharacter);
    }

    public IEnumerator PlayerDialogueCoroutine(AICharacterManager aICharacter)
    {
        yield return null;
    }

    public void OnDialogueEnded(AICharacterManager aICharacter)
    {

    }
    public void OnDialogueCancelled(AICharacterManager aICharacter)
    {

    }

}
