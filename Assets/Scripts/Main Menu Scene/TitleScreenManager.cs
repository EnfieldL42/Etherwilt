using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using JetBrains.Annotations;
public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [Header("Menus")]
    [SerializeField] GameObject titleScreenMainMeu;
    [SerializeField] GameObject titleScreenLoadMeu;

    [Header("Buttons")]
    [SerializeField] Button mainMenuNewGameButton;
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button deleteCharacterPopUpConfirmButton;

    [Header("Pop Ups")]
    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] GameObject deleteCharacterSlotPopUp;


    [Header("Save Slots")]
    public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();

    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMeu.SetActive(false);//cloase main

        titleScreenLoadMeu.SetActive(true);//open load

        loadMenuReturnButton.Select();//select the return button
    }

    public void CloseLoadGameMenu()
    {

        titleScreenLoadMeu.SetActive(false);//open load

        titleScreenMainMeu.SetActive(true);//cloase main


        mainMenuLoadGameButton.Select();//select the load button
    }

    public void DisplayNoFreeCharacterCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(true);
        noCharacterSlotsOkayButton.Select();
    }

    public void CloseNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(false);
        mainMenuNewGameButton.Select();
    }

    //Character slots
    public void SelectCharacterSlot(CharacterSlot characterSlot)
    {
        currentSelectedSlot = characterSlot;
    }

    public void SelectNoSlot()
    {
        currentSelectedSlot = CharacterSlot.NO_SLOT;
    }

    public void AttemptToDeleteCharacterSlot()
    {
        if (currentSelectedSlot != CharacterSlot.NO_SLOT)
        {
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterPopUpConfirmButton.Select();

        }

    }

    public void DeleteCharacterSlot()
    {
        deleteCharacterSlotPopUp.SetActive(true);
        WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);
        //disable and enable to refresh slots
        titleScreenLoadMeu.SetActive(false);
        titleScreenLoadMeu.SetActive(true);

        loadMenuReturnButton.Select();

    }

    public void CloseDeleteCharacterPopUp()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        loadMenuReturnButton.Select();
    }

}
