using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BonefireInteractable : Interactable
{
    [Header("Bonefire Info")]
    public int bonefireID;
    public NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("VFX")]
    [SerializeField] GameObject activatedParticles;

    [Header("Interaction Text")]
    [SerializeField] string unactivatedInteractionText = "Restore the Bonfire";
    [SerializeField] string activatedInteractionText = "Rest";
    [SerializeField] string interactionText = "BONFIRE RESTORED";

    [Header("Teleport Transform")]
    [SerializeField] Transform teleportTransform;

    protected override void Start()
    {
        base.Start();

        if(IsOwner)
        {
            if (WorldSaveGameManager.instance.currentCharacterData.bonfires.ContainsKey(bonefireID))
            {
                isActivated.Value = WorldSaveGameManager.instance.currentCharacterData.bonfires[bonefireID];
            }
            else
            {
                isActivated.Value = false;
            }

        }

        if (isActivated.Value)
        {
            interactionText = activatedInteractionText;
        }
        else
        {
            interactionText = unactivatedInteractionText;
        }


    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(!IsOwner)
        {
            OnIsActivatedChanged(false, isActivated.Value);
        }
        isActivated.OnValueChanged += OnIsActivatedChanged;

        WorldObjectManager.instance.AddBonfireToList(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        isActivated.OnValueChanged -= OnIsActivatedChanged;
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        PlayerUIManager.instance.CloseBonfireWindows();
    }

    private void RestoreBonfire(PlayerManager player)
    {
        isActivated.Value = true;

        //is save file contrina info on this bonfire, remove it 
        if(WorldSaveGameManager.instance.currentCharacterData.bonfires.ContainsKey(bonefireID))
        {
            WorldSaveGameManager.instance.currentCharacterData.bonfires.Remove(bonefireID);
        }
        //re add it wit h the value true(is activated)
        WorldSaveGameManager.instance.currentCharacterData.bonfires.Add(bonefireID, true);

        player.playerAnimatorManager.PlayTargetActionAnimation("Activate_Bonfire_01", true);
        player.playerNetworkManager.HideWeaponServerRpc();

        //hide weapon models

        PlayerUIManager.instance.playerUIPopUpManager.SendBonfireRestoredDefeatedPopUp(interactionText);

        StartCoroutine(WaitForAnimationAndPopUpThenRestoreCollider());
    }

    private void RestAtBonfire(PlayerManager player)
    {
        PlayerUIManager.instance.playerUIBonfireManager.OpenBonfireManagerMenu();

        interactableCollider.enabled = true;//temporary so we can keep interacting with the bonfire for the meantime
        player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxHealth.Value; //temp code
        player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value; //temp code

        WorldAIManager.instance.ResetAllCharacters();
    }

    private IEnumerator WaitForAnimationAndPopUpThenRestoreCollider()
    {
        yield return new WaitForSeconds(2);
        interactableCollider.enabled = true;
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        if (!isActivated.Value)
        {
            RestoreBonfire(player);
        }
        else
        {
            RestAtBonfire(player);
        }

    }

    private void OnIsActivatedChanged(bool oldStatus, bool newStatus)
    {
        if (isActivated.Value)
        {
            if(activatedParticles != null)
            {
                activatedParticles.SetActive(true);
            }

            interactionText = unactivatedInteractionText;
        }
    }

    public void TeleportToBonfire()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        PlayerUIManager.instance.playerUILoadingScreenManager.ActivateLoadingScreen();

        player.transform.position = teleportTransform.position;

        PlayerUIManager.instance.playerUILoadingScreenManager.DeactivateLoadingScreen();

    }
}

