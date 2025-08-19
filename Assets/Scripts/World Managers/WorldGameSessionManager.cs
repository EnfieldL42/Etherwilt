using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;

public class WorldGameSessionManager : MonoBehaviour
{
    public static WorldGameSessionManager instance;

    [Header("Active Players In Session")]
    public List<PlayerManager> players = new List<PlayerManager>();

    private Coroutine revivalCoroutine;


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

    public void WaitThenReviveHost()
    {
        if(revivalCoroutine != null)
        {
            StopCoroutine(revivalCoroutine);
        }

        revivalCoroutine = StartCoroutine(ReviveHostCoroutine(5f));
    }

    private IEnumerator ReviveHostCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        PlayerUIManager.instance.playerUILoadingScreenManager.ActivateLoadingScreen();

        PlayerUIManager.instance.localPlayer.ReviveCharacter();

        WorldAIManager.instance.ResetAllCharacters();

        for (int i = 0; i < WorldObjectManager.instance.bonfires.Count; i++)
        {
            if (WorldObjectManager.instance.bonfires[i].bonefireID == WorldSaveGameManager.instance.currentCharacterData.lastBonfireRestedAt)
            {
                WorldObjectManager.instance.bonfires[i].TeleportToBonfire();
                break;
            }
        }

        WorldObjectManager.instance.bonfires[0].TeleportToBonfire(); //CHANGE THIS TO SPAWN LOCATION VECTOR 3
        WorldSaveGameManager.instance.SaveGame();

    }

    public void AddPlayerToActivePlayerList(PlayerManager player)
    {


        if(!players.Contains(player))
        {
            players.Add(player);
        }

        //check for null slots and remove null slots

        for (int i = players.Count - 1; i > -1; i--)
        {
            if (players[i] == null)
            {
                players.RemoveAt(i);
            }
        }
    }

    public void RemovePlayerToActivePlayerList(PlayerManager player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            
        }
        for (int i = players.Count - 1; i > -1; i--)
        {
            if (players[i] == null)
            {
                players.RemoveAt(i);
            }
        }


    }

}
