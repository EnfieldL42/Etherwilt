using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class EventTriggerBossFight : NetworkBehaviour
{
    [Header("Event Trigger ID")]
    public int eventTriggerID;

    [SerializeField] int[] bossID;
    public Collider triggerCollider;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        WorldObjectManager.instance.AddBossTriggerToList(this);
    }

    override public void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collect all boss references
        AIBossCharacterManager[] bosses = new AIBossCharacterManager[bossID.Length];
        for (int i = 0; i < bossID.Length; i++)
        {
            bosses[i] = WorldAIManager.instance.GetBossCharacterByID(bossID[i]);
            if (bosses[i] == null || !bosses[i].gameObject.activeInHierarchy)
            {
                // If any boss is missing or inactive, do not trigger
                return;
            }
        }

        // All bosses are present and active, wake each
        foreach (var boss in bosses)
        {
            boss.WakeBoss();
            //if (!boss.hasBeenAwakened.Value)
            //{
            //    
            //}
        }

        triggerCollider.enabled = false;
    }
}
