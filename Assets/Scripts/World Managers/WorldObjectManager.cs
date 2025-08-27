using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager instance;

    [Header("Network Objects")]
    [SerializeField] List<NetworkObjectSpawner> networkObjectSpawners;
    [SerializeField] List<GameObject> spawnedInObjects;

    [Header("Fog Walls")]
    public List<FogWallInteractable> fogWalls;

    [Header("Bonfires")]
    public List<BonefireInteractable> bonfires;

    [Header("Boss Triggers")]
    public List<EventTriggerBossFight> bossTriggers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnObject(NetworkObjectSpawner networkObjectsSpawner)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            networkObjectSpawners.Add(networkObjectsSpawner);
            networkObjectsSpawner.AttemptToSpawnObject();
        }

    }

    public void AddFogWallToList(FogWallInteractable fogWall)
    {
        if(!fogWalls.Contains(fogWall))
        {
            fogWalls.Add(fogWall);
        }
    }

    public void RemoveFogWallToList(FogWallInteractable fogWall)
    {
        if (fogWalls.Contains(fogWall))
        {
            fogWalls.Remove(fogWall);
        }
    }

    public void ResetAllFogWalls()
    {
        foreach (var fogWall in fogWalls)
        {
            if (fogWall != null)
            {
                fogWall.interactableCollider.enabled = true;
            }
        }
    }

    //public void TurnOffFogWallEventTrigger()
    //{
    //    foreach (var fogWall in fogWalls)
    //    {
    //        if (fogWall != null)
    //            fogWall.eventTrigger.enabled = false;
    //    }
    //}


    public void AddBonfireToList(BonefireInteractable bonfire)
    {
        if (!bonfires.Contains(bonfire))
        {
            bonfires.Add(bonfire);
        }
    }

    public void RemoveBonfireToList(BonefireInteractable bonfire)
    {
        if (bonfires.Contains(bonfire))
        {
            bonfires.Remove(bonfire);
        }
    }


    public void AddBossTriggerToList(EventTriggerBossFight eventTrigger)
    {
        if (!bossTriggers.Contains(eventTrigger))
        {
            bossTriggers.Add(eventTrigger);
        }
    }

    public void RemoveBossTriggerToList(EventTriggerBossFight eventTrigger)
    {
        if (bossTriggers.Contains(eventTrigger))
        {
            bossTriggers.Remove(eventTrigger);
        }
    }

    public void ResetAllBossTriggers()
    {
        foreach (var eventTrigger in bossTriggers)
        {
            if (eventTrigger != null)
            {
                eventTrigger.triggerCollider.enabled = true;
            }
        }
    }

}
