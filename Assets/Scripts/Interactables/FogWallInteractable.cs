using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using System.Collections;

public class FogWallInteractable : NetworkBehaviour
{
    [Header("Fog")]
    [SerializeField] GameObject[] fogGameObjects;

    [Header("ID")]
    public int fogWallID;

    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnIsActiveChanged(false, isActive.Value);
        isActive.OnValueChanged += OnIsActiveChanged;
        WorldObjectManager.instance.AddFogWallToList(this);

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        isActive.OnValueChanged -= OnIsActiveChanged;

    }


    private void OnIsActiveChanged(bool oldStatus, bool newStatus)
    {

        StartCoroutine(HandleFogChangeWithDelay(newStatus));
    }
    private IEnumerator HandleFogChangeWithDelay(bool newStatus)
    {
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed

        if (isActive.Value)
        {
            foreach (var fogObject in fogGameObjects)
            {
                fogObject.SetActive(true);
            }
        }
        else
        {
            foreach (var fogObject in fogGameObjects)
            {
                fogObject.SetActive(false);
            }
        }

    }

}
