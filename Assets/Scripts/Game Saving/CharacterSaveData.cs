using UnityEngine;

[System.Serializable]
//reference for every save file, not a monobehavior
public class CharacterSaveData
{
    [Header("Character Name")]
    public string characterName;

    [Header("Time Played")]
    public float secondsPlayed;

    //cannot use a vector 3 as its not serializable, serializable can only use float int strin and bool
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;
}
