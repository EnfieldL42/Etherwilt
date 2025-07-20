using UnityEngine;

public class EventTriggerBossFight : MonoBehaviour
{
    [SerializeField] int[] bossID;


    //private void OnTriggerEnter(Collider other)
    //{
    //    foreach (int id in bossID)
    //    {
    //        AIBossCharacterManager boss = WorldAIManager.instance.GetBossCharacterByID(id);
    //        if (boss != null)
    //        {
    //            boss.WakeBoss();
    //        }
    //    }
    //}

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
        }
    }
}
