using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class DialogueInteractable : Interactable
{
    AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
    }

    public override void Interact(PlayerManager player)
    {
        if (PlayerUIManager.instance.menuWindowIsOpen)
        {
            return;
        }

        if (aiCharacter.isDead.Value)
        {
            interactableCollider.enabled = false;
            return;
        }

        if (NetworkManager.Singleton.IsServer)
        {
            WorldSaveGameManager.instance.SaveGame();
            //Close any open popups
        }

        //1. play current dialogue
        aiCharacter.aICharacterSoundFXManager.PlayCurrentDialogueEvent();

        //2. use face IK tracking to look at player 
    }

    public override void OnTriggerEnter(Collider other)
    {
        if(aiCharacter.isDead.Value)
        {
            interactableCollider.enabled = false;

            //if there is an active dialogue, close it
            PlayerManager player = other.GetComponent<PlayerManager>();

            if (player != null && player.IsOwner)
            {
                aiCharacter.aICharacterSoundFXManager.CancelCurrentDialogueEvent();
            }
        }

        base.OnTriggerEnter(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player == null)
        {
            return;
        }

        if (!player.IsOwner)
        {
            return;
        }

        //cancel dialogue if player walks away
        aiCharacter.aICharacterSoundFXManager.CancelCurrentDialogueEvent();
        //close all menus from this character
        //reset face IK
    }
}
