using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;


public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Characters")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
    [SerializeField] List<GameObject> spawnedInCharacters;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            aiCharacterSpawners.Add(aiCharacterSpawner);
            aiCharacterSpawner.AttemptToSpawnCharacter();
        }

    }

    private void DespawnAllCharacters()
    {
        foreach(var character in spawnedInCharacters)
        {
            character.GetComponent<NetworkObject>().Despawn();
        }

    }

    private void DisableAllCharacters()
    {
        //TODO disable characters gameobjects, sync disables statuus on network
        //disable gamebject for clients upon connection, if disabled status is true
        //can be used to disable characters that are far from players to save memory
        //characters can be split into areas
    }



}
