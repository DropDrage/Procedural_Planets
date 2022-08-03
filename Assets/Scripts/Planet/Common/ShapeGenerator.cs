using Noise;
using Planet.Settings;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Planet.Common
{
    public class ShapeGenerator
    {
        private ShapeSettings _settings;
        private INoiseFilter[] _noiseFilters;

        public MinMax elevationMinMax;


        public void UpdateSettings(ShapeSettings shapeSettings)
        {
            _settings = shapeSettings;
            _noiseFilters = new INoiseFilter[_settings.noiseLayers.Length];

            for (var i = 0; i < _noiseFilters.Length; i++)
            {
                _noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(_settings.noiseLayers[i].noiseSettings);
            }

            elevationMinMax = new MinMax();
        }

        public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
        {
            float firstLayerValue = 0;
            var elevation = 0f;
            if (_noiseFilters.IsEmpty())
            {
                firstLayerValue = _noiseFilters[0].Evaluate(pointOnUnitSphere);
                if (_settings.noiseLayers[0].isEnabled)
                {
                    elevation = firstLayerValue;
                }
            }

            var noiseLayers = _settings.noiseLayers;
            for (var i = 1; i < _noiseFilters.Length; i++)
            {
                var noiseLayer = noiseLayers[i];
                if (noiseLayer.isEnabled)
                {
                    var mask = noiseLayer.useFirstLayerAsMask ? firstLayerValue : 1;
                    elevation += _noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
                }
            }

            elevationMinMax.AddValue(elevation);
            return elevation;
        }

        public float GetScaledElevation(float unscaledElevation)
        {
            var elevation = Mathf.Max(0, unscaledElevation);
            elevation = _settings.planetRadius * (1 + elevation);
            return elevation;
        }
    }
}
