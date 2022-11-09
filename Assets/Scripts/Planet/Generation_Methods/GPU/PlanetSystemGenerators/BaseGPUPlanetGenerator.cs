using Noise;
using Planet.Common;
using Planet.Settings;
using Planet.Settings.Generation;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Random = System.Random;

namespace Planet.Generation_Methods.GPU.PlanetSystemGenerators
{
    public abstract class BaseGPUPlanetGenerator<T> : BasePlanetGenerator<T> where T : BasePlanetGenerationParameters
    {
        [Space]
        [SerializeField] private ComputeShader trianglesGenerationShader;


        public GravityBody Generate(int seed)
        {
            var planetObject = SpawnUtils.SpawnPrefab(prefab);
            var planetGenerator = new PlanetGeneratorGPU();

            var planet = planetObject.GetComponent<Planet>();
            var gravityBody = planetObject.GetComponent<GravityBody>();

            var mainRandom = new Random(seed);
            var colorRandom = new Random(seed);

            GenerateShape(planet, planetGenerator, mainRandom);
            CalculateGravity(planet, mainRandom);
            GenerateColor(planet, colorRandom);

            CustomGeneration(planetObject, mainRandom);

            Debug.Log("Parameters Done");
            planetGenerator.GeneratePlanet(planet, trianglesGenerationShader);
            Debug.Log("Planet generated");
            return gravityBody;
        }

        private void GenerateShape(Planet planet, PlanetGeneratorGPU planetGenerator, Random random)
        {
            var noiseLayers = new ShapeSettings.NoiseLayer[parameters.noiseLayersRange.GetRandomValue(random)];
            for (int i = 0, noiseSettingsLength = noiseLayers.Length + 1; i < noiseLayers.Length; i++)
            {
                noiseLayers[i] = new ShapeSettings.NoiseLayer(
                    random.Next(0, i) > i / 2f,
                    GenerateNoiseSettings(i + 1, noiseSettingsLength, random)
                );
            }

            var shapeSettings = planet.shapeSettings = ScriptableObject.CreateInstance<ShapeSettings>();
            shapeSettings.planetRadius = parameters.planetRadiusRange.GetRandomValue(random);
            shapeSettings.noiseLayers = noiseLayers;
            planetGenerator.resolution = (int) (shapeSettings.planetRadius / parameters.planetRadiusRange.to * 256);
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
            planetRigidbody.AddTorque(
                random.OnUnitSphere() * parameters.angularVelocityRange.GetRandomValue(random),
                ForceMode.VelocityChange
            );
        }

        private void GenerateColor(Planet planet, Random random)
        {
            var color = ScriptableObject.CreateInstance<ColorSettings>();
            color.planetMaterial = new Material(shader);
            var colorSettings = planet.colorSettings = color;

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
