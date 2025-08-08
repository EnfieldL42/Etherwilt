using UnityEngine;
using Unity.Netcode;

public class AICharacterSpawner : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] GameObject characterGameObject;
    [SerializeField] GameObject instantiatedGameObject;
    private AICharacterManager aiCharacter;

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

            if (aiCharacter != null)
            {
                WorldAIManager.instance.AddCharacterToSpawnCharacterList(aiCharacter);
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

        if (aiCharacter.isDead.Value)
        {
            aiCharacter.isDead.Value = false;
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Empty", false, false, true, true, true, true);
        }

        aiCharacter.characterUIManager.ResetCharacterHPBar();
    }

}
