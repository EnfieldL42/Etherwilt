using UnityEngine;

public class AIBossCharacterNetworkManager : AICharacterNetworkManager
{
    AIBossCharacterManager aiBossCharacter;

    protected override void Awake()
    {
        base.Awake();
        aiBossCharacter = GetComponent<AIBossCharacterManager>();
    }

    public override void CheckHP(int oldValue, int newValue)
    {
        base.CheckHP(oldValue, newValue);

        if(IsOwner)
        {
            if(currentHealth.Value <= 0)
            {
                return;
            }

            float healthNeededForShift = maxHealth.Value * (aiBossCharacter.minimumHealthPercentageForPhaseShift / 100);

            if (currentHealth.Value <= healthNeededForShift)
            {
                //aiBossCharacter.PhaseShift();
            }
        }

    }
}
