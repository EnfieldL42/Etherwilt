using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        WorldSaveGameManager.instance.LoadWorldScene(0);
        PlayerUIManager.instance.CloseAllMenuWindows();
    }
}
