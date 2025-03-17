using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class TitleScreenManager : MonoBehaviour
{

    [Header("Menus")]
    [SerializeField] GameObject titleScreenMainMeu;
    [SerializeField] GameObject titleScreenLoadMeu;

    [Header("Buttons")]
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuLoadGameButton;

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.CreateNewGame();

        StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
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

}
