using System.Threading.Tasks;
using Noise;
using Planet.Common;
using Planet.Common.Generation;
using Planet.Settings;
using UnityEngine;
using Utils;

namespace Planet.Generation_Methods.Multithreaded
{
    public class ColorGeneratorAsync : BaseColorGenerator
    {
        public async Task UpdateSettings(ColorSettings settings, TaskScheduler main)
        {
            this.settings = settings;

            if (texture == null || texture.height != settings.biomeColorSettings.biomes.Length)
            {
                texture = await AsyncUtils.RunAsyncWithScheduler(() =>
                    new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGBA32, false), main);
            }

            biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
        }

        public void UpdateElevation(MinMax elevationMinMax)
        {
            settings.planetMaterial.SetVector(ElevationMinMax, new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        }

        public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
        {
            var biomeColorSettings = settings.biomeColorSettings;
            var heightPercent = (pointOnUnitSphere.y + 1) / 2f
                + (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - biomeColorSettings.noiseOffset)
                * biomeColorSettings.noiseStrength;
            var biomeIndex = 0f;
            var biomesCount = biomeColorSettings.biomes.Length;
            var blendRange = biomeColorSettings.blendAmount / 2f + .001f;

            for (var i = 0; i < biomesCount; i++)
            {
                var distance = heightPercent - biomeColorSettings.biomes[i].startHeight;
                var weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
                biomeIndex = biomeIndex * (1 - weight) + i * weight;
            }

            return biomeIndex / Mathf.Max(1, biomesCount - 1);
        }

        public async Task UpdateColors(TaskScheduler main)
        {
            var colors = new Color[TextureSize];
            for (int biomeIndex = 0, biomesCount = settings.biomeColorSettings.biomes.Length;
                 biomeIndex < biomesCount;
                 biomeIndex++)
            {
                var biome = settings.biomeColorSettings.biomes[biomeIndex];
                var biomeTint = biome.tint;
                var biomeTintPercent = biome.tintPercent;

                for (int i = 0, colorIndex = biomeIndex * DoubledTextureResolution;
                     i < TextureResolution;
                     i++, colorIndex++)
                {
                    var time = i / TextureResolutionMinusOne;
                    var oceanGradientColor = settings.oceanGradient.Evaluate(time);
                    colors[colorIndex] = GenerateColorFromGradient(oceanGradientColor, biomeTint, biomeTintPercent);

                    var biomeGradientColor = biome.gradient.Evaluate(time);
                    colors[colorIndex + TextureResolution] =
                        GenerateColorFromGradient(biomeGradientColor, biomeTint, biomeTintPercent);
                }
            }

            await AsyncUtils.RunAsyncWithScheduler(() =>
            {
                texture.SetPixels(colors);
                texture.Apply();
                settings.planetMaterial.SetTexture(Texture, texture);
            }, main);
        }
    }
}
