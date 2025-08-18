using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.AI;


[CreateAssetMenu(menuName = "A.I/States/Idle")]
public class IdleState : AIState
{
    [Header("Idle State Options")]
    [SerializeField] IdleStateMode idleState;

    [Header("Patrol Options")]
    public AIPatrolPath aiPatrolPath; 
    [SerializeField] bool hasFoundClosestPointNearCharacterSpawn = false; // If true, the AI will find the closest patrol point instead of the first patrol point
    [SerializeField] bool patrolComplete = false;
    [SerializeField] bool repeatPatrol = false;
    [SerializeField] int patrolDestinationIdex; //which patrol point is the ai going to
    [SerializeField] bool hasPatrolDestination = false; //does the ai have a next patrol to go to
    [SerializeField] Vector3 currentPatrolDestination; //destination coords ai is going to
    [SerializeField] float distanceFromCurrentDestination; //distance from ai to patrol destination
    [SerializeField] float timeBetweenPatrol = 15; //time between patrol points
    [SerializeField] float restTimer = 0; //timer for time between patrol points

    [Header("Sleep Options")]
    public bool willInvestigateSound = false; //if true, the AI will investigate sounds

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        aiCharacter.aICharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);


        switch (idleState)
        {
            case IdleStateMode.Idle:
                return Idle(aiCharacter);
            case IdleStateMode.Patrol:
                return Patrol(aiCharacter);
            default:
                break;
        }

        return this;
    }

    protected virtual AIState Idle(AICharacterManager aICharacter)
    {
        if (aICharacter.characterCombatManager.currentTarget != null)
        {
            return SwitchState(aICharacter, aICharacter.pursueState);
        }
        else
        {
            aICharacter.navmeshAgent.enabled = false;
            //return this state,
            return this;
        }
    }

    protected virtual AIState Patrol(AICharacterManager aICharacter)
    {
        if (!aICharacter.isGrounded)
        {
            return this;
        }

        if (aICharacter.isPerformingAction)
        {
            aICharacter.navmeshAgent.enabled = false;
            aICharacter.characterNetworkManager.isMoving.Value = false;
            return this;
        }

        if (!aICharacter.navmeshAgent.enabled) 
        {
            aICharacter.navmeshAgent.enabled = true;
        }

        if(aICharacter.aICharacterCombatManager.currentTarget != null)
        {
            return SwitchState(aICharacter, aICharacter.pursueState);
        }

        if (patrolComplete && repeatPatrol)
        {
            //stop and wait if resting
            if (timeBetweenPatrol > restTimer)
            {
                aICharacter.navmeshAgent.enabled = false;
                aICharacter.characterNetworkManager.isMoving.Value = false;
                restTimer += Time.deltaTime;
            }
            else
            {
                patrolDestinationIdex = -1;
                hasPatrolDestination = false;
                currentPatrolDestination = aICharacter.transform.position;
                patrolComplete = false;
                restTimer = 0;
            }
        }
        else if (patrolComplete && !repeatPatrol)
        {
            aICharacter.navmeshAgent.enabled = false;
            aICharacter.characterNetworkManager.isMoving.Value = false;
        }

        if (hasPatrolDestination)
        {
            distanceFromCurrentDestination = Vector3.Distance(aICharacter.transform.position, currentPatrolDestination);

            if (distanceFromCurrentDestination > 2)//this checks if ai is close enough to patrol point, without the check it would just keep rotating aournd the patrol point and never getting there
            {
                aICharacter.navmeshAgent.enabled = true;
                aICharacter.aICharacterLocomotionManager.RotateTowardsAgent(aICharacter);
            }
            else
            {
                currentPatrolDestination = aICharacter.transform.position;
                hasPatrolDestination = false;
            }
        }
        else
        {
            patrolDestinationIdex += 1;

            if (patrolDestinationIdex > aiPatrolPath.patrolPoints.Count - 1)
            {
                patrolComplete = true;
                return this;
            }

            if (!hasFoundClosestPointNearCharacterSpawn)
            {
                hasFoundClosestPointNearCharacterSpawn = true;
                float closestDistance = Mathf.Infinity;

                for (int i = 0; i < aiPatrolPath.patrolPoints.Count; i++)
                {
                    float distanceToPoint = Vector3.Distance(aICharacter.transform.position, aiPatrolPath.patrolPoints[i]);

                    if (distanceToPoint < closestDistance)
                    {
                        closestDistance = distanceToPoint;
                        patrolDestinationIdex = i;
                        currentPatrolDestination = aiPatrolPath.patrolPoints[i];
                    }
                }
            }
            else
            {
                currentPatrolDestination = aiPatrolPath.patrolPoints[patrolDestinationIdex];
            }

            hasPatrolDestination = true;
        }

        NavMeshPath path = new NavMeshPath();
        aICharacter.navmeshAgent.CalculatePath(currentPatrolDestination, path);
        aICharacter.navmeshAgent.SetPath(path);

        return this;
    }
}
