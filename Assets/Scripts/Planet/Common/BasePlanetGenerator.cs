using System;
using Noise;
using Planet.Settings.Generation;
using UnityEngine;
using Utils.Extensions;
using Object = UnityEngine.Object;

namespace Planet.Common
{
    public abstract class BasePlanetGenerator<T> : MonoBehaviour where T : BasePlanetGenerationParameters
    {
        protected const int MaxResolution = 256;

        protected static readonly int FilterTypesLastIndex =
            Enum.GetValues(typeof(NoiseSettings.FilterType)).LastIndex();

        protected readonly FloatRange zeroOneRange = new(0f, 1f);

        [SerializeField] protected Object prefab;
        [SerializeField] protected Shader shader;

        [SerializeField] public T parameters;


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
