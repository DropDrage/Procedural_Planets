using System;
using Noise;
using Settings;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetGenerator : MonoBehaviour
{
    private const string PlanetPrefab = "Planet";

    private readonly FloatRange _zeroOneRange = new FloatRange(0f, 1f);


    [SerializeField] private Shader planetShader;

    [SerializeField] public FloatRange planetRadiusRange;
    [SerializeField] private IntRange noiseLayersRange;

    [Space(20)]
    [Header("Noise")]
    [SerializeField] private FloatRange strengthRange;

    [SerializeField] private IntRange layersRange;
    [SerializeField] private FloatRange baseRoughnessRange;
    [SerializeField] private FloatRange roughnessRange;
    [SerializeField] private FloatRange persistenceRange;
    [SerializeField] private FloatRange centerMagnitudeRange;
    [SerializeField] private FloatRange weightRange;


    [Space(20)]
    [Header("Biomes")]
    [SerializeField] private IntRange biomesRange;

    [SerializeField] private IntRange biomeColorCountRange;
    [SerializeField] private FloatRange biomeNoiseOffsetRange;
    [SerializeField] private FloatRange biomeStrengthRange;
    [SerializeField] private FloatRange biomeBlendRange;
    [SerializeField] private IntRange biomeOceanColorCountRange;


    [Space(20)]
    [Header("Gravity")]
    [SerializeField] private FloatRange massMultiplierRange;

    [SerializeField] private FloatRange angularVelocityRange;


    [Space(20)]
    [Range(0, int.MaxValue)]
    [SerializeField] private int seed = 1;


    private void Awake()
    {
        if (planetShader == null)
        {
            throw new NullReferenceException("Set planet shader");
        }
    }

    public void SetupSeed()
    {
        Random.InitState(seed);
    }

    public GravityBody Generate()
    {
        var planetObject = Utils.SpawnPrefab(PlanetPrefab);
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

        var material = new Material(planetShader);
        var biomes = new ColorSettings.BiomeColorSettings.Biome[biomesRange.Random()];
        for (var i = 0; i < biomes.Length; i++)
        {
            var colorKeys = new GradientColorKey[biomeColorCountRange.Random()];
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

        var biomeColorSettings = new ColorSettings.BiomeColorSettings(biomes, GenerateNoiseSettings(),
            biomeNoiseOffsetRange.Random(), biomeStrengthRange.Random(), biomeBlendRange.Random());


        var oceanColorKeys = new GradientColorKey[biomeOceanColorCountRange.Random()];
        for (var k = 0; k < oceanColorKeys.Length; k++) //ToDo traid or other color generator
        {
            oceanColorKeys[k].color = Random.ColorHSV(0.37f, 0.9f, 0.1f, 0.75f);
            oceanColorKeys[k].time = Random.value;
        }

        var oceanGradient = new Gradient();
        oceanGradient.colorKeys = oceanColorKeys;

        planet.colorSettings = ScriptableObject.CreateInstance<ColorSettings>();
        planet.colorSettings.planetMaterial = material;
        planet.colorSettings.biomeColorSettings = biomeColorSettings;
        planet.colorSettings.oceanGradient = oceanGradient;

        #endregion

        #region Gravity

        var planetRigidbody = planet.GetComponent<Rigidbody>();
        planetRigidbody.mass = planet.shapeSettings.planetRadius
                               * planet.shapeSettings.planetRadius
                               * massMultiplierRange.Random();
        planetRigidbody.AddTorque(Random.onUnitSphere * angularVelocityRange.Random(), ForceMode.VelocityChange);

        #endregion

        var gravityBody = planet.GetComponent<GravityBody>();
        return gravityBody;
    }


    private NoiseSettings GenerateNoiseSettings(int i, int length)
    {
        var downModifier = (float) (length - i) / length;
        var limitModifier = (float) i / length;

        return GenerateNoiseSettings(downModifier, limitModifier);
    }

    private NoiseSettings GenerateNoiseSettings(float downModifier = 1, float limitModifier = 1)
    {
        var filterType =
            (NoiseSettings.FilterType) Mathf.FloorToInt(
                Random.Range(0f, Enum.GetValues(typeof(NoiseSettings.FilterType)).Length - 0.75f));
        var strength = strengthRange.Random() * limitModifier;
        var simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings(
            strength,
            Mathf.CeilToInt(layersRange.Random() * limitModifier),
            baseRoughnessRange.Random() * limitModifier,
            roughnessRange.Random() * limitModifier,
            persistenceRange.Random() * limitModifier,
            Random.onUnitSphere * centerMagnitudeRange.Random(),
            strength * _zeroOneRange.Random() * downModifier
        );

        NoiseSettings.RigidNoiseSettings rigidNoiseSettings = null;
        if (filterType == NoiseSettings.FilterType.Rigid)
        {
            rigidNoiseSettings = simpleNoiseSettings.ToRigid(weightRange.Random());
        }

        return new NoiseSettings(filterType, simpleNoiseSettings, rigidNoiseSettings);
    }
}