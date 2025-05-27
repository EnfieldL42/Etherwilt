using UnityEngine;

public class Colliders : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        GetComponent<TerrainCollider>().enabled = false;

        GetComponent<TerrainCollider>().enabled = true;
    }
}
