using System;
using Noise;
using Settings;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace PlanetSystemGenerators
{
    public abstract class PlanetGeneratorAbstract : MonoBehaviour
    {
        protected readonly FloatRange ZeroOneRange = new FloatRange(0f, 1f);

        [SerializeField] private Object prefab;
        [SerializeField] private Shader shader;

        [SerializeField] public FloatRange planetRadiusRange;
        [SerializeField] private IntRange noiseLayersRange;

        [Space(20)]
        [Header("Noise")]
        [SerializeField] private FloatRange strengthRange;

        [SerializeField] private IntRange layersInNoiseCountRange;
        [SerializeField] private FloatRange baseRoughnessRange;
        [SerializeField] private FloatRange roughnessRange;
        [SerializeField] private FloatRange persistenceRange;
        [SerializeField] private FloatRange centerMagnitudeRange;
        [SerializeField] private FloatRange weightRange;


        [Space(20)]
        [Header("Gravity")]
        [SerializeField] private FloatRange massMultiplierRange;

        [SerializeField] private FloatRange angularVelocityRange;


        private void Awake()
        {
            if (prefab == null)
            {
                throw new NullReferenceException("Set prefab");
            }

            if (shader == null)
            {
                throw new NullReferenceException("Set shader");
            }
        }

        public GravityBody Generate()
        {
            var planetObject = Utils.SpawnPrefab(prefab);
            var planet = planetObject.GetComponent<Planet>();

            #region Shape

            var noiseLayers = new ShapeSettings.NoiseLayer[noiseLayersRange.Random()];
            for (var i = 0; i < noiseLayers.Length; i++)
            {
                noiseLayers[i] =
                    new ShapeSettings.NoiseLayer(Random.Range(0f, i) > i / 2f,
                        GenerateNoiseSettings(i + 1, noiseLayers.Length + 1));
            }

            planet.shapeSettings = ScriptableObject.CreateInstance<ShapeSettings>();
            planet.shapeSettings.planetRadius = Random.Range(planetRadiusRange.from, planetRadiusRange.to);
            planet.shapeSettings.noiseLayers = noiseLayers;
            planet.resolution = Mathf.FloorToInt(planet.shapeSettings.planetRadius / planetRadiusRange.to * 256);

            #endregion

            #region Color //ToDo triad or other color generator

            planet.colorSettings = ScriptableObject.CreateInstance<ColorSettings>();
            planet.colorSettings.planetMaterial = new Material(shader);
            planet.colorSettings.biomeColorSettings = GenerateBiomeSettings();
            planet.colorSettings.oceanGradient = GenerateOceanGraident();

            #endregion

            #region Gravity

            var planetRigidbody = planet.GetComponent<Rigidbody>();
            planetRigidbody.mass = planet.shapeSettings.planetRadius
                                   * planet.shapeSettings.planetRadius
                                   * massMultiplierRange.Random();
            planetRigidbody.AddTorque(Random.onUnitSphere * angularVelocityRange.Random(), ForceMode.VelocityChange);

            #endregion

            CustomGeneration(planetObject);

            var gravityBody = planet.GetComponent<GravityBody>();
            return gravityBody;
        }

        protected abstract ColorSettings.BiomeColorSettings GenerateBiomeSettings();
        protected abstract Gradient GenerateOceanGraident();

        protected virtual void CustomGeneration(GameObject planet)
        {
        }


        private NoiseSettings GenerateNoiseSettings(int i, int length)
        {
            var downModifier = (float) (length - i) / length;
            var limitModifier = (float) i / length;

            return GenerateNoiseSettings(downModifier, limitModifier);
        }

        protected NoiseSettings GenerateNoiseSettings(float downModifier = 1, float limitModifier = 1)
        {
            var filterType =
                (NoiseSettings.FilterType) Mathf.FloorToInt(
                    Random.Range(0f, Enum.GetValues(typeof(NoiseSettings.FilterType)).Length - 0.75f));
            var strength = strengthRange.Random() * limitModifier;
            var simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings(
                strength,
                Mathf.CeilToInt(layersInNoiseCountRange.Random() * limitModifier),
                baseRoughnessRange.Random() * limitModifier,
                roughnessRange.Random() * limitModifier,
                persistenceRange.Random() * limitModifier,
                Random.onUnitSphere * centerMagnitudeRange.Random(),
                strength * ZeroOneRange.Random() * downModifier / ZeroOneRange.Random()
            );

            NoiseSettings.RigidNoiseSettings rigidNoiseSettings = null;
            if (filterType == NoiseSettings.FilterType.Rigid)
            {
                rigidNoiseSettings = simpleNoiseSettings.ToRigid(weightRange.Random());
            }

            return new NoiseSettings(filterType, simpleNoiseSettings, rigidNoiseSettings);
        }
    }
}