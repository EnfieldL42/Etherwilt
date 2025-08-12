using Unity.Netcode;
using UnityEngine;

public class AICharacterNetworkManager : CharacterNetworkManager
{
    AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
    }

    public override void OnIsDeadChanged(bool oldStatus, bool newStatus)
    {
        base.OnIsDeadChanged(oldStatus, newStatus);

        if (aiCharacter.isDead.Value)
        {
            aiCharacter.aICharacterCombatManager.AwardEtherOnDeath(PlayerUIManager.instance.localPlayer);

        }
    }
}
