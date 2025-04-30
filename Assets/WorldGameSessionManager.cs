using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class WorldGameSessionManager : MonoBehaviour
{
    public static WorldGameSessionManager instance;

    [Header("Active Players In Session")]
    public List<PlayerManager> players = new List<PlayerManager>();


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
