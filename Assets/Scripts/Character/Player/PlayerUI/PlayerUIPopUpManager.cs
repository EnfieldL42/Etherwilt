using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("Message Pop up")]
    [SerializeField] TextMeshProUGUI popUpMessageText;
    [SerializeField] GameObject popUpMessageGameObject;

    [Header("Item Pop Up")]
    [SerializeField] GameObject itemPopUPGameObject;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemAmount;

    [Header("You DIED Pop up")]
    [SerializeField] GameObject youDiedPopUpGameObject;
    //[SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI youDiedPopUpText;
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;

    [Header("Boss Defeated Pop up")]
    [SerializeField] GameObject bossDefeatedPopUpGameObject;
    //[SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
    [SerializeField] CanvasGroup bossDefeatedCanvasGroup;

    [Header("Bonfire Restored Pop up")]
    [SerializeField] GameObject bonfireRestoredPopUpGameObject;
    //[SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI bonfireRestoredPopUpText;
    [SerializeField] CanvasGroup bonfireRestoredCanvasGroup;

    [Header("Dialogue Pop Up")]
    [SerializeField] GameObject dialoguedPopUpGameObject;
    [SerializeField] TextMeshProUGUI dialoguePopUpText;

    [SerializeField] CharacterDialogue currentDialogue;
    private Coroutine dialogueCoroutine;


    public void CloseAllPopUpWindows()
    {
        popUpMessageGameObject.SetActive(false);
        itemPopUPGameObject.SetActive(false);

        PlayerUIManager.instance.popUpWindowIsOpen = false;
    }

    public void SendPlayerMessagePopUp(string messageText)
    {
        PlayerUIManager.instance.popUpWindowIsOpen = true;
        popUpMessageText.text = messageText;
        popUpMessageGameObject.SetActive(true);
    }

    public void SentItemPopUp(Item item, int amount)
    {
        itemAmount.enabled = false;
        itemIcon.sprite = item.itemIcon;
        itemName.text = item.name;

        if(amount > 1)
        {
            itemAmount.enabled = true;
            itemAmount.text = "x" + amount.ToString();
        }

        itemPopUPGameObject.SetActive(true);
        PlayerUIManager.instance.popUpWindowIsOpen = true;
    }

    public void SendYouDiePopUp()
    {
        //actuvate post processing effects

        youDiedPopUpGameObject.SetActive(true);
        //youDiedPopUpBackgroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpText, 8, 10));
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));

        //stretch our the pop up
        //fade in the pop up
        //wait, then dafe out the pop up


    }

    public void SendBossDefeatedPopUp(string bossDefeatedMessage)
    {
        //actuvate post processing effects

        bossDefeatedPopUpText.text = bossDefeatedMessage;
        bossDefeatedPopUpGameObject.SetActive(true);
        //youDiedPopUpBackgroundText.characterSpacing = 0;
        WorldSoundFXManager.instance.PlayBossDefeatedSound();
        StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpText, 8, 10));
        StartCoroutine(FadeInPopUpOverTime(bossDefeatedCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedCanvasGroup, 2, 5));

        //stretch our the pop up
        //fade in the pop up
        //wait, then dafe out the pop up
    }

    public void SendBonfireRestoredDefeatedPopUp(string bonfireRestoredMessage)
    {
        //actuvate post processing effects

        bonfireRestoredPopUpText.text = bonfireRestoredMessage;
        bonfireRestoredPopUpGameObject.SetActive(true);
        //youDiedPopUpBackgroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUpTextOverTime(bonfireRestoredPopUpText, 8, 10));
        StartCoroutine(FadeInPopUpOverTime(bonfireRestoredCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(bonfireRestoredCanvasGroup, 2, 5));

        //stretch our the pop up
        //fade in the pop up
        //wait, then dafe out the pop up
    }

    public void SendDialoguePopUp(CharacterDialogue dialogue, AICharacterManager aICharacter)
    {
        PlayerUIManager.instance.playerUIHudManager.ToggleHUDWithoutPopUps(false);
        currentDialogue = dialogue;

        if(dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        PlayerUIManager.instance.popUpWindowIsOpen = true;

        dialogueCoroutine = StartCoroutine(dialogue.PlayerDialogueCoroutine(aICharacter));
    }

    public void SendNextDialoguePopUpInIndex(CharacterDialogue dialogue, AICharacterManager aiCharacter)
    {
        currentDialogue = dialogue;

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        if (aiCharacter.aICharacterSoundFXManager.dialogueIsPlaying)
        {
            aiCharacter.aICharacterSoundFXManager.audioSource.Stop();
        }

        //Close any open popups
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        PlayerUIManager.instance.popUpWindowIsOpen = true;

        currentDialogue.dialogueIndex++;
        dialogueCoroutine = StartCoroutine(dialogue.PlayerDialogueCoroutine(aiCharacter));
    }

    public void SetDialoguePopUpSubtitles(string dialogueText)
    {
        dialoguedPopUpGameObject.SetActive(true);
        dialoguePopUpText.text = dialogueText;
    }

    public void EndDialoguePopUp()
    {
        dialoguedPopUpGameObject.SetActive(false);
        PlayerUIManager.instance.playerUIHudManager.ToggleHUDWithoutPopUps(true);
    }

    public void CancelDialoguePopUp(AICharacterManager aiCharacter)
    {
        PlayerUIManager.instance.playerUIHudManager.ToggleHUDWithoutPopUps(true);

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        if (aiCharacter.aICharacterSoundFXManager.audioSource.isPlaying)
        {
            aiCharacter.aICharacterSoundFXManager.audioSource.Stop();
        }

        dialoguedPopUpGameObject.SetActive(false);
        currentDialogue.OnDialogueCancelled(aiCharacter);
    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
    {
        if(duration > 0f)
        {
            text.characterSpacing = 0;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        if(duration > 0)
        {
            canvas.alpha = 0;
            float timer = 0;

            yield return null;
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha,1, duration * Time.deltaTime);
                yield return null;
            }

            canvas.alpha = 1;


            yield return null;
        }
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
    {
        if (duration > 0)
        {
            while(delay > 0)
            {
                delay -= Time.deltaTime;
                yield return null;
            }

            canvas.alpha = 1;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                yield return null;

            }

            canvas.alpha = 0;


            yield return null;
        }
    }

}
