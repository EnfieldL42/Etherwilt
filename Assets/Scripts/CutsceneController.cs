using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    private void Start()
    {
        PlayerManager player = FindAnyObjectByType<PlayerManager>();
        SkinnedMeshRenderer[] meshes = player.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = false;
        }
        MeshRenderer[] weaponMeshes = player.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer weapon in weaponMeshes)
        {
            weapon.enabled = false;
        }
        PlayerUIHudManager hudManager = FindAnyObjectByType<PlayerUIHudManager>();
        hudManager.ToggleHUD(false);
    }
    public void EndIntroCutscene()
    {
        PlayerManager player = FindAnyObjectByType<PlayerManager>();
        SkinnedMeshRenderer[] meshes = player.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = true;
        }
        MeshRenderer[] weaponMeshes = player.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer weapon in weaponMeshes)
        {
            weapon.enabled = true;
        }
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
        PlayerUIHudManager hudManager = FindAnyObjectByType<PlayerUIHudManager>();
        hudManager.ToggleHUD(true);
    }
}