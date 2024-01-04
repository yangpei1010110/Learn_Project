#ifndef GET_LIGHT_DATA_INCLUDED
#define GET_LIGHT_DATA_INCLUDED

void _GetLightData(out float3 direction, out float3 color, out float distanceAttenuation, out float shadowAttenuation)
{
    // In Shader Graph Preview we will assume a default light direction and white color
    direction = float3(-0.3, -0.8, 0.6);
    color = float3(1, 1, 1);
    distanceAttenuation = 1.0;
    shadowAttenuation = 1.0;
    // Universal Render Pipeline
    #if defined(UNIVERSAL_LIGHTING_INCLUDED)
    
    // GetMainLight defined in Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
    Light mainLight = GetMainLight();
    direction = -mainLight.direction;
    color = mainLight.color;
    distanceAttenuation = mainLight.distanceAttenuation;
    shadowAttenuation = mainLight.shadowAttenuation;
    
    #elif defined(HD_LIGHTING_INCLUDED) 
    // ToDo: make it work for HDRP (check define above)
    // Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightDefinition.cs.hlsl
    // if (_DirectionalLightCount > 0)
    // {
    //     DirectionalLightData light = _DirectionalLightDatas[0];
    //     lightDir = -light.forward.xyz;
    //     color = light.color;
    //     ......
        
    #endif
}

#endif