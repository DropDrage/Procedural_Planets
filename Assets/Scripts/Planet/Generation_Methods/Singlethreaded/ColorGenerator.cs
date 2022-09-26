using System;
using Noise;
using Planet.Common;
using Planet.Common.Generation;
using Planet.Settings;
using UnityEngine;

namespace Planet.Generation_Methods.Singlethreaded
{
    [Obsolete("Use async")]
    public class ColorGenerator : BaseColorGenerator
    {
        public void UpdateSettings(ColorSettings settings)
        {
            base.settings = settings;

            if (texture == null || texture.height != settings.biomeColorSettings.biomes.Length)
            {
                texture = new Texture2D(DoubledTextureResolution, settings.biomeColorSettings.biomes.Length,
                    TextureFormat.RGBA32, false);
            }

            biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
        }

        public void UpdateElevation(MinMax elevationMinMax)
        {
            settings.planetMaterial.SetVector(ElevationMinMax, new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        }

        public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
        {
            var heightPercent = (pointOnUnitSphere.y + 1) / 2f
                                + (biomeNoiseFilter.Evaluate(pointOnUnitSphere) -
                                   settings.biomeColorSettings.noiseOffset)
                                * settings.biomeColorSettings.noiseStrength;
            var biomeIndex = 0f;
            var biomesCount = settings.biomeColorSettings.biomes.Length;
            var blendRange = settings.biomeColorSettings.blendAmount / 2f + .001f;

            for (var i = 0; i < biomesCount; i++)
            {
                var distance = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
                var weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
                biomeIndex = biomeIndex * (1 - weight) + i * weight;
            }

            return biomeIndex / Mathf.Max(1, biomesCount - 1);
        }

        public void UpdateColors()
        {
            var colors = new Color[texture.width * texture.height];
            for (var biomeIndex = 0; biomeIndex < settings.biomeColorSettings.biomes.Length; biomeIndex++)
            {
                var biome = settings.biomeColorSettings.biomes[biomeIndex];
                var colorIndex = biomeIndex * DoubledTextureResolution;
                for (var i = 0; i < TextureResolution; i++)
                {
                    var gradientColor = settings.oceanGradient.Evaluate(i / TextureResolutionMinusOne);
                    colors[colorIndex++] = GenerateColorFromGradient(gradientColor, biome.tint, biome.tintPercent);
                }

                for (int i = TextureResolution; i < DoubledTextureResolution; i++)
                {
                    var gradientColor = biome.gradient.Evaluate((i - TextureResolution) / TextureResolutionMinusOne);
                    colors[colorIndex++] = GenerateColorFromGradient(gradientColor, biome.tint, biome.tintPercent);
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            settings.planetMaterial.SetTexture(Texture, texture);
        }
    }
}
