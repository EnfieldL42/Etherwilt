using UnityEngine;

public class AICharacterLocomotionManager : CharacterLocomotionManager
{
    AICharacterManager aiCharacter;

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aICharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navmeshAgent.transform.rotation;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (aiCharacter.IsOwner)
        {
            aiCharacter.characterNetworkManager.verticalMovement.Value = aiCharacter.animator.GetFloat("Vertical");
            aiCharacter.characterNetworkManager.horizontalMovement.Value = aiCharacter.animator.GetFloat("Horizontal");
        }
        else
        {
            aiCharacter.animator.SetFloat("Vertical", aiCharacter.aICharacterNetworkManager.verticalMovement.Value, 0.1f, Time.deltaTime);
            aiCharacter.animator.SetFloat("Horizontal", aiCharacter.aICharacterNetworkManager.horizontalMovement.Value, 0.1f, Time.deltaTime);
        }
    }

}
