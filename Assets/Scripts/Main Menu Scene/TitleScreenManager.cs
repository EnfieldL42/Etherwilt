using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
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

    [Header("Pop Ups")]
    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] Button noCharacterSlotsOkayButton;

    private void Awake()
    {
        if(instance == null)
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

}
