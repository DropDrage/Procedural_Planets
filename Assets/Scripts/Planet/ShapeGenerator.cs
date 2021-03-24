using System.Linq;
using Noise;
using UnityEngine;

public class ShapeGenerator
{
    private ShapeSettings _settings;
    private INoiseFilter[] _noiseFilters;

    public MinMax ElevationMinMax;


    public void UpdateSettings(ShapeSettings shapeSettings)
    {
        _settings = shapeSettings;
        _noiseFilters = new INoiseFilter[_settings.noiseLayers.Length];

        for (var i = 0; i < _noiseFilters.Length; i++)
        {
            _noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(_settings.noiseLayers[i].noiseSettings);
        }

        ElevationMinMax = new MinMax();
    }

    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        var elevation = 0f;
        if (_noiseFilters.Length > 0)
        {
            firstLayerValue = _noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (_settings.noiseLayers[0].isEnabled)
            {
                elevation = firstLayerValue;
            }
        }

        elevation += _noiseFilters
            .Zip(_settings.noiseLayers, (filter, layer) => new {NoiseFilter = filter, NoiseLayer = layer})
            .Skip(1)
            .Where(noise => noise.NoiseLayer.isEnabled)
            .Select(noise =>
            {
                var mask = noise.NoiseLayer.useFirstLayerAsMask ? firstLayerValue : 1;
                return noise.NoiseFilter.Evaluate(pointOnUnitSphere) * mask;
            })
            .Sum();


        ElevationMinMax.AddValue(elevation);
        return elevation;
    }

    public float GetScaledElevation(float unscaledElevation)
    {
        var elevation = Mathf.Max(0, unscaledElevation);
        elevation = _settings.planetRadius * (1 + elevation);
        return elevation;
    }
}