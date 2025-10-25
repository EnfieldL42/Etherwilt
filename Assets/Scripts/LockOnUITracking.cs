using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class LockOnUITracking : MonoBehaviour
{
    public LockOnTrasform lockOn;
    [SerializeField] Image image;
    private Vector3 trackingPosition;

    public void SetTracking(LockOnTrasform target)
    {
        lockOn = target;
        StartCoroutine(DelayImage());
    }

    IEnumerator DelayImage()
    {
        yield return new WaitForSeconds(0.1f);
        image.enabled = true;
        yield return null;
    }
    void Update()
    {
        if (lockOn != null)
        {
            trackingPosition = lockOn.gameObject.transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(trackingPosition);
            transform.position = screenPos;
        }
    }

    private void OnDisable()
    {
        image.enabled = false;
    }
}
