using UnityEngine;


[CreateAssetMenu(menuName = "A.I/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if(aiCharacter.characterCombatManager.currentTarget != null)
        {
            Debug.Log("target found");

            return this;

        }
        else
        {
            //return this state,
            aiCharacter.aICharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);
            Debug.Log("Searching for a target");

            return this;

        }

    }

}
