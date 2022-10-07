using System.Threading.Tasks;
using Noise;
using Planet.Common;
using Planet.Settings;
using Planet.Settings.Generation;
using UnityEngine;
using Utils;
using static Utils.AsyncUtils;
using Random = System.Random;

namespace Planet.Generation_Methods.Jobs.PlanetSystemGenerators
{
    public abstract class BasePlanetGeneratorJob<T, TGenerator> : BasePlanetGenerator<T>
        where T : BasePlanetGenerationParameters
        where TGenerator : PlanetGeneratorJob, new()
    {
        public async Task<GravityBody> Generate(int seed, TaskScheduler mainScheduler)
        {
            GameObject planetObject = null;
            Planet planet = null;
            GravityBody gravityBody = null;

            var planetGenerator = new TGenerator();

            await RunAsyncWithScheduler(() =>
            {
                planetObject = SpawnUtils.SpawnPrefab(prefab);
                planet = planetObject.GetComponent<Planet>();
                gravityBody = planetObject.GetComponent<GravityBody>();
            }, mainScheduler);

            var mainRandom = new Random(seed);
            var colorRandom = new Random(seed);

            var shapeTask = Task.Run(async () =>
            {
                await GenerateShape(planet, planetGenerator, mainRandom, mainScheduler);
                await RunAsyncWithScheduler(() => CalculateGravity(planet, mainRandom), mainScheduler);
            });
            var colorTask = GenerateColor(planet, colorRandom, mainScheduler);

            Task.WaitAll(shapeTask, colorTask);
            await RunAsyncWithScheduler(() => CustomGeneration(planetObject, mainRandom), mainScheduler);

            await planetGenerator.GeneratePlanet(planet, mainScheduler);
            return gravityBody;
        }

        private async Task GenerateShape(Planet planet, PlanetGeneratorJob planetGenerator, Random random,
            TaskScheduler main)
        {
            var noiseLayers = new ShapeSettings.NoiseLayer[parameters.noiseLayersRange.GetRandomValue(random)];
            for (int i = 0, noiseSettingsLength = noiseLayers.Length + 1; i < noiseLayers.Length; i++)
            {
                noiseLayers[i] = new ShapeSettings.NoiseLayer(
                    random.Next(0, i) > i / 2f,
                    GenerateNoiseSettings(i + 1, noiseSettingsLength, random)
                );
            }

            var shapeSettings = planet.shapeSettings =
                await RunAsyncWithScheduler(ScriptableObject.CreateInstance<ShapeSettings>, main);
            shapeSettings.planetRadius = parameters.planetRadiusRange.GetRandomValue(random);
            shapeSettings.noiseLayers = noiseLayers;
            planetGenerator.resolution =
                (int) (shapeSettings.planetRadius / parameters.planetRadiusRange.to * MaxResolution);
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
            var layersCount =
                Mathf.CeilToInt(parameters.layersInNoiseCountRange.GetRandomValue(random) * limitModifier);
            var strength = parameters.strengthRange.GetRandomValue(random) * limitModifier / layersCount;
            var simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings(
                strength,
                layersCount,
                parameters.baseRoughnessRange.GetRandomValue(random) * limitModifier,
                parameters.roughnessRange.GetRandomValue(random) * limitModifier,
                parameters.persistenceRange.GetRandomValue(random) * limitModifier,
                parameters.centerMagnitudeRange.GetRandomValue(random) * random.OnUnitSphere(),
                strength * zeroOneRange.GetRandomValue(random) * downModifier
            );

            NoiseSettings.RigidNoiseSettings rigidNoiseSettings = null;
            if (filterType == NoiseSettings.FilterType.Rigid)
            {
                rigidNoiseSettings = simpleNoiseSettings.ToRigid(parameters.weightRange.GetRandomValue(random));
            }

            return new NoiseSettings(filterType, simpleNoiseSettings, rigidNoiseSettings);
        }

        private void CalculateGravity(Planet planet, Random random)
        {
            var planetRigidbody = planet.GetComponent<Rigidbody>();
            planetRigidbody.mass = planet.shapeSettings.planetRadius
                * planet.shapeSettings.planetRadius
                * parameters.massMultiplierRange.GetRandomValue(random);
            planetRigidbody.AddTorque(random.OnUnitSphere() * parameters.angularVelocityRange.GetRandomValue(random),
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