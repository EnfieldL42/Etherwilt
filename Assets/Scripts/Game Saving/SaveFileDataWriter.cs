using UnityEngine;
using System;
using System.IO;
using System.Linq.Expressions;


public class SaveFileDataWriter
{
    public string saveDataDataDirectoryPath = "";
    public string saveFileName = "";



    public bool CheckToSeeIfFileExists()
    {
        if (File.Exists(Path.Combine(saveDataDataDirectoryPath, saveFileName)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDataDirectoryPath,saveFileName));
    }

    public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
    {
        string savePath = Path.Combine(saveDataDataDirectoryPath, saveFileName);

        try
        {
            //creates a directory for the file unless it already has one
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("CREATING SAVE FILE, AT SAVE PATH: " + savePath);


            //serialize the data into a json
            string dataToStore = JsonUtility.ToJson(characterData, true);

            //write the file to the system
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(dataToStore);
                }
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR WHILST TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED" + savePath + "\n" + ex);
        }

    }

    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterData = null;

        string loadPath = Path.Combine(saveDataDataDirectoryPath, saveFileName);


        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.Log("FILE IS BLANK" + ex);
            }
        }

        return characterData;



    }


}
