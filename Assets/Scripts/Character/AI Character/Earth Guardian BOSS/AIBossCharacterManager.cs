using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class AIBossCharacterManager : AICharacterManager
{
    public int bossID = 0;


    [Header("Music")]
    [SerializeField] AudioClip bossIntroClip;
    [SerializeField] AudioClip bossBattleLoopClip;

    [Header("Status")]
    public NetworkVariable<bool> bossFightIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenAwakened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenDefeated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] List<FogWallInteractable> fogWalls;
    [SerializeField] string sleepAnimation;
    [SerializeField] string awakenAnimation;

    [Header("States")]
    public BossSleepState sleepState;

    [SerializeField] string bossDefeatedMessage = "Ancient Conquered";

    [Header("Phase Shift")]
    public bool canPhaseShift = true;
    public float minimumHealthPercentageForPhaseShift = 50;
    [SerializeField] protected string phaseShiftAnimation = "Phase_Shift_01";
    [SerializeField] protected CombatStanceState phase02CombatStanceState;

    [Header("Second Body Settings")]
    public int[] bossGroupIDs;
    public bool isSecondBody = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        bossFightIsActive.OnValueChanged += OnBossFightIsActiveChanged;
        OnBossFightIsActiveChanged(false, bossFightIsActive.Value);

        if (IsOwner)
        {
            sleepState = Instantiate(sleepState);
            currentState = sleepState;
        }

        if (IsServer)
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
                hasBeenDefeated.Value = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
                hasBeenAwakened.Value = WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID];
            }

            StartCoroutine(GetFogWallsFromWorldObjectManager());
            
            if(hasBeenAwakened.Value)
            {
                for(int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = true;
                }
            }

            if (hasBeenDefeated.Value)
            {
                for (int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = false;
                }

                aICharacterNetworkManager.isActive.Value = false;
            }
        }

        if(!hasBeenAwakened.Value)
        {
            animator.Play(sleepAnimation);
        }

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        bossFightIsActive.OnValueChanged -= OnBossFightIsActiveChanged;

    }

    private IEnumerator GetFogWallsFromWorldObjectManager()
    {
        while (WorldObjectManager.instance.fogWalls.Count == 0)
        {
            yield return new WaitForEndOfFrame();
        }

        fogWalls = new List<FogWallInteractable>();

        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallID == bossID)
            {
                fogWalls.Add(fogWall);
            }
        }
    }

    //public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    //{
    //    PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp(bossDefeatedMessage);

    //    if (IsOwner)
    //    {
    //        characterNetworkManager.currentHealth.Value = 0;
    //        isDead.Value = true;

    //        bossFightIsActive.Value = false;

    //        foreach (var fogWall in fogWalls)
    //        {
    //            fogWall.isActive.Value = false;
    //        }

    //        //reset any flags

    //        //if not grounded play falling death anim

    //        if (!manuallySelectDeathAnimation)
    //        {
    //            characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);

    //        }

    //        hasBeenDefeated.Value = true;

    //        if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
    //        {
    //            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
    //            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
    //        }
    //        //else load the data that already exists on this boss
    //        else
    //        {
    //            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
    //            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
    //            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
    //            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
    //        }

    //        WorldSaveGameManager.instance.SaveGame();

    //        //play death sfx

    //        yield return new WaitForSeconds(5);

    //        //award players with runers
    //    }
    //}

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            // Play individual death animation
            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
            }

            hasBeenDefeated.Value = true;
        }

        // Wait a frame to ensure isDead is synced before checking
        yield return null;

        // Check if all bosses in the group are dead
        bool allDead = true;

        foreach (int id in bossGroupIDs)
        {
            AIBossCharacterManager boss = WorldAIManager.instance.GetBossCharacterByID(id);
            if (boss == null || !boss.isDead.Value)
            {
                allDead = false;
                break;
            }
        }

        if (!allDead)
            yield break; // Exit if not all bosses are dead

        // Only proceed once all bosses in the group are defeated
        if (IsOwner)
        {
            bossFightIsActive.Value = false;

            foreach (var fogWall in fogWalls)
            {
                fogWall.isActive.Value = false;
            }

            PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp(bossDefeatedMessage);

            foreach (int id in bossGroupIDs)
            {
                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(id))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(id, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(id, true);
                }
                else
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(id);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(id);
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(id, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(id, true);
                }
            }

            WorldSaveGameManager.instance.SaveGame();

            yield return new WaitForSeconds(5);

            // award runes, etc.

            gameObject.SetActive(false);
        }
    }

    public void WakeBoss()
    {

        if (IsOwner)
        {
            if (!hasBeenAwakened.Value)
            {
                characterAnimatorManager.PlayTargetActionAnimation(awakenAnimation, true);
            }

            bossFightIsActive.Value = true;
            hasBeenAwakened.Value = true;
            currentState = idle;

            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }
            //else load the data that already exists on this boss
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }

            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].isActive.Value = true;
            }


        }



    }

    private void OnBossFightIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if(bossFightIsActive.Value && !isSecondBody)
        {
            if(bossIntroClip != null && bossBattleLoopClip != null)
            {
                WorldSoundFXManager.instance.PlayBossTrack(bossIntroClip, bossBattleLoopClip);
            }

            GameObject bossHealthBar = Instantiate(PlayerUIManager.instance.playerUIHudManager.bossHealthBarObject, PlayerUIManager.instance.playerUIHudManager.bossHealthBarParent);

            UI_Boss_HP_Bar bossHPBar = bossHealthBar.GetComponentInChildren<UI_Boss_HP_Bar>();


            bossHPBar.EnableBossHPBar(this);
            

            PlayerUIManager.instance.playerUIHudManager.currentBossHealthBar = bossHPBar;
        }
        else
        {
            WorldSoundFXManager.instance.StopBossMusic();
        }
    }

    public virtual void PhaseShift()
    {
        if(canPhaseShift == true)
        {
            canPhaseShift = false;
            characterAnimatorManager.PlayTargetActionAnimation(phaseShiftAnimation, true);
            combatState = Instantiate(phase02CombatStanceState);
            currentState = combatState;
        }
    }


}
