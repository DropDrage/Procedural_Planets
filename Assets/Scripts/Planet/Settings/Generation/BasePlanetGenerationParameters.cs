using Planet.Common;
using UnityEngine;

namespace Planet.Settings.Generation
{
    public abstract class BasePlanetGenerationParameters : ScriptableObject
    {
        [SerializeField] public FloatRange planetRadiusRange;
        [SerializeField] public IntRange noiseLayersRange;

        [Space]
        [Header("Noise")]
        [SerializeField] public FloatRange strengthRange;

        [SerializeField] public IntRange layersInNoiseCountRange;
        [SerializeField] public FloatRange baseRoughnessRange;
        [SerializeField] public FloatRange roughnessRange;
        [SerializeField] public FloatRange persistenceRange;
        [SerializeField] public FloatRange centerMagnitudeRange;
        [SerializeField] public FloatRange weightRange;

        [Space]
        [Header("Gravity")]
        [SerializeField] public FloatRange angularVelocityRange;
        [SerializeField] public FloatRange massMultiplierRange;
    }
}
