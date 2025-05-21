using UnityEngine;


[CreateAssetMenu(menuName = "A.I/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {

        if(aiCharacter.characterCombatManager.currentTarget != null)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueState);
        }
        else
        {
            //return this state,
            aiCharacter.aICharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);

            return this;

        }

    }

}
