using System;

namespace Noise
{
    public static class NoiseFilterFactory
    {
        public static INoiseFilter CreateNoiseFilter(NoiseSettings settings) => settings.filterType switch
        {
            NoiseSettings.FilterType.Simple => new SimpleNoiseFilter(settings.simpleNoiseSettings),
            NoiseSettings.FilterType.Rigid => new RigidNoiseFilter(settings.rigidNoiseSettings),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
