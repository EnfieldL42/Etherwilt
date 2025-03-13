using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    [SerializeField] PlayerManager player;

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
    public CharacterSaveData characterSlot1;
    //public CharacterSaveData characterSlot2;
    //public CharacterSaveData characterSlot3;
    //public CharacterSaveData characterSlot4;
    //public CharacterSaveData characterSlot5;
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
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
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

    private void DecideCharacterFileNameBasedOnCharacterSlotBeingUsed()
    {
        switch(currentCharacterSlotBeingUsed)
        {
            case CharacterSlot.CharacterSlot_01:
                saveFileName = "characterSlot_01";
                break;
            case CharacterSlot.CharacterSlot_02:
                saveFileName = "characterSlot_02";
                break;
            case CharacterSlot.CharacterSlot_03:
                saveFileName = "characterSlot_03";
                break;
            case CharacterSlot.CharacterSlot_04:
                saveFileName = "characterSlot_04";
                break;
            case CharacterSlot.CharacterSlot_05:
                saveFileName = "characterSlot_05";
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
    }

    public void CreateNewGame()
    {
        //create new file, name of file depends on which slot
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();

        currentCharacterData = new CharacterSaveData();
    }

    public void LoadGame()
    {
        //loading a previous file, with file name depending on which slot
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();

        saveFileDataWriter = new SaveFileDataWriter();
        //works on multiple machine types
        saveFileDataWriter.saveDataDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame()
    {
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();

        saveFileDataWriter = new SaveFileDataWriter();

        saveFileDataWriter.saveDataDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }


    public IEnumerator LoadWorldScene()
    {
        UnityEngine.AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(worldSceneIndex);
        yield return null;
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }

}
