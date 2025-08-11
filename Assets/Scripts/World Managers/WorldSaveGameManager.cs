using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;
    public PlayerManager player;

    [Header("SAVE/LOAD")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("Curent Character Data")]
    public CharacterSlot currentCharacterSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;

    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;
    public CharacterSaveData characterSlot04;
    public CharacterSaveData characterSlot05;
    //public CharacterSaveData characterSlot6;
    //public CharacterSaveData characterSlot7;
    //public CharacterSaveData characterSlot8;
    //public CharacterSaveData characterSlot9;
    //public CharacterSaveData characterSlot10;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        LoadAllCharacterProfiles();
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }
        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    public bool HasFreeCharacterSlot()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDataDirectoryPath = Application.persistentDataPath;


        //check if we can create a new save file
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            return true;
        }

        //check if we can create a new save file
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            return true;
        }

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            return true;
        }

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            return true;
        }

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            return true;
        }

        return false;
    }

    public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot characterSlot)
    {
        string fileName = "";

        switch(characterSlot)
        {
            case CharacterSlot.CharacterSlot_01:
                fileName = "characterSlot_01";
                break;
            case CharacterSlot.CharacterSlot_02:
                fileName = "characterSlot_02";
                break;
            case CharacterSlot.CharacterSlot_03:
                fileName = "characterSlot_03";
                break;
            case CharacterSlot.CharacterSlot_04:
                fileName = "characterSlot_04";
                break;
            case CharacterSlot.CharacterSlot_05:
                fileName = "characterSlot_05";
                break;
            //case CharacterSlot.CharacterSlot_06:
            //    fileName = "characterSlot_06";
            //    break;
            //case CharacterSlot.CharacterSlot_07:
            //    fileName = "characterSlot_07";
            //    break;
            //case CharacterSlot.CharacterSlot_08:
            //    fileName = "characterSlot_08";
            //    break;
            //case CharacterSlot.CharacterSlot_09:
            //    fileName = "characterSlot_09";
            //    break;
            //case CharacterSlot.CharacterSlot_10:
            //    fileName = "characterSlot_10";
            //    break;
        }

        return fileName;
    }

    public void AttemptToCreateNewGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDataDirectoryPath = Application.persistentDataPath;


        //check if we can create a new save file
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            //if this profile slot is not taken, make a new one using this slot
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_01;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        //check if we can create a new save file
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            //if this profile slot is not taken, make a new one using this slot
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_02;
            currentCharacterData = new CharacterSaveData();

            NewGame();
            return;
        }

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            //if this profile slot is not taken, make a new one using this slot
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_03;
            currentCharacterData = new CharacterSaveData();

            NewGame();
            return;
        }

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            //if this profile slot is not taken, make a new one using this slot
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_04;
            currentCharacterData = new CharacterSaveData();

            NewGame();
            return;
        }

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            //if this profile slot is not taken, make a new one using this slot
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_05;
            currentCharacterData = new CharacterSaveData();

            NewGame();
            return;
        }



        //if there are no free slots, notify the player
        TitleScreenManager.instance.DisplayNoFreeCharacterCharacterSlotsPopUp();
    }

    private void NewGame()
    {
        player.playerNetworkManager.vitality.Value = 10;
        player.playerNetworkManager.endurance.Value = 10;

        SaveGame();
        LoadWorldScene(worldSceneIndex);
    }

    public void LoadGame()
    {
        //loading a previous file, with file name depending on which slot
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        //works on multiple machine types
        saveFileDataWriter.saveDataDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();
        PlayerUIManager.instance.LockMouse();


        LoadWorldScene(worldSceneIndex);
    }

    public void SaveGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();

        saveFileDataWriter.saveDataDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public void DeleteGame(CharacterSlot characterSlot)
    {
        //chooose file based on name

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

        saveFileDataWriter.DeleteSaveFile();


    }

    private void LoadAllCharacterProfiles()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();


        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
        characterSlot02 = saveFileDataWriter.LoadSaveFile();


        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
        characterSlot03 = saveFileDataWriter.LoadSaveFile();


        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
        characterSlot04 = saveFileDataWriter.LoadSaveFile();


        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
        characterSlot05 = saveFileDataWriter.LoadSaveFile();

    }

    //public void LoadWorldScene(int buildIndex)
    //{
    //    string worldScene = SceneUtility.GetScenePathByBuildIndex(buildIndex);
    //    NetworkManager.Singleton.SceneManager.LoadScene(worldScene, LoadSceneMode.Single);
    //    //UnityEngine.AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(worldSceneIndex);

    //    //can use this if we want to save different scenes
    //    //UnityEngine.AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);

    //    player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

    //}

    public void LoadWorldScene(int buildIndex)
    {
        PlayerUIManager.instance.playerUILoadingScreenManager.ActivateLoadingScreen();
        Time.timeScale = 0f; // Freeze the game

        string worldScene = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        NetworkManager.Singleton.SceneManager.LoadScene(worldScene, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;

        // Load character data
        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

        // Unfreeze the game
        Time.timeScale = 1f;
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }


    public SerializableWeapon GetSerializableWeaponFromWeaponItem(WeaponItem weapon)
    {
        SerializableWeapon serializedWeapon = new SerializableWeapon();

        serializedWeapon.itemID = weapon.itemID;

        return serializedWeapon;

    }

    //public FlaskItem GetSerializableFlaskFromFlaskItem(FlaskItem flask)
    //{
    //    serializableFlask serializedWeapon = new SerializableWeapon();

    //    serializedWeapon.itemID = weapon.itemID;

    //    return serializedWeapon;

    //}

    public SerializableQuickSlotItem GetSerializableQuickSlotItemFromQuickSlotItem(QuickSlotItem quickSlotItem)
    {
        SerializableQuickSlotItem serializedQuickSlotItem = new SerializableQuickSlotItem();

        if (quickSlotItem != null)
        {
            serializedQuickSlotItem.itemID = quickSlotItem.itemID;
            serializedQuickSlotItem.itemAmount = quickSlotItem.itemAmount;
        }
        else
        {
            serializedQuickSlotItem.itemID = -1;
        }

        return serializedQuickSlotItem;

    }


}
