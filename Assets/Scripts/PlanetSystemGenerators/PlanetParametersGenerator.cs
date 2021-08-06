using Settings;
using UnityEngine;

namespace PlanetSystemGenerators
{
    public class PlanetParametersGenerator : PlanetGeneratorAbstract
    {
        [Space(20)] [Header("Biomes")] [SerializeField]
        private IntRange biomesRange;

        [SerializeField] private IntRange biomeColorCountRange;
        [SerializeField] private FloatRange biomeNoiseOffsetRange;
        [SerializeField] private FloatRange biomeStrengthRange;
        [SerializeField] private FloatRange biomeBlendRange;
        [SerializeField] private IntRange biomeOceanColorCountRange;


        protected override ColorSettings.BiomeColorSettings GenerateBiomeSettings()
        {
            var biomes = new ColorSettings.BiomeColorSettings.Biome[biomesRange.RandomValue];
            for (var i = 0; i < biomes.Length; i++)
            {
                var colorKeys = new GradientColorKey[biomeColorCountRange.RandomValue];
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
                biomeNoiseOffsetRange.RandomValue, biomeStrengthRange.RandomValue, biomeBlendRange.RandomValue);
        }

        protected override Gradient GenerateOceanGraident()
        {
            var oceanColorKeys = new GradientColorKey[biomeOceanColorCountRange.RandomValue];
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