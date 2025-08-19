using UnityEngine;
using Unity.Netcode;

public class AICharacterSpawner : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] GameObject characterGameObject;
    [SerializeField] GameObject instantiatedGameObject;
    private AICharacterManager aiCharacter;

    [Header("Patrol")]
    [SerializeField] bool hasPatrolPath = false;
    [SerializeField] int patrolPathID = 0;

    private void Awake()
    {

    }


    private void Start()
    {
        WorldAIManager.instance.SpawnCharacter(this);
        gameObject.SetActive(false);
    }

    public void AttemptToSpawnCharacter()
    {
        if(characterGameObject != null)
        {
            instantiatedGameObject = Instantiate(characterGameObject);
            instantiatedGameObject.transform.position = transform.position;
            instantiatedGameObject.transform.rotation = transform.rotation;
            instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
            aiCharacter = instantiatedGameObject.GetComponent<AICharacterManager>();
            WorldAIManager.instance.AddCharacterToSpawnCharacterList(instantiatedGameObject.GetComponent<AICharacterManager>());

            if (aiCharacter == null)
            {
                return;
            }

            WorldAIManager.instance.AddCharacterToSpawnCharacterList(aiCharacter);

            if (hasPatrolPath)
            {
                aiCharacter.idle.aiPatrolPath = WorldAIManager.instance.GetAIPatrolPathByID(patrolPathID);
            }


        }
    }
    public void ResetCharacter()
    {
        if(instantiatedGameObject == null)
        {
            return;
        }
        if (aiCharacter == null)
        {
            return;
        }

        instantiatedGameObject.transform.position = transform.position;
        instantiatedGameObject.transform.rotation = transform.rotation;
        aiCharacter.aICharacterNetworkManager.currentHealth.Value = aiCharacter.aICharacterNetworkManager.maxHealth.Value;
        aiCharacter.aICharacterCombatManager.SetTarget(null);
        //aiCharacter.characterNetworkManager.isMoving.Value = false;

        if (aiCharacter.isDead.Value)
        {
            aiCharacter.isDead.Value = false;
            aiCharacter.characterNetworkManager.isActive.Value = true;
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Empty", false, false, true, true, true, true);
            aiCharacter.currentState.SwitchState(aiCharacter, aiCharacter.idle);
        }

        aiCharacter.characterUIManager.ResetCharacterHPBar();

        if (aiCharacter is AIBossCharacterManager)
        {
            AIBossCharacterManager boss = aiCharacter as AIBossCharacterManager;
            //boss.aICharacterNetworkManager.isAwake.Value = false;
            //boss.sleepState.hasBeenAwakened = boss.hasBeenAwakened.Value;
            //boss.currentState = boss.currentState.SwitchState(boss, boss.sleepState);
        }

    }

}
