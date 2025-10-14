using EZhex1991.EZSoftBone;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AssignWind : MonoBehaviour
{
    public EZSoftBone[] physicsObjects;
    public EZSoftBoneForceField wind;
    private bool windFound = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        StartCoroutine(LookForWind());
    }

    IEnumerator LookForWind()
    {
        while (windFound == false)
        {
            yield return new WaitForSeconds(0.5f);
            wind = FindAnyObjectByType<EZSoftBoneForceField>();
            Debug.Log("Looking for Wind");

            if (wind != null)
            {
                AssignForce();
                windFound = true;
                break;
            }
        }
        yield return null;
    }

    private void AssignForce()
    {
        foreach (EZSoftBone i in physicsObjects)
        {
            i.forceModule = wind;
        }
        Debug.Log("Wind Assigned");
        this.enabled = false;
    }
    
    
}
