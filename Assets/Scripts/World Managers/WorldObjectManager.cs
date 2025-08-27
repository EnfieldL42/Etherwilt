using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

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
                fogWall.interactableCollider.enabled = true;
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


}
