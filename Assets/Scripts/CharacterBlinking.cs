using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterBlinking : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer[] meshes;
    [Header("Blink Properties")]
    private float blinkInterval = 4.0f;
    private float blinkEyeCloseDuration = 0.05f;
    private float blinkOpeningDuration = 0.1f;
    private float blinkClosingDuration = 0.1f;

    private void OnEnable()
    {
        StartCoroutine(BlinkCoroutine());  
    }

    private void OnDisable()
    {
        StopCoroutine(BlinkCoroutine());
    }
    IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkInterval - 1f, blinkInterval + 1f));
            var value = 0f;
            var closeSpeed = 1f / blinkClosingDuration;
            while (value < 1)
            {
                foreach (var mesh in meshes)
                {
                    mesh.SetBlendShapeWeight(0, value * 100);
                    value += Time.deltaTime * closeSpeed;
                    yield return null;
                }
            }
            foreach (var mesh in meshes)
            {
                mesh.SetBlendShapeWeight(0, 100);
            }
            
            yield return new WaitForSeconds(blinkEyeCloseDuration);

            value = 1f;
            var openSpeed = 1f / blinkOpeningDuration;
            while (value > 0)
            {
                foreach (var mesh in meshes)
                {
                    mesh.SetBlendShapeWeight(0, value * 100);
                    value -= Time.deltaTime * openSpeed;
                    yield return null;
                }
            }
            foreach (var mesh in meshes)
            {
                mesh.SetBlendShapeWeight(0, 0);
            }
        }
    }
}
