using UnityEngine;

namespace Noise
{
    public class SimpleNoiseFilter : NoiseFilterAbstract
    {
        private NoiseSettings.SimpleNoiseSettings _settings;


        public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
        {
            _settings = settings;
        }


        public override float Evaluate(Vector3 point)
        {
            var frequency = _settings.baseRoughness;
            var noiseValue = 0f;
            var amplitude = 1f;

            for (var i = 0; i < _settings.layersCount; i++)
            {
                var noise = Noise.Evaluate(point * frequency + _settings.center);
                noiseValue += (noise + 1) * .5f * amplitude;
                frequency *= _settings.roughness;
                amplitude *= _settings.persistence;
            }

            noiseValue -= _settings.minValue;
            return noiseValue * _settings.strength;
        }
    }
}
