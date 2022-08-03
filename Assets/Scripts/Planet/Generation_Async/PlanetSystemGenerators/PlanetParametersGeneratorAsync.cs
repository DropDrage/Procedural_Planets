using Planet.Settings;
using UnityEngine;
using Utils;

namespace Planet.Generation_Async.PlanetSystemGenerators
{
    public class PlanetParametersGeneratorAsync : BaseAsyncPlanetGenerator
    {
        [Space] [Header("Biomes")] [SerializeField]
        private IntRange biomesRange;

        [SerializeField] private IntRange biomeColorCountRange;
        [SerializeField] private FloatRange biomeNoiseOffsetRange;
        [SerializeField] private FloatRange biomeStrengthRange;
        [SerializeField] private FloatRange biomeBlendRange;
        [SerializeField] private IntRange biomeOceanColorCountRange;


        protected override ColorSettings.BiomeColorSettings GenerateBiomeSettingsAsync(System.Random random)
        {
            var biomes = new ColorSettings.BiomeColorSettings.Biome[biomesRange.GetRandomValue(random)];
            for (var i = 0; i < biomes.Length; i++)
            {
                var colorKeys = new GradientColorKey[biomeColorCountRange.GetRandomValue(random)];
                for (var k = 0; k < colorKeys.Length; k++)
                {
                    colorKeys[k].color = random.ColorHSV(0, 1f, 0.1f, 0.825f);
                    colorKeys[k].time = random.NextFloat();
                }

                var gradient = new Gradient {colorKeys = colorKeys};
                biomes[i] = new ColorSettings.BiomeColorSettings.Biome(
                    gradient, random.ColorHSV(0, 1f, 0.1f, 0.825f),
                    random.NextFloat(), random.NextFloat(0.2f));
            }

            return new ColorSettings.BiomeColorSettings(biomes, GenerateNoiseSettings(random),
                biomeNoiseOffsetRange.GetRandomValue(random),
                biomeStrengthRange.GetRandomValue(random),
                biomeBlendRange.GetRandomValue(random));
        }

        protected override Gradient GenerateOceanGradientAsync(System.Random random)
        {
            var oceanColorKeys = new GradientColorKey[biomeOceanColorCountRange.GetRandomValue(random)];
            for (var k = 0; k < oceanColorKeys.Length; k++)
            {
                oceanColorKeys[k].color = random.ColorHSV(0.37f, 0.9f, 0.1f, 0.75f);
                oceanColorKeys[k].time = random.NextFloat();
            }

            var oceanGradient = new Gradient {colorKeys = oceanColorKeys};
            return oceanGradient;
        }
    }
}