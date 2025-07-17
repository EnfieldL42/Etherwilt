using UnityEngine;

public class EventTriggerBossFight : MonoBehaviour
{
    [SerializeField] int[] bossID;


    private void OnTriggerEnter(Collider other)
    {
        foreach (int id in bossID)
        {
            AIBossCharacterManager boss = WorldAIManager.instance.GetBossCharacterByID(id);
            if (boss != null)
            {
                boss.WakeBoss();
            }
        }
    }
}
