using Unity.AI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
public class PursueTargetState : AIState
{
    [Header("Flags")]
    public bool canPivot = false;
    [SerializeField] bool canStopChasing = true;


    public override AIState Tick(AICharacterManager aiCharacter)
    {
        //check if we are performing an action
        if (aiCharacter.isPerformingAction)
        {
            aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0);
            return this;
        }

        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 1);

        //check if target is null, if we dont have a target, return to idle state
        if (aiCharacter.aICharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        // --- 3. Check distance from target
        float distance = Vector3.Distance(aiCharacter.transform.position, aiCharacter.aICharacterCombatManager.currentTarget.transform.position);
        aiCharacter.aICharacterCombatManager.distanceFromTarget = distance;


        if (distance > aiCharacter.aICharacterCombatManager.detectionRadius && canStopChasing)
        {
            aiCharacter.aICharacterCombatManager.SetTarget(null);
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        //make sure our navmesh agent is active, if its not enable it
        if (!aiCharacter.navmeshAgent.enabled)
        {
            aiCharacter.navmeshAgent.enabled = true;
        }

        //aiCharacter.navmeshAgent.stoppingDistance = aiCharacter.combatState.maximumEngagementDistance;

        if(canPivot)
        {
            if (aiCharacter.aICharacterCombatManager.viewableAngle < aiCharacter.aICharacterCombatManager.minimumFOV || aiCharacter.aICharacterCombatManager.viewableAngle > aiCharacter.aICharacterCombatManager.maximumFOV)
            {
                aiCharacter.aICharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        aiCharacter.aICharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        if(aiCharacter.aICharacterCombatManager.distanceFromTarget <= aiCharacter.navmeshAgent.stoppingDistance)
        {
            return SwitchState(aiCharacter, aiCharacter.combatState);
        }

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
