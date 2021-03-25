using Noise;
using Settings;
using UnityEngine;

namespace PlanetSystemGenerators
{
    public class SunGenerator : PlanetGeneratorAbstract
    {
        private const float ForwardColorLimit = 55 / 360f;
        private const float BackwardColorLimit = 170 / 360f;

        private const float RotatedColorLimit = ForwardColorLimit + BackwardColorLimit;


        [SerializeField] private IntRange biomeOceanColorCountRange;

        private Color _sunColor;


        protected override ColorSettings.BiomeColorSettings GenerateBiomeSettings()
        {
            {
                var tempSunColor = Random.ColorHSV(0f, RotatedColorLimit, 0.05f, 0.85f, 0.75f, 1f);
                Color.RGBToHSV(tempSunColor, out var h, out var s, out var v);
                h = (1 + ForwardColorLimit - h) % 1;
                _sunColor = Color.HSVToRGB(h, s, v);
            }

            var biomes = new ColorSettings.BiomeColorSettings.Biome[1];
            var biomeColorKeys = new GradientColorKey[1];
            biomeColorKeys[0].color = _sunColor;
            biomeColorKeys[0].time = 0;
            var biomeGradient = new Gradient();
            biomeGradient.colorKeys = biomeColorKeys;
            biomes[0] = new ColorSettings.BiomeColorSettings.Biome(biomeGradient, Color.black, 0, 0);

            return new ColorSettings.BiomeColorSettings(biomes, GenerateNoiseSettings(), 0, 0, 0);
        }

        protected override Gradient GenerateOceanGraident()
        {
            var oceanColorKeys = new GradientColorKey[biomeOceanColorCountRange.Random()];
            {
                Color.RGBToHSV(_sunColor, out var h, out var s, out var v);
                for (var k = oceanColorKeys.Length - 1; k >= 0; k--) //from black to saturated
                {
                    oceanColorKeys[k].color = Color.HSVToRGB(h, s, v);
                    oceanColorKeys[k].time = Random.value;

                    s += Random.value;
                    v -= Random.value;
                }
            }

            var oceanGradient = new Gradient();
            oceanGradient.colorKeys = oceanColorKeys;
            return oceanGradient;
        }


        protected override void CustomGeneration(GameObject planet)
        {
            var sunLight = planet.GetComponent<Light>();
            sunLight.color = _sunColor;

            var gravityBody = planet.GetComponent<GravityBody>();
            sunLight.intensity = gravityBody.Mass * 80;
            sunLight.range = gravityBody.Mass * 5;
        }

        protected override NoiseSettings GenerateNoiseSettings(float downModifier = 1, float limitModifier = 1)
        {
            var noiseSettings = base.GenerateNoiseSettings(downModifier, limitModifier);
            noiseSettings.simpleNoiseSettings.minValue =
                noiseSettings.simpleNoiseSettings.strength + ZeroOneRange.Random();
            if (noiseSettings.rigidNoiseSettings != null)
            {
                noiseSettings.rigidNoiseSettings.minValue =
                    noiseSettings.rigidNoiseSettings.strength + ZeroOneRange.Random();
            }

            return noiseSettings;
        }
    }
}