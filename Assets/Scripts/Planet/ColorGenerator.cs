using System.Linq;
using Noise;
using Settings;
using UnityEngine;

public class ColorGenerator
{
    private static readonly int ElevationMinMax = Shader.PropertyToID("_elevationMinMax");
    private static readonly int Texture = Shader.PropertyToID("_texture");

    private const int TextureResolution = 50;
    private const int DoubledTextureResolution = TextureResolution * 2;
    private const float TextureResolutionMinusOne = TextureResolution - 1f;

    private ColorSettings _settings;
    private Texture2D _texture;
    private INoiseFilter _biomeNoiseFilter;


    public void UpdateSettings(ColorSettings settings)
    {
        _settings = settings;

        if (_texture == null || _texture.height != settings.biomeColorSettings.biomes.Length)
        {
            _texture = new Texture2D(DoubledTextureResolution, settings.biomeColorSettings.biomes.Length,
                TextureFormat.RGBA32, false);
        }

        _biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        _settings.planetMaterial.SetVector(ElevationMinMax, new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        var heightPercent = (pointOnUnitSphere.y + 1) / 2f
                            + (_biomeNoiseFilter.Evaluate(pointOnUnitSphere) - _settings.biomeColorSettings.noiseOffset)
                            * _settings.biomeColorSettings.noiseStrength;
        var biomeIndex = 0f;
        var biomesCount = _settings.biomeColorSettings.biomes.Length;
        var blendRange = _settings.biomeColorSettings.blendAmount / 2f + .001f;

        for (var i = 0; i < biomesCount; i++)
        {
            var distance = heightPercent - _settings.biomeColorSettings.biomes[i].startHeight;
            var weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
            biomeIndex = biomeIndex * (1 - weight) + i * weight;
        }

        return biomeIndex / Mathf.Max(1, biomesCount - 1);
    }

    public void UpdateColors()
    {
        var colors = new Color[_texture.width * _texture.height];
        ParallelEnumerable.Range(0, _settings.biomeColorSettings.biomes.Length)
            .ForAll(biomeIndex =>
            {
                var biome = _settings.biomeColorSettings.biomes[biomeIndex];
                var colorIndex = biomeIndex * DoubledTextureResolution;
                for (var i = 0; i < TextureResolution; i++)
                {
                    var gradientColor = _settings.oceanGradient.Evaluate(i / TextureResolutionMinusOne);
                    colors[colorIndex++] = GenerateColorFromGradient(gradientColor, biome.tint, biome.tintPercent);
                }

                for (int i = TextureResolution; i < DoubledTextureResolution; i++)
                {
                    var gradientColor = biome.gradient.Evaluate((i - TextureResolution) / TextureResolutionMinusOne);
                    colors[colorIndex++] = GenerateColorFromGradient(gradientColor, biome.tint, biome.tintPercent);
                }
            });

        _texture.SetPixels(colors);
        _texture.Apply();
        _settings.planetMaterial.SetTexture(Texture, _texture);
    }

    private static Color GenerateColorFromGradient(Color gradientColor, Color tintColor, float tintPercent) =>
        gradientColor * (1 - tintPercent) + tintColor * tintPercent;
}