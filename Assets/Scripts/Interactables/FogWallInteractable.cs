using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using System.Collections;

public class FogWallInteractable : Interactable
{
    [Header("Fog")]
    [SerializeField] GameObject[] fogGameObjects;

    [Header("Collision")]
    [SerializeField] Collider fogWallCollider;

    [Header("ID")]
    public int fogWallID;

    [Header("Sound")]
    private AudioSource fogWallAudioSource;
    [SerializeField] AudioClip fogWallSFX;

    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        fogWallAudioSource = GetComponent<AudioSource>();
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        Quaternion targetRotation = transform.localRotation;
        player.transform.rotation = targetRotation;

        AllowPlayerThroughFogWallksCollidersServerRpc(player.NetworkObjectId);
        player.playerAnimatorManager.PlayTargetActionAnimation("Pass_Through_Fog_01", true);

    }

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

    [ServerRpc(RequireOwnership = false)]
    private void AllowPlayerThroughFogWallksCollidersServerRpc(ulong playerObjectID)
    {
        if (IsServer)
        {
            AllowPlayerThroughFogWallksCollidersClientRpc(playerObjectID);
        }
    }
    [ClientRpc]
    private void AllowPlayerThroughFogWallksCollidersClientRpc(ulong playerObjectID)
    {
        PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>();

        if (fogWallSFX != null)
        {
            fogWallAudioSource.PlayOneShot(fogWallSFX);
        }

        if (player != null)
        {
            StartCoroutine(DisableCollisionForTime(player));
        }
    }

    private IEnumerator DisableCollisionForTime(PlayerManager player)
    {
        Physics.IgnoreCollision(player.characterController, fogWallCollider, true);
        yield return new WaitForSeconds(1);
        Physics.IgnoreCollision(player.characterController, fogWallCollider, false);

    }

}
