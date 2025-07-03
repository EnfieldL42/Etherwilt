using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class AIBossCharacterManager : AICharacterManager
{
    public int bossID = 0;
    [SerializeField] bool hasBeenDefeated = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsServer)
        {
            //if save data does not contain info on this boss, add this
            if(!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
            }
            //else load the data that already exists on this boss
            else
            {
                hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];

                if(hasBeenDefeated )
                {
                    aICharacterNetworkManager.isActive.Value = false;
                }
            }
        }
    }


    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            //reset any flags

            //if not grounded play falling death anim

            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);

            }

            hasBeenDefeated = true;

            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
            }
            //else load the data that already exists on this boss
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
            }

            WorldSaveGameManager.instance.SaveGame();

            //play death sfx

            yield return new WaitForSeconds(5);

            //award players with runers
            //disable character
        }
    }


}
