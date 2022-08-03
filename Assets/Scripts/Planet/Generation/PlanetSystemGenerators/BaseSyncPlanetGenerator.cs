using System;
using Noise;
using Planet.Common;
using Planet.Settings;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Planet.Generation.PlanetSystemGenerators
{
    public abstract class BaseSyncPlanetGenerator : BasePlanetGenerator
    {
        public GravityBody Generate()
        {
            var planetObject = SpawnUtils.SpawnPrefab(prefab);
            var planet = planetObject.GetComponent<PlanetAutoGenerator>();

            GenerateShape(planet);
            GenerateColor(planet); //ToDo triad or other color generator
            CalculateGravity(planet);

            CustomGeneration(planetObject);

            var gravityBody = planet.GetComponent<GravityBody>();
            return gravityBody;
        }

        private void GenerateShape(PlanetAutoGenerator planet)
        {
            var noiseLayers = new ShapeSettings.NoiseLayer[noiseLayersRange.RandomValue];
            for (int i = 0, noiseSettingsLength = noiseLayers.Length + 1; i < noiseLayers.Length; i++)
            {
                noiseLayers[i] = new ShapeSettings.NoiseLayer(
                    Random.Range(0f, i) > i / 2f,
                    GenerateNoiseSettings(i + 1, noiseSettingsLength)
                );
            }

            planet.shapeSettings = ScriptableObject.CreateInstance<ShapeSettings>();
            planet.shapeSettings.planetRadius = Random.Range(planetRadiusRange.from, planetRadiusRange.to);
            planet.shapeSettings.noiseLayers = noiseLayers;
            planet.resolution = Mathf.FloorToInt(planet.shapeSettings.planetRadius / planetRadiusRange.to * 256);
        }

        private void GenerateColor(PlanetAutoGenerator planet)
        {
            planet.colorSettings = ScriptableObject.CreateInstance<ColorSettings>();
            planet.colorSettings.planetMaterial = new Material(shader);
            planet.colorSettings.biomeColorSettings = GenerateBiomeSettings();
            planet.colorSettings.oceanGradient = GenerateOceanGradient();
        }

        private void CalculateGravity(PlanetAutoGenerator planet)
        {
            var planetRigidbody = planet.GetComponent<Rigidbody>();
            planetRigidbody.mass = planet.shapeSettings.planetRadius
                                   * planet.shapeSettings.planetRadius
                                   * massMultiplierRange.RandomValue;
            planetRigidbody.AddTorque(Random.onUnitSphere * angularVelocityRange.RandomValue, ForceMode.VelocityChange);
        }

        protected abstract ColorSettings.BiomeColorSettings GenerateBiomeSettings();
        protected abstract Gradient GenerateOceanGradient();


        protected virtual void CustomGeneration(GameObject planet)
        {
        }


        private NoiseSettings GenerateNoiseSettings(int i, int length)
        {
            var downModifier = (float) (length - i) / length;
            var limitModifier = (float) i / length;

            return GenerateNoiseSettings(downModifier, limitModifier);
        }

        protected virtual NoiseSettings GenerateNoiseSettings(float downModifier = 1, float limitModifier = 1)
        {
            var filterType =
                (NoiseSettings.FilterType) Mathf.FloorToInt(
                    Random.Range(0f, Enum.GetValues(typeof(NoiseSettings.FilterType)).Length - 0.75f));
            var layersCount = Mathf.CeilToInt(layersInNoiseCountRange.RandomValue * limitModifier);
            var strength = strengthRange.RandomValue * limitModifier / layersCount;
            var simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings(
                strength,
                layersCount,
                baseRoughnessRange.RandomValue * limitModifier,
                roughnessRange.RandomValue * limitModifier,
                persistenceRange.RandomValue * limitModifier,
                Random.onUnitSphere * centerMagnitudeRange.RandomValue,
                strength * zeroOneRange.RandomValue * downModifier
            );

            NoiseSettings.RigidNoiseSettings rigidNoiseSettings = null;
            if (filterType == NoiseSettings.FilterType.Rigid)
            {
                rigidNoiseSettings = simpleNoiseSettings.ToRigid(weightRange.RandomValue);
            }

            return new NoiseSettings(filterType, simpleNoiseSettings, rigidNoiseSettings);
        }
    }
}
