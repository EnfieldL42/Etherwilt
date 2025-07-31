using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerInteractionManager : MonoBehaviour
{
    PlayerManager player;

    private List<Interactable> currentInteractableActions;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        currentInteractableActions = new List<Interactable>();
    }

    private void FixedUpdate()
    {
        if(!player.IsOwner)
        {
            return;
        }

        if(!PlayerUIManager.instance.menuWindowIsOpen && !PlayerUIManager.instance.popUpWindowIsOpen)
        {
            CheckForInteractable();
        }
    }

    private void CheckForInteractable()
    {
        if(currentInteractableActions.Count == 0)
        {
            return;
        }
        if (currentInteractableActions[0] == null)
        {
            currentInteractableActions.RemoveAt(0); //if the current interactable item at position 0 becomes null, remove position 0 from the list
            return;
        }

        //if we have an interactable action and have no notified the player, notify them
        if (currentInteractableActions[0] != null)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopUp(currentInteractableActions[0].interactableText);
        }
    }

    private void RefreshInteractionList()
    {
        for (int i = currentInteractableActions.Count - 1; i > -1; i--)
        {
            if (currentInteractableActions[i] == null)
            {
                currentInteractableActions.RemoveAt(i);
            }
        }
    }

    public void AddInteractonToList(Interactable interactableObject)
    {
        RefreshInteractionList();

        if(!currentInteractableActions.Contains(interactableObject))
        {
            currentInteractableActions.Add(interactableObject);
        }

    }

    public void RemoveInteractionFromList(Interactable interactableObject)
    {

        if (currentInteractableActions.Contains(interactableObject))
        {
            currentInteractableActions.Remove(interactableObject);
        }

        RefreshInteractionList();
    }

    public void Interact()
    {
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();

        if(currentInteractableActions.Count == 0)
        {
            return;
        }

        if (currentInteractableActions[0] != null)
        {
            currentInteractableActions[0].Interact(player);
            RefreshInteractionList();
        }
    }
}
