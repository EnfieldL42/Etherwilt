using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "A.I/States/Investigate Sound")]
public class InvestigateSoundState : AIState
{
    [Header("Flags")]
    [SerializeField] bool destinationSet = false;
    [SerializeField] bool destinationReached = false;

    [Header("Position")]
    public Vector3 positionOfSound = Vector3.zero;

    [Header("Investigation TImer")]
    [SerializeField] float investigationTime = 3;
    [SerializeField] float investigationTimer = 0;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0.5f);

        if ((aiCharacter.isPerformingAction))
        {
            return this;
        }

        aiCharacter.aICharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);

        if(aiCharacter.aICharacterCombatManager.currentTarget != null)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueState);
        }

        if (!destinationSet)
        {
            destinationSet = true;
            aiCharacter.aICharacterCombatManager.PivotTowardPosition(aiCharacter, positionOfSound);
            aiCharacter.navmeshAgent.enabled = true;

            if (!IsDestinationReachable(aiCharacter, positionOfSound))
            {
                NavMeshHit hit;

                if (NavMesh.SamplePosition(positionOfSound, out hit, 2, NavMesh.AllAreas))
                {
                    NavMeshPath partialPath = new NavMeshPath();
                    aiCharacter.navmeshAgent.CalculatePath(hit.position, partialPath);
                    aiCharacter.navmeshAgent.SetPath(partialPath);
                }
            }
            else
            {
                NavMeshPath path = new NavMeshPath();
                aiCharacter.navmeshAgent.CalculatePath(positionOfSound, path);
                aiCharacter.navmeshAgent.SetPath(path);
            }
        }

        aiCharacter.aICharacterCombatManager.RotateTowardsAgent(aiCharacter);

        float distanceFromDestination = Vector3.Distance(aiCharacter.transform.position, positionOfSound);

        //CAN USE THIS FLAG TO MAKE AI DO OTHER THINGS ONCE REACHED DESTINATION, LIKE GO BACCK TO IDLE STATE OR TO ITS ORIGINAL POSITION
        if (distanceFromDestination <= aiCharacter.navmeshAgent.stoppingDistance)
        {
            destinationReached = true;
        }

        if (destinationReached)
        {
            if (investigationTimer < investigationTime)
            {
                investigationTimer += Time.deltaTime;
            }
            else
            {
                return SwitchState(aiCharacter, aiCharacter.idle);
            }

        }

        return this;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        aiCharacter.navmeshAgent.enabled = false;
        destinationReached= false;
        destinationSet = false;
        //positionOfSound = Vector3.zero;
        investigationTimer = 0;
    }

}
