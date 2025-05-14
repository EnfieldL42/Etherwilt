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
    [SerializeField] GameObject[] aiCharacters;
    [SerializeField] List<GameObject> spawnedInCharacters;

    [Header("Debug")]
    [SerializeField] bool despawnCharacters = false;
    [SerializeField] bool spawnCharacters = false;

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
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(WaitForSceneToLoadThenSpawnCharacters());
        }
    }

    private void Update()
    {
        Debug();
    }

    private IEnumerator WaitForSceneToLoadThenSpawnCharacters()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }

        SpawnAllCharacters();

    }

    private void SpawnAllCharacters()
    {
        foreach (var character in aiCharacters)
        {
            GameObject instantiatedCharacter = Instantiate(character);
            instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
            spawnedInCharacters.Add(instantiatedCharacter);
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

    private void Debug()
    {
        if(despawnCharacters)
        {
            despawnCharacters = false;
            DespawnAllCharacters();
        }

        if(spawnCharacters)
        {
            spawnCharacters = false;
            SpawnAllCharacters();
        }
    }


}
