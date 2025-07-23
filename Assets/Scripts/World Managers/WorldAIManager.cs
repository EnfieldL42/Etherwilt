using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Characters")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
    [SerializeField] List<AICharacterManager> spawnedInCharacters;

    [Header("Bosses")]
    [SerializeField] List<AIBossCharacterManager> spawnedInBosses;


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

    public void AddCharacterToSpawnCharacterList(AICharacterManager character)
    {
        if(spawnedInCharacters.Contains(character))
        {
            return;
        }
        spawnedInCharacters.Add(character);

        AIBossCharacterManager bossCharacter = character as AIBossCharacterManager;

        if(bossCharacter != null)
        {
            if(spawnedInBosses.Contains(bossCharacter))
            {
                return;
            }

            spawnedInBosses.Add(bossCharacter);
        }
    }

    public AIBossCharacterManager GetBossCharacterByID(int ID)
    {
        return spawnedInBosses.FirstOrDefault(boss => boss.bossID == ID);
    }

    public void ResetAllCharacters()
    {
        DespawnAllCharacters();

        foreach(var spawner in aiCharacterSpawners)
        {
            spawner.AttemptToSpawnCharacter();
        }

    }

    private void DespawnAllCharacters()
    {
        foreach(var character in spawnedInCharacters)
        {
            character.GetComponent<NetworkObject>().Despawn();
        }
        spawnedInCharacters.Clear();
    }

    private void DisableAllCharacters()
    {
        //TODO disable characters gameobjects, sync disables statuus on network
        //disable gamebject for clients upon connection, if disabled status is true
        //can be used to disable characters that are far from players to save memory
        //characters can be split into areas
    }



}
