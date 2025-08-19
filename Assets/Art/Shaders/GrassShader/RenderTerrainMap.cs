using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class RenderTerrainMap : MonoBehaviour
{
    public Camera camToDrawWith;
    public UniversalAdditionalCameraData additionalCameraData;
    // layer to render
    [SerializeField]
    LayerMask layer;
    // objects to render
    [SerializeField]
    Renderer[] renderers;
    // unity terrain to render
    [SerializeField]
    Terrain[] terrains;
    // map resolution
    public int resolution = 512;

    // padding the total size
    public float adjustScaling = 2.5f;

    RenderTexture diffuseTex;
    RenderTexture normalTex;
    RenderTexture depthTex;

    private Bounds bounds;

    void GetBounds()
    {
        bounds = new Bounds(transform.position, Vector3.zero);
        if (renderers.Length > 0)
        {
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (terrains.Length > 0)
        {
            foreach (Terrain terrain in terrains)
            {
                Vector3 terrainCenter = terrain.GetPosition() + terrain.terrainData.bounds.center;
                Bounds worldBounds = new Bounds(terrainCenter, terrain.terrainData.bounds.size);
                bounds.Encapsulate(worldBounds);
            }
        }
    }

    void OnEnable()
    {
        diffuseTex = new RenderTexture(resolution, resolution, 24);
        normalTex = new RenderTexture(resolution, resolution, 24);
        depthTex = new RenderTexture(resolution, resolution, 24);
        GetBounds();
        SetUpCam();

        StartCoroutine(DelayedDraw());
    }

    IEnumerator DelayedDraw()
    {
        yield return null; // wait one frame
        DrawDiffuseMap();
    }
    public void DrawDiffuseMap()
    {
        camToDrawWith.enabled = true;
        additionalCameraData.SetRenderer(0);
        camToDrawWith.targetTexture = diffuseTex;
        camToDrawWith.depthTextureMode = DepthTextureMode.Depth;
        Shader.SetGlobalFloat("_OrthographicCamSizeTerrain", camToDrawWith.orthographicSize);
        Shader.SetGlobalVector("_OrthographicCamPosTerrain", camToDrawWith.transform.position);
        camToDrawWith.Render(); 
        Shader.SetGlobalTexture("_TerrainDiffuse", diffuseTex);
        //Debug.Log("DiffuseMap Rendered");
        DrawNormalMap();
    }

    public void DrawNormalMap()
    {
        camToDrawWith.enabled = true;
        additionalCameraData.SetRenderer(1);
        camToDrawWith.targetTexture = normalTex;
        camToDrawWith.depthTextureMode = DepthTextureMode.Depth;
        camToDrawWith.Render(); 
        Shader.SetGlobalTexture("_TerrainNormal", normalTex);
        //Debug.Log("NormalMap Rendered");
        DrawHeightMap();
    }

    public void DrawHeightMap()
    {
        camToDrawWith.enabled = true;
        additionalCameraData.SetRenderer(2);
        camToDrawWith.targetTexture = depthTex;
        camToDrawWith.depthTextureMode = DepthTextureMode.Depth;
        camToDrawWith.Render();
        Shader.SetGlobalTexture("_TerrainDepth", depthTex);
        //Debug.Log("DepthMap Rendered");
        camToDrawWith.enabled = false;
    }

    void SetUpCam()
    {
        if (camToDrawWith == null)
        {
            camToDrawWith = GetComponentInChildren<Camera>();
        }
        float size = bounds.size.magnitude;
        camToDrawWith.cullingMask = layer;
        camToDrawWith.orthographicSize = size / adjustScaling;
        camToDrawWith.transform.parent = null;
        camToDrawWith.transform.position = bounds.center + new Vector3(0, bounds.extents.y + 500f, 0);
        camToDrawWith.transform.parent = gameObject.transform;
        Shader.SetGlobalFloat("_TerrainCamHeight", camToDrawWith.transform.position.y);
    }

}