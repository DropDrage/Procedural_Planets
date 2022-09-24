using System;
using Planet.Settings;
using Planet.Settings.Generation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Planet.Generation.PlanetSystemGenerators
{
    [Obsolete("Use async")]
    public class PlanetParametersGenerator : BaseSyncPlanetGenerator<PlanetGenerationParameters>
    {
        protected override ColorSettings.BiomeColorSettings GenerateBiomeSettings()
        {
            var biomes = new ColorSettings.BiomeColorSettings.Biome[parameters.biomesRange.RandomValue];
            for (var i = 0; i < biomes.Length; i++)
            {
                var colorKeys = new GradientColorKey[parameters.biomeColorCountRange.RandomValue];
                for (var k = 0; k < colorKeys.Length; k++)
                {
                    colorKeys[k].color = Random.ColorHSV(0, 1f, 0.1f, 0.825f);
                    colorKeys[k].time = Random.value;
                }

                var gradient = new Gradient();
                gradient.colorKeys = colorKeys;

                biomes[i] = new ColorSettings.BiomeColorSettings.Biome(
                    gradient, Random.ColorHSV(0, 1f, 0.1f, 0.825f),
                    Random.value, Random.Range(0, 0.2f));
            }

            return new ColorSettings.BiomeColorSettings(biomes, GenerateNoiseSettings(),
                parameters.biomeNoiseOffsetRange.RandomValue, parameters.biomeStrengthRange.RandomValue,
                parameters.biomeBlendRange.RandomValue);
        }

        protected override Gradient GenerateOceanGradient()
        {
            var oceanColorKeys = new GradientColorKey[parameters.biomeOceanColorCountRange.RandomValue];
            for (var k = 0; k < oceanColorKeys.Length; k++) //ToDo triad or other color generator
            {
                oceanColorKeys[k].color = Random.ColorHSV(0.37f, 0.9f, 0.1f, 0.75f);
                oceanColorKeys[k].time = Random.value;
            }

            var oceanGradient = new Gradient();
            oceanGradient.colorKeys = oceanColorKeys;
            return oceanGradient;
        }
    }
}
