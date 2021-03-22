using System.Linq;
using Noise;
using Settings;
using UnityEngine;

public class ColorGenerator
{
    private static readonly int ElevationMinMax = Shader.PropertyToID("_elevationMinMax");

    private const int TextureResolution = 50;

    private ColorSettings _settings;
    private Texture2D _texture;
    private INoiseFilter _biomeNoiseFilter;


    public void UpdateSettings(ColorSettings settings)
    {
        _settings = settings;

        if (_texture == null || _texture.height != settings.biomeColorSettings.Biomes.Length)
        {
            _texture = new Texture2D(TextureResolution * 2, settings.biomeColorSettings.Biomes.Length,
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
        var biomesCount = _settings.biomeColorSettings.Biomes.Length;
        var blendRange = _settings.biomeColorSettings.blendAmount / 2f + .001f;

        for (var i = 0; i < biomesCount; i++)
        {
            var distance = heightPercent - _settings.biomeColorSettings.Biomes[i].startHeight;
            var weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
            biomeIndex = biomeIndex * (1 - weight) + i * weight;
        }

        return biomeIndex / Mathf.Max(1, biomesCount - 1);
    }

    public void UpdateColors()
    {
        var colors = new Color[_texture.width * _texture.height];
        ParallelEnumerable.Range(0, _settings.biomeColorSettings.Biomes.Length)
            .ForAll(biomeIndex =>
            {
                var biome = _settings.biomeColorSettings.Biomes[biomeIndex];
                var colorIndex = biomeIndex * TextureResolution * 2;
                for (var i = 0; i < TextureResolution * 2; i++)
                {
                    var gradientColor = i < TextureResolution
                        ? _settings.oceanGradient.Evaluate(i / (TextureResolution - 1f))
                        : biome.gradient.Evaluate((i - TextureResolution) / (TextureResolution - 1f));
                    var tintColor = biome.tint;
                    colors[colorIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
                    colorIndex++;
                }
            });

        _texture.SetPixels(colors);
        _texture.Apply();
        _settings.planetMaterial.SetTexture("_texture", _texture);
    }
}