using UnityEngine;

public class BasicVfxSpawner : MonoBehaviour
{
    public GameObject VFX;
    public void ActivateVFX()
    {
        Instantiate(VFX, transform.position, Quaternion.identity);
    }
}
