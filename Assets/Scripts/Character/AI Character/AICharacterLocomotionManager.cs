using UnityEngine;

public class AICharacterLocomotionManager : CharacterLocomotionManager
{
    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        aiCharacter.transform.rotation = aiCharacter.navmeshAgent.transform.rotation;
    }
}
