using JetBrains.Annotations;
using System.Collections;
using System.Xml.Serialization;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [Header("Main Menu Menus")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject titleScreenLoadMeu;
    [SerializeField] GameObject titleScreenCharacterCreationMenu;

    [Header("Main Menu Buttons")]
    [SerializeField] Button mainMenuNewGameButton;
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button deleteCharacterPopUpConfirmButton;

    [Header("Main Menu Pop Ups")]
    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] GameObject deleteCharacterSlotPopUp;
    [SerializeField] GameObject noNamePopUp;
    [SerializeField] Button noNameOkayButton;
    [SerializeField] GameObject noClassPopUp;
    [SerializeField] Button noClassOkayButton;


    [Header("Save Slots")]
    public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

    [Header("Character Creation Main Panel Buttons")]
    [SerializeField] Button characterNameButton;
    [SerializeField] Button characterClassButton;
    [SerializeField] Button startGameButton;
    [SerializeField] TextMeshProUGUI characterNameText;
    [SerializeField] TextMeshProUGUI characterClassText;


    [Header("Character Creation Class Panel Buttons")]
    [SerializeField] Button[] characterClassButtons;


    [Header("Character Creation Secondary Panel Menus")]
    [SerializeField] GameObject characterClassMenu;
    [SerializeField] GameObject characterNameMenu;
    [SerializeField] TMP_InputField characterNameInputField;
    [SerializeField] GameObject characterClassDisplay;
    [SerializeField] CCStatsDisplay characterStatsDisplay;

    [Header("Classes")]
    public CharacterClass[] startingClasses;


    [Header("Audio Mixer")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider mixerSlider;

    [Header("UI keybinds")]
    [SerializeField] Image[] loadingScreenSubmitImage;
    [SerializeField] Image[] loadingScreenEscapeImage;

    [SerializeField] Sprite enterKeyboardSprite;
    [SerializeField] Sprite deleteKeyboardSprite;
    [SerializeField] Sprite enterXboxSprite;
    [SerializeField] Sprite deleteXboxSprite;
    //[SerializeField] Sprite enterPSSprite;
    //[SerializeField] Sprite deletePSSprite;


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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("mixerVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
        }

    }

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void AttemptToCreateNewCharacter()
    {
        if (WorldSaveGameManager.instance.HasFreeCharacterSlot())
        {
            OpenCharacterCreationMenu();
        }
        else
        {
            DisplayNoFreeCharacterCharacterSlotsPopUp();
        }
    }

    public void StartIntroCutscene()
    {
        if (string.IsNullOrEmpty(characterNameInputField.text))
        {
            DisplayNoNamePopUp();
        }

        else if (string.IsNullOrEmpty(characterClassText.text))
        {
            DisplayNoClassPopUp();
        }

        else
        {
            LoadIntroCutscene();
        }
    }

    public void LoadIntroCutscene()
    {
    PlayerUIManager.instance.playerUILoadingScreenManager.ActivateLoadingScreen();
    Time.timeScale = 0f; // Freeze the game
    NetworkManager.Singleton.SceneManager.LoadScene("IntroCutscene", LoadSceneMode.Single);
    PlayerUIManager.instance.playerUILoadingScreenManager.DeactivateLoadingScreen();
    Time.timeScale = 1f;
    PlayerCamera.instance.GetComponentInChildren<UniversalAdditionalCameraData>().SetRenderer(0);
    }
    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
    }
    IEnumerator ChangeCanvasGroup(GameObject menu, Button button)
    {
        yield return new WaitForSecondsRealtime(0.25f);
        menu.SetActive(true);
        button.Select();
        yield return null;
    }
    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.GetComponentInChildren<CanvasGroupFade>().FadeOut(); //cloase main

        //titleScreenLoadMeu.SetActive(true);//open load

        //loadMenuReturnButton.Select();//select the return button
        StartCoroutine(ChangeCanvasGroup(titleScreenLoadMeu, loadMenuReturnButton));
    }

    public void CloseLoadGameMenu()
    {

        titleScreenLoadMeu.GetComponentInChildren<CanvasGroupFade>().FadeOut();

        //titleScreenMainMenu.SetActive(true);//cloase main

       //mainMenuLoadGameButton.Select();//select the load button
        StartCoroutine(ChangeCanvasGroup(titleScreenMainMenu, mainMenuLoadGameButton));
    }

    /*
     * public void OpenTitleScreenMainMenu()
    {
        titleScreenMainMenu.SetActive(true);
    }

    public void CloseTitleScreenMainMenu()
    {
        titleScreenMainMenu.SetActive(false);
    }
    */
    public void OpenCharacterCreationMenu()
    {
        //CloseTitleScreenMainMenu();
        titleScreenMainMenu.GetComponentInChildren<CanvasGroupFade>().FadeOut();
        StartCoroutine(ChangeCanvasGroup(titleScreenCharacterCreationMenu, characterNameButton));
        //titleScreenCharacterCreationMenu.SetActive(true);
    }

    public void CloseCharacterCreationMenu()
    {
        titleScreenCharacterCreationMenu.GetComponentInChildren<CanvasGroupFade>().FadeOut(); //cloase main

        StartCoroutine(ChangeCanvasGroup(titleScreenMainMenu, mainMenuNewGameButton));

        //titleScreenCharacterCreationMenu.SetActive(false);
        //OpenTitleScreenMainMenu();
    }

    public void OpenChooseCharacterClassSubMenu()
    {
        //disable main menu buttons
        ToggleCharacterCreationScreenMainMenuButtons(false);
        characterNameMenu.SetActive(false) ;
        characterClassDisplay.SetActive(false);
        //enable sub menu object
        characterClassMenu.SetActive(true);
        //auto select first button

        if (characterClassButtons.Length > 0)
        {
            characterClassButtons[0].Select();
            characterClassButtons[0].OnSelect(null);
        }
        

    }

    public void CloseChooseCharacterClassSubMenu()
    {
        ToggleCharacterCreationScreenMainMenuButtons(true);
        characterNameMenu.SetActive(true);
        characterClassDisplay.SetActive(true);
        characterClassMenu.SetActive(false);
        characterClassButton.Select();
        characterClassButton.OnSelect(null);
    }

    public void OpenChooseCharacterNameSubMenu()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        ToggleCharacterCreationScreenMainMenuButtons(false);

        //characterNameButton.gameObject.SetActive(false);
        //characterNameMenu.SetActive(true);
        characterNameInputField.interactable = true;
        characterNameInputField.Select();
    }

    public void CloseChooseCharacterNameSubMenu()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        ToggleCharacterCreationScreenMainMenuButtons(true);


        //characterNameButton.gameObject.SetActive(true);
        //characterNameMenu.SetActive(false);
        characterNameInputField.interactable = false;
        characterNameButton.Select();

        player.playerNetworkManager.characterName.Value = characterNameInputField.text;
        //characterNameText.text = characterNameInputField.text;
    }

    private void ToggleCharacterCreationScreenMainMenuButtons(bool status)
    {
        characterNameButton.interactable = status;
        characterClassButton.interactable = status;
        startGameButton.interactable = status;
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

    //Character Creation
    public void SelectClass(int classID)
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        if (startingClasses.Length <= 0)
        {
            return;
        }

        startingClasses[classID].SetClass(player);
        characterClassText.text = startingClasses[classID].className.ToString();
        CloseChooseCharacterClassSubMenu();
    }

    public void PreviewClass(int classID)
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        if (startingClasses.Length <= 0)
        {
            return;
        }

        startingClasses[classID].SetClass(player);
    }

    public void SetCharacterClass(PlayerManager player, int vitality, int endurance, int strength, int dexterity, int weaponMastery, int magicMastery, int breakerMastery, int tankMastery,
        WeaponItem[] mainHandWeapons, WeaponItem[] offHandWeapons, QuickSlotItem[] quickSlotItems)
    {
        //set stats
        player.playerNetworkManager.health.Value = vitality;
        player.playerNetworkManager.endurance.Value = endurance;
        player.playerNetworkManager.strength.Value = strength;
        player.playerNetworkManager.dexterity.Value = dexterity;
        player.playerNetworkManager.weaponMastery.Value = weaponMastery;
        player.playerNetworkManager.magicMastery.Value = magicMastery;
        player.playerNetworkManager.breakerMastery.Value = breakerMastery;
        player.playerNetworkManager.tankMastery.Value = tankMastery;

        //set weapons
        player.playerInventoryManager.weaponsInRightHandSlots[0] = Instantiate(mainHandWeapons[0]);
        player.playerInventoryManager.weaponsInRightHandSlots[1] = Instantiate(mainHandWeapons[1]);
        player.playerInventoryManager.weaponsInRightHandSlots[2] = Instantiate(mainHandWeapons[2]);
        player.playerInventoryManager.currentRightHandWeapon = player.playerInventoryManager.weaponsInRightHandSlots[0];
        player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[0].itemID;

        if (player.playerInventoryManager.currentRightHandWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
        {
            player.playerInventoryManager.rightHandWeaponIndex = 0;
        }
        else
        {
            player.playerInventoryManager.rightHandWeaponIndex = -1;
        }

        player.playerInventoryManager.weaponsInLeftHandSlots[0] = Instantiate(offHandWeapons[0]);
        player.playerInventoryManager.weaponsInLeftHandSlots[1] = Instantiate(offHandWeapons[1]);
        player.playerInventoryManager.weaponsInLeftHandSlots[2] = Instantiate(offHandWeapons[2]);
        player.playerInventoryManager.currentLeftHandWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[0];
        player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponsInLeftHandSlots[0].itemID;

        if (player.playerInventoryManager.currentLeftHandWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
        {
            player.playerInventoryManager.leftHandWeaponIndex = 0;
        }
        else
        {
            player.playerInventoryManager.leftHandWeaponIndex = -1;
        }

        //set quick slots
        player.playerInventoryManager.quickSlotItemIndex = 0;

        if (quickSlotItems[0] != null)
        {
            player.playerInventoryManager.quickSlotItemsInQuickSlots[0] = Instantiate(quickSlotItems[0]);
        }
        if (quickSlotItems[1] != null)
        {
            player.playerInventoryManager.quickSlotItemsInQuickSlots[1] = Instantiate(quickSlotItems[1]);
        }
        if (quickSlotItems[2] != null)
        {
            player.playerInventoryManager.quickSlotItemsInQuickSlots[2] = Instantiate(quickSlotItems[2]);
        }

        player.playerInventoryManager.currentQuickSlotItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[0];
        player.playerEquipmentManager.LoadQuickSlotEquipment(player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex]); //refreshes the hud

        //update stats display on character creation menu
        characterStatsDisplay.UpdateStats(player);
    }

    public void DisplayNoNamePopUp()
    {
        noNamePopUp.SetActive(true);
        noNameOkayButton.Select();
    }

    public void CloseNoNamePopUp()
    {
        noNamePopUp.SetActive(false);
        OpenChooseCharacterNameSubMenu();
    }

    public void DisplayNoClassPopUp()
    {
        noClassPopUp.SetActive(true);
        noClassOkayButton.Select();
    }

    public void CloseNoClassPopUp()
    {
        noClassPopUp.SetActive(false);
        OpenChooseCharacterClassSubMenu();
    }

    public void SetMasterVolume()
    {
        float volume = Mathf.Clamp(mixerSlider.value, 0.0001f, 1f);
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("mixerVolume", volume);
    }

    private void LoadVolume()
    {
        mixerSlider.value = PlayerPrefs.GetFloat("mixerVolume");
        SetMasterVolume();
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            LoadVolume();
        }
    }

    public void ChangeMainMenuUIToKeyboard()
    {
        //enterKeyboardImage;
        //deleteKeyboardImage;

        foreach (Image img in loadingScreenSubmitImage)
        {
            img.sprite = enterKeyboardSprite;
        }

        foreach (Image img in loadingScreenEscapeImage)
        {
            img.sprite = deleteKeyboardSprite;
        }
    }

    public void ChangeMainMenuUIToXbox()
    {
        foreach (Image img in loadingScreenSubmitImage)
        {
            img.sprite = enterXboxSprite;
        }

        foreach (Image img in loadingScreenEscapeImage)
        {
            img.sprite = deleteXboxSprite;
        }
    }

    //public void ChangeUIToPlayStation()
    //{
    //    foreach (Image img in loadingScreenSubmitImage)
    //    {
    //        img.sprite = enterPSSprite;
    //    }

    //    foreach (Image img in loadingScreenEscapeImage)
    //    {
    //        img.sprite = deletePSSprite;
    //    }
    //}

    public void QuitGame()
    {
        Application.Quit();
    }
}

