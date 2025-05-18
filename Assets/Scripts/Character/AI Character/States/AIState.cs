using UnityEngine;

public class AIState : ScriptableObject
{
    public virtual AIState Tick(AICharacterManager aiCharacter)
    {
        //do some logic to find player

        //if we found a player, return the pursue target state instead

        // if we have not found the player, return the idle state

        return this;
    }

    protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
    {
        ResetStateFlags(aiCharacter);

        return newState;
    }

    protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
    {
        //reset any state flags here so when you return to the state, they are blank once again
    }
}
