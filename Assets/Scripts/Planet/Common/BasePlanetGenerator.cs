using System;
using Noise;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Object = UnityEngine.Object;

namespace Planet.Common
{
    public abstract class BasePlanetGenerator : MonoBehaviour
    {
        protected static readonly int FilterTypesLastIndex =
            Enum.GetValues(typeof(NoiseSettings.FilterType)).LastIndex();

        protected readonly FloatRange zeroOneRange = new(0f, 1f);

        [SerializeField] protected Object prefab;
        [SerializeField] protected Shader shader;

        [SerializeField] public FloatRange planetRadiusRange;
        [SerializeField] protected IntRange noiseLayersRange;

        [Space]
        [Header("Noise")]
        [SerializeField] protected FloatRange strengthRange;

        [SerializeField] protected IntRange layersInNoiseCountRange;
        [SerializeField] protected FloatRange baseRoughnessRange;
        [SerializeField] protected FloatRange roughnessRange;
        [SerializeField] protected FloatRange persistenceRange;
        [SerializeField] protected FloatRange centerMagnitudeRange;
        [SerializeField] protected FloatRange weightRange;

        [Space]
        [Header("Gravity")]
        [SerializeField] protected FloatRange massMultiplierRange;

        [SerializeField] protected FloatRange angularVelocityRange;


        private void Awake()
        {
            if (prefab == null)
            {
                throw new NullReferenceException("Set prefab");
            }

            if (shader == null)
            {
                throw new NullReferenceException("Set shader");
            }
        }
    }
}
