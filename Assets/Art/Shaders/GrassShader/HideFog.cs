using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class HideFog : MonoBehaviour
{
    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += BeginRender;
        RenderPipelineManager.endCameraRendering += EndRender;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= BeginRender;
        RenderPipelineManager.endCameraRendering -= EndRender;
    }

    private void BeginRender(ScriptableRenderContext context, Camera camera)
    {
        if(camera.name == "TerrainRender")
        {
            RenderSettings.fog = false;
        }
    }

    private void EndRender(ScriptableRenderContext context, Camera camera)
    {
        if (camera.name == "TerrainRender")
        {
            RenderSettings.fog = true;
        }
    }
}
