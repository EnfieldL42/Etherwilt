using UnityEngine;
using Unity.Netcode;

public class DummySpawn : NetworkBehaviour
{
    [SerializeField] CharacterManager character;

    private void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    private void OnEnable()
    {
        var instance = character;
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }
}
