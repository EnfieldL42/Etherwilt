using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Loading")]
    public bool isPerformingLoadingOperation;

    [Header("Characters")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
    [SerializeField] List<AICharacterManager> spawnedInCharacters;
    private Coroutine spawnAllCharactersCoroutine;
    private Coroutine despawnAllCharactersCoroutine;
    private Coroutine resetAllCharactersCoroutine;

    [Header("Bosses")]
    [SerializeField] List<AIBossCharacterManager> spawnedInBosses;

    [Header("Patrol Paths")]
    [SerializeField] List<AIPatrolPath> aIPatrolPaths = new List<AIPatrolPath>();


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

    //if we have more than 25 enemies per area, this function will stutter the game -> instead reset their stats/animations
    public void SpawnAllCharacters()
    {
        isPerformingLoadingOperation = true;

        if (spawnAllCharactersCoroutine != null)
        {
            StopCoroutine(spawnAllCharactersCoroutine);
        }

        spawnAllCharactersCoroutine = StartCoroutine(SpawnAllCharactersCoroutine());
    }

    private IEnumerator SpawnAllCharactersCoroutine()
    {
        for (int i = 0; i < aiCharacterSpawners.Count; i++)
        {
            yield return new WaitForFixedUpdate();

            aiCharacterSpawners[i].AttemptToSpawnCharacter();

            yield return null;
        }

        isPerformingLoadingOperation = false;
        yield return null;
    }

    public void ResetAllCharacters()
    {
        isPerformingLoadingOperation = true;

        if (resetAllCharactersCoroutine != null)
        {
            StopCoroutine(resetAllCharactersCoroutine);
        }

        resetAllCharactersCoroutine = StartCoroutine(ResetAllCharactersCoroutine());
    }

    public IEnumerator ResetAllCharactersCoroutine()
    {
        for (int i = 0; i < aiCharacterSpawners.Count; i++)
        {
            yield return new WaitForFixedUpdate();

            aiCharacterSpawners[i].ResetCharacter();

            yield return null;
        }

        isPerformingLoadingOperation = false;
        yield return null;
    }

    private void DespawnAllCharacters()
    {
        isPerformingLoadingOperation = true;

        if (despawnAllCharactersCoroutine != null)
        {
            StopCoroutine(despawnAllCharactersCoroutine);
        }

        despawnAllCharactersCoroutine = StartCoroutine(DespawnAllCharactersCoroutine());

    }

    private IEnumerator DespawnAllCharactersCoroutine()
    {
        for (int i = 0; i < spawnedInCharacters.Count; i++)
        {
            yield return new WaitForFixedUpdate();

            spawnedInCharacters[i].GetComponent<NetworkObject>().Despawn();

            yield return null;
        }

        spawnedInCharacters.Clear();

        isPerformingLoadingOperation = false;
        yield return null;
    }

    private void DisableAllCharacters()
    {
        //TODO disable characters gameobjects, sync disables statuus on network
        //disable gamebject for clients upon connection, if disabled status is true
        //can be used to disable characters that are far from players to save memory
        //characters can be split into areas
    }

    public void DisableAllBossFights()
    {
        for(int i = 0; i < spawnedInBosses.Count; i++)
        {
            if(spawnedInBosses[i] == null)
            {
                continue;
            }

            spawnedInBosses[i].bossFightIsActive.Value = false;
        }
    }

    //PATROL PATHS
    public void AddPatrolPathToList(AIPatrolPath patrolPath)
    {
        if (aIPatrolPaths.Contains(patrolPath))
        {
            return;
        }

        aIPatrolPaths.Add(patrolPath);
    }

    public AIPatrolPath GetAIPatrolPathByID(int patrolPathID)
    {
        AIPatrolPath patrolPath = null;

        for (int i = 0; i < aIPatrolPaths.Count; i++)
        {
            if (aIPatrolPaths[i].patrolPathID == patrolPathID)
            {
                patrolPath = aIPatrolPaths[i];
            }
        }

        return patrolPath;
    }


}
