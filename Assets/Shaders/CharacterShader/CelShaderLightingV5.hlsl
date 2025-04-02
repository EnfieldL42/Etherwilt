void MainLight_float(in float3 clipSpacePos, in float3 worldPos, out float intensity, out float3 color, out float3 shadows)
{
    #ifdef SHADERGRAPH_PREVIEW
        intensity = float3(0.5, 0.5, 0.5);
        color = float3(0.5,0.5,0);
        shadows = float3(0.5,0.5,0);
    #else
        #if SHADOWS_SCREEN
             half4 shadowCoord = ComputeScreenPos(clipSpacePos);
        #else
             half4 shadowCoord = TransformWorldToShadowCoord(worldPos);
        #endif
    
        #if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
            Light light = GetMainLight(shadowCoord);
            color = light.color;
            intensity = length(light.color);
            shadows = light.shadowAttenuation;
        #endif
    
    #endif
}

void Shadowmask_half(float2 lightmapUV, out half4 Shadowmask)
{
#ifdef SHADERGRAPH_PREVIEW
		Shadowmask = half4(1,1,1,1);
#else
    OUTPUT_LIGHTMAP_UV(lightmapUV, unity_LightmapST, lightmapUV);
    Shadowmask = SAMPLE_SHADOWMASK(lightmapUV);
#endif
}

void AdditionalLights_float(in float3 normal, in float3 worldPos, in float3 worldView, in half4 shadowMask, in float lightCutoff, in float lightSmoothness, in float fresnel, in float rimLightCutoff, in float rimLightSmoothness, out float3 addLights, out float3 rimLights)
{
    
    addLights = float3(0, 0, 0);
    rimLights = float3(0, 0, 0);

#ifndef SHADERGRAPH_PREVIEW
    uint pixelLightCount = GetAdditionalLightsCount();
    uint meshRenderingLayers = GetMeshRenderingLayer();

#if USE_FORWARD_PLUS
	for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++) {
		FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
		Light aLight = GetAdditionalLight(lightIndex, worldPos, shadowMask);
#ifdef _LIGHT_LAYERS
		if (IsMatchingLightLayer(aLight.layerMask, meshRenderingLayers))
#endif
		{
	                    float3 color = dot(normal, aLight.direction);
                        color = smoothstep(lightCutoff, lightCutoff + lightSmoothness, color);
                        color *= aLight.color;
                        color *= (aLight.distanceAttenuation * aLight.shadowAttenuation);
            
                        addLights += color;
 
    
                        float3 lightDot = dot(normal, aLight.direction);
                        
                        lightDot *= fresnel;

                        lightDot = smoothstep(rimLightCutoff, rimLightSmoothness, lightDot);

                        rimLights += lightDot;

    
		}
	}
#endif

    InputData inputData = (InputData) 0;
    float4 screenPos = ComputeScreenPos(TransformWorldToHClip(worldPos));
    inputData.normalizedScreenSpaceUV = screenPos.xy / screenPos.w;
    inputData.positionWS = worldPos;

    LIGHT_LOOP_BEGIN(pixelLightCount)

    Light aLight = GetAdditionalLight(lightIndex, worldPos, shadowMask);
	#ifdef _LIGHT_LAYERS
		if (IsMatchingLightLayer(aLight.layerMask, meshRenderingLayers))
	#endif
		{
        float3 color = dot(normal, aLight.direction);
        color = smoothstep(lightCutoff, lightCutoff + lightSmoothness, color);
        color *= aLight.color;
        color *= (aLight.distanceAttenuation * aLight.shadowAttenuation);
            
        addLights += color;

        
        float3 lightDot = dot(normal, aLight.direction);
                        
        lightDot *= fresnel;

        lightDot = smoothstep(rimLightCutoff, rimLightSmoothness, lightDot);

        lightDot *= aLight.color;

        lightDot *= (aLight.distanceAttenuation * aLight.shadowAttenuation);
        
        rimLights += lightDot;

    }
    LIGHT_LOOP_END
#endif

}