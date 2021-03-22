using UnityEngine;

namespace Noise
{
    public class RidgidNoiseFilter: NoiseFilterAbstract
    {
        protected NoiseSettings.RigidNoiseSettings _settings;

        public RidgidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
        {
            _settings = settings;
        }
        
        public override float Evaluate(Vector3 point)
        {
            var frequency = _settings.baseRoughness;
            var noiseValue = 0f;
            var amplitude = 1f;
            var weight = 1f;

            for (var i = 0; i < _settings.layersCount; i++)
            {
                var noise = 1 - Mathf.Abs(Noise.Evaluate(point * frequency + _settings.center));
                weight = noise *= noise * weight;

                noiseValue += noise * amplitude;
                frequency *= _settings.roughness;
                amplitude *= _settings.persistence;
            }

            noiseValue -= _settings.minValue;

            return noiseValue * _settings.strength;
        }
    }
}