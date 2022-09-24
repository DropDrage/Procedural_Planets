using Noise;
using Planet.Settings;
using Planet.Settings.Generation;
using UnityEngine;
using Utils;

namespace Planet.Generation_Async.PlanetSystemGenerators
{
    public class SunParametersGeneratorAsync : BaseAsyncPlanetGenerator<SunGenerationParameters>
    {
        private const float ForwardColorLimit = 55 / 360f;
        private const float BackwardColorLimit = 170 / 360f;
        private const float RotatedColorLimit = ForwardColorLimit + BackwardColorLimit;

        private Color _sunColor;


        protected override ColorSettings.BiomeColorSettings GenerateBiomeSettingsAsync(System.Random random)
        {
            var tempSunColor = random.ColorHSV(0f, RotatedColorLimit, 0.05f, 0.85f, 0.8f, 1f);
            Color.RGBToHSV(tempSunColor, out var h, out var s, out var v);
            h = (1 + ForwardColorLimit - h) % 1;
            _sunColor = Color.HSVToRGB(h, s, v);

            var biomes = new ColorSettings.BiomeColorSettings.Biome[1];
            var biomeColorKeys = new GradientColorKey[1];
            biomeColorKeys[0].color = _sunColor;
            biomeColorKeys[0].time = 0;
            var biomeGradient = new Gradient {colorKeys = biomeColorKeys};
            biomes[0] = new ColorSettings.BiomeColorSettings.Biome(biomeGradient, Color.black, 0, 0);

            return new ColorSettings.BiomeColorSettings(biomes, GenerateNoiseSettings(random), 0, 0, 0);
        }

        protected override Gradient GenerateOceanGradientAsync(System.Random random)
        {
            var oceanColorKeys = new GradientColorKey[parameters.biomeOceanColorCountRange.GetRandomValue(random)];

            var time = random.NextFloat();
            Color.RGBToHSV(_sunColor, out var h, out var s, out var v);
            for (var k = oceanColorKeys.Length - 1; k >= 0; k--) //from black to saturated
            {
                oceanColorKeys[k].color = Color.HSVToRGB(h, s, v);
                oceanColorKeys[k].time = time;

                s += random.NextFloat();
                v = Mathf.Lerp(0, v, random.NextFloat());
                time = Mathf.Lerp(0, time, random.NextFloat());
            }

            var oceanGradient = new Gradient {colorKeys = oceanColorKeys};
            return oceanGradient;
        }

        protected override NoiseSettings GenerateNoiseSettings(System.Random random,
            float downModifier = 1, float limitModifier = 1)
        {
            var noiseSettings = base.GenerateNoiseSettings(random, downModifier, limitModifier);
            noiseSettings.simpleNoiseSettings.minValue =
                noiseSettings.simpleNoiseSettings.strength + zeroOneRange.GetRandomValue(random);
            if (noiseSettings.rigidNoiseSettings != null)
            {
                noiseSettings.rigidNoiseSettings.minValue =
                    noiseSettings.rigidNoiseSettings.strength + zeroOneRange.GetRandomValue(random);
            }

            return noiseSettings;
        }


        // sync
        protected override void CustomGeneration(GameObject planet, System.Random random)
        {
            var sunLight = planet.GetComponent<Light>();
            Color.RGBToHSV(_sunColor, out var h, out var s, out var v);
            s = Mathf.Lerp(0, s, random.NextFloat());
            v = Mathf.Lerp(v, 1f, random.NextFloat());
            sunLight.color = Color.HSVToRGB(h, s, v);

            var gravityBody = planet.GetComponent<GravityBody>();
            sunLight.intensity = parameters.lightIntensityRange.GetRandomValue(random);
            sunLight.range = gravityBody.Mass * 100;
        }
    }
}
