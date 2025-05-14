using UnityEngine;
using Unity.AI;
using UnityEngine.AI;
[CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
public class PursueTargetState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        //check if we are performing an action
        if (aiCharacter.isPerformingAction)
        {
            return this;
        }

        //check if target is null, if we dont have a target, return to idle state
        if(aiCharacter.aICharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        //make sure our navmesh agent is active, if its not enable it
        if(!aiCharacter.navmeshAgent.enabled)
        {
            aiCharacter.navmeshAgent.enabled = true;
        }

        aiCharacter.aICharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        //if we are within combat range, switch state to combat stance state

        //if the target is not reachable, or they ai is too far away, return home

        //pursue the target
        //OPTION 1 - might not work sometimes but its better for performance
        //aiCharacter.navmeshAgent.SetDestination(aiCharacter.aICharacterCombatManager.currentTarget.transform.position);

        //OPTION 2
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navmeshAgent.CalculatePath(aiCharacter.aICharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navmeshAgent.SetPath(path);

        return this;
    }
}
