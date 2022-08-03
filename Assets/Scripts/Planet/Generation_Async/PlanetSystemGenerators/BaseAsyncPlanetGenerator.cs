using System.Threading.Tasks;
using Noise;
using Planet.Common;
using Planet.Settings;
using UnityEngine;
using Utils;
using static Utils.AsyncUtils;
using Random = System.Random;

namespace Planet.Generation_Async.PlanetSystemGenerators
{
    public abstract class BaseAsyncPlanetGenerator : BasePlanetGenerator
    {
        public async Task<GravityBody> Generate(int seed, TaskScheduler mainScheduler)
        {
            GameObject planetObject = null;
            PlanetAutoGeneratorAsync planetAutoGenerator = null;
            GravityBody gravityBody = null;

            await RunAsyncWithScheduler(() =>
            {
                planetObject = SpawnUtils.SpawnPrefab(prefab);
                planetAutoGenerator = planetObject.GetComponent<PlanetAutoGeneratorAsync>();
                planetAutoGenerator.generateOnStart = false;

                gravityBody = planetAutoGenerator.GetComponent<GravityBody>();
            }, mainScheduler);

            var mainRandom = new Random(seed);
            var colorRandom = new Random(seed);

            var shapeTask = Task.Run(async () =>
            {
                await GenerateShape(planetAutoGenerator, mainRandom, mainScheduler);
                await RunAsyncWithScheduler(() => CalculateGravity(planetAutoGenerator, mainRandom), mainScheduler);
            });
            var colorTask = GenerateColor(planetAutoGenerator, colorRandom, mainScheduler);

            Task.WaitAll(shapeTask, colorTask);
            await RunAsyncWithScheduler(() => CustomGeneration(planetObject, mainRandom), mainScheduler);

            Debug.Log("Parameters Done");
            await planetAutoGenerator.GeneratePlanet(mainScheduler);
            Debug.Log("Planet generated");
            return gravityBody;
        }

        private async Task GenerateShape(PlanetAutoGeneratorAsync planet, Random random, TaskScheduler main)
        {
            var noiseLayers = new ShapeSettings.NoiseLayer[noiseLayersRange.GetRandomValue(random)];
            for (int i = 0, noiseSettingsLength = noiseLayers.Length + 1; i < noiseLayers.Length; i++)
            {
                noiseLayers[i] = new ShapeSettings.NoiseLayer(
                    random.Next(0, i) > i / 2f,
                    GenerateNoiseSettings(i + 1, noiseSettingsLength, random)
                );
            }

            var shapeSettings = planet.shapeSettings =
                await RunAsyncWithScheduler(ScriptableObject.CreateInstance<ShapeSettings>, main);
            shapeSettings.planetRadius = planetRadiusRange.GetRandomValue(random);
            shapeSettings.noiseLayers = noiseLayers;
            planet.resolution = (int) (shapeSettings.planetRadius / planetRadiusRange.to * 256);
        }

        private NoiseSettings GenerateNoiseSettings(int i, int length, Random random)
        {
            var downModifier = (float) (length - i) / length;
            var limitModifier = (float) i / length;

            return GenerateNoiseSettings(random, downModifier, limitModifier);
        }

        protected virtual NoiseSettings GenerateNoiseSettings(Random random,
            float downModifier = 1, float limitModifier = 1)
        {
            var filterType = (NoiseSettings.FilterType) random.Next(0, FilterTypesLastIndex);
            var layersCount = Mathf.CeilToInt(layersInNoiseCountRange.GetRandomValue(random) * limitModifier);
            var strength = strengthRange.GetRandomValue(random) * limitModifier / layersCount;
            var simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings(
                strength,
                layersCount,
                baseRoughnessRange.GetRandomValue(random) * limitModifier,
                roughnessRange.GetRandomValue(random) * limitModifier,
                persistenceRange.GetRandomValue(random) * limitModifier,
                centerMagnitudeRange.GetRandomValue(random) * random.OnUnitSphere(),
                strength * zeroOneRange.GetRandomValue(random) * downModifier
            );

            NoiseSettings.RigidNoiseSettings rigidNoiseSettings = null;
            if (filterType == NoiseSettings.FilterType.Rigid)
            {
                rigidNoiseSettings = simpleNoiseSettings.ToRigid(weightRange.GetRandomValue(random));
            }

            return new NoiseSettings(filterType, simpleNoiseSettings, rigidNoiseSettings);
        }

        private void CalculateGravity(Planet planet, Random random)
        {
            var planetRigidbody = planet.GetComponent<Rigidbody>();
            planetRigidbody.mass = planet.shapeSettings.planetRadius
                                   * planet.shapeSettings.planetRadius
                                   * massMultiplierRange.GetRandomValue(random);
            planetRigidbody.AddTorque(random.OnUnitSphere() * angularVelocityRange.GetRandomValue(random),
                ForceMode.VelocityChange);
        }

        private async Task GenerateColor(Planet planet, Random random, TaskScheduler main)
        {
            var colorSettings = planet.colorSettings = await RunAsyncWithScheduler(() =>
            {
                var color = ScriptableObject.CreateInstance<ColorSettings>();
                color.planetMaterial = new Material(shader);
                return color;
            }, main);
            colorSettings.biomeColorSettings = GenerateBiomeSettingsAsync(random);
            colorSettings.oceanGradient = GenerateOceanGradientAsync(random);
        }


        protected abstract ColorSettings.BiomeColorSettings GenerateBiomeSettingsAsync(Random random);

        protected abstract Gradient GenerateOceanGradientAsync(Random random);


        protected virtual void CustomGeneration(GameObject planet, Random random)
        {
        }
    }
}
