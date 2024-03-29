﻿void AdditionalLights_half(
   half3 WorldPosition,
   half3 WorldNormal,
   out half3 Diffuse)
{
   half3 diffuseColor = 0;
   half3 specularColor = 0;

   #ifndef SHADERGRAPH_PREVIEW

      WorldNormal = normalize(WorldNormal);
      int pixelLightCount = GetAdditionalLightsCount();

      for (int i = 0; i < pixelLightCount; ++i)
      {
         Light light = GetAdditionalLight(i, WorldPosition);
         half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
         diffuseColor += LightingLambert(attenuatedLightColor, WorldNormal, WorldNormal);
      }

   #endif

   Diffuse = diffuseColor;
}