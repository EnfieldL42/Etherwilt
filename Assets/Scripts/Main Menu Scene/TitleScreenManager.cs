using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Xml.Serialization;
using TMPro;

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


    [Header("Classes")]
    public CharacterClass[] startingClasses;


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

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);//cloase main

        titleScreenLoadMeu.SetActive(true);//open load

        loadMenuReturnButton.Select();//select the return button
    }

    public void CloseLoadGameMenu()
    {

        titleScreenLoadMeu.SetActive(false);//open load

        titleScreenMainMenu.SetActive(true);//cloase main


        mainMenuLoadGameButton.Select();//select the load button
    }

    public void OpenTitleScreenMainMenu()
    {
        titleScreenMainMenu.SetActive(true);
    }

    public void CloseTitleScreenMainMenu()
    {
        titleScreenMainMenu.SetActive(false);
    }

    public void OpenCharacterCreationMenu()
    {
        CloseTitleScreenMainMenu();
        titleScreenCharacterCreationMenu.SetActive(true);
    }

    public void CloseCharacterCreationMenu()
    {
        titleScreenCharacterCreationMenu.SetActive(false);
        OpenTitleScreenMainMenu();
    }

    public void OpenChooseCharacterClassSubMenu()
    {
        //disable main menu buttons
        ToggleCharacterCreationScreenMainMenuButtons(false);
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
        characterClassMenu.SetActive(false);
        characterClassButton.Select();
        characterClassButton.OnSelect(null);
    }

    public void OpenChooseCharacterNameSubMenu()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        ToggleCharacterCreationScreenMainMenuButtons(false);

        characterNameButton.gameObject.SetActive(false);
        characterNameMenu.SetActive(true);
        characterNameInputField.Select();
    }

    public void CloseChooseCharacterNameSubMenu()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        ToggleCharacterCreationScreenMainMenuButtons(true);


        characterNameButton.gameObject.SetActive(true);
        characterNameMenu.SetActive(false);

        characterNameButton.Select();

        player.playerNetworkManager.characterName.Value = characterNameInputField.text;
        characterNameText.text = characterNameInputField.text;
    }

    private void ToggleCharacterCreationScreenMainMenuButtons(bool status)
    {
        characterNameButton.enabled = status;
        characterClassButton.enabled = status;
        startGameButton.enabled = status;
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

    public void SetCharacterClass(PlayerManager player, int vitality, int endurance, int strength, int dexterity,
        WeaponItem[] mainHandWeapons, WeaponItem[] offHandWeapons, QuickSlotItem[] quickSlotItems)
    {
        //set stats
        player.playerNetworkManager.vitality.Value = vitality;
        player.playerNetworkManager.endurance.Value = endurance;
        player.playerNetworkManager.strength.Value = strength;
        player.playerNetworkManager.dexterity.Value = dexterity;

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
        player.playerEquipmentManager.LoadQuickSlotEquipment(player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex]); //refreshes the hud
    }

}
