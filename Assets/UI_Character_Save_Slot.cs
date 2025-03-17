using UnityEngine;
using TMPro;

public class UI_Character_Save_Slot : MonoBehaviour
{
    SaveFileDataWriter saveFileWriter;

    [Header("Game Slot")]
    public CharacterSlot characterSlot;

    [Header("Character Info")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timePlayed;

    private void OnEnable()
    {
        LoadSaveSlots();
    }

    private void LoadSaveSlots()
    {
        saveFileWriter = new SaveFileDataWriter();
        saveFileWriter.saveDataDataDirectoryPath = Application.persistentDataPath;

        //save slot 01

        if(characterSlot == CharacterSlot.CharacterSlot_01)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            //if it exists get information
            if(saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
            }
            else//if its doesnt, disable gameobject
            {
                gameObject.SetActive(false);
            }    
        }
        else if (characterSlot == CharacterSlot.CharacterSlot_02)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            //if it exists get information
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
            }
            else//if its doesnt, disable gameobject
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlot.CharacterSlot_03)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            //if it exists get information
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
            }
            else//if its doesnt, disable gameobject
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlot.CharacterSlot_04)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            //if it exists get information
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlot04.characterName;
            }
            else//if its doesnt, disable gameobject
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlot.CharacterSlot_05)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            //if it exists get information
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlot05.characterName;
            }
            else//if its doesnt, disable gameobject
            {
                gameObject.SetActive(false);
            }
        }


    }
}
