using System;
using System.Collections.Generic;
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
        _noiseFilters = new INoiseFilter[_settings.NoiseLayers.Length];

        for (var i = 0; i < _noiseFilters.Length; i++)
        {
            _noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(_settings.NoiseLayers[i].NoiseSettings);
        }

        ElevationMinMax = new MinMax();
    }

    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (_noiseFilters.Length > 0)
        {
            firstLayerValue = _noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (_settings.NoiseLayers[0].isEnabled)
            {
                elevation = firstLayerValue;
            }
        }

        for (int i = 1; i < _noiseFilters.Length; i++)
        {
            if (_settings.NoiseLayers[i].isEnabled)
            {
                float mask = _settings.NoiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += _noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
                // Debug.Log($"{i} {_noiseFilters[i].Evaluate(pointOnUnitSphere) * mask}");
            }
        }
        // Debug.Log($"{elevation}");

        ElevationMinMax.AddValue(elevation);
        return elevation;
    }

    /*public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        var elavation = 0f;
        if (_noiseFilters.Length > 0)
        {
            firstLayerValue = _noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (_settings.NoiseLayers[0].isEnabled)
            {
                elavation = firstLayerValue;
            }
        }

        elavation += _noiseFilters
            .Zip(_settings.NoiseLayers, (filter, layer) => new {NoiseFilter = filter, NoiseLayer = layer})
            .Skip(1)
            .Where(noise => noise.NoiseLayer.isEnabled)
            .Select(noise =>
            {
                var mask = noise.NoiseLayer.useFirstLayerAsMask ? firstLayerValue : 1;
                return noise.NoiseFilter.Evaluate(pointOnUnitSphere) * mask;
            })
            .Sum();


        ElevationMinMax.AddValue(elavation);
        return elavation;
    }*/

    public float GetScaledElevation(float unscaledElevation)
    {
        var elevation = Mathf.Max(0, unscaledElevation);
        elevation = _settings.planetRadius * (1 + elevation);
        return elevation;
    }
}