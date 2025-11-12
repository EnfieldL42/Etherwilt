using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class PickUpEtherInteractable : Interactable
{
    public int etherCount = 0;
    public SkinnedMeshRenderer[] meshes;

    private void OnEnable()
    {
        StartCoroutine(DissolveIn());  
    }
    IEnumerator DissolveIn()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var i in meshes)
        {
            Sequence inDissolve = DOTween.Sequence();
            inDissolve.Append(i.material.DOFloat(0f, "_Dissolve", 4f));
        }

    }

    public override void Interact(PlayerManager player)
    {
        WorldSaveGameManager.instance.currentCharacterData.hasDeadSpot = false;
        player.playerStatsManager.AddEther(etherCount);
        WorldSoundFXManager.instance.PlayEtherPickSound();
        interactableCollider.enabled = false;
        player.playerInteractionManager.RemoveInteractionFromList(this);
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        StartCoroutine(DissolveOut());
    }

    IEnumerator DissolveOut()
    { 
        foreach (var i in meshes)
        {
                Sequence deathDissolve = DOTween.Sequence();
            deathDissolve.Append(i.material.DOFloat(1f, "_Dissolve", 3f));
        }
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

}
