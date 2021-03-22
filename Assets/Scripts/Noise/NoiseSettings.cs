using System;
using Editor;
using UnityEngine;

namespace Noise
{
    [Serializable]
    public class NoiseSettings
    {
        public enum FilterType
        {
            Simple, Rigid,
        }

        public FilterType filterType;

        [ConditionalHide("filterType", 0)]
        public SimpleNoiseSettings simpleNoiseSettings;
        [ConditionalHide("filterType", 1)]
        public RigidNoiseSettings rigidNoiseSettings;

        [Serializable]
        public class SimpleNoiseSettings
        {
            public float strength = 1f;

            [Range(1, 8)]
            public int layersCount = 1;

            public float baseRoughness = 1f;
            public float roughness = 1f;
            public float persistence = .5f;

            public Vector3 center;

            public float minValue;
        }
        
        [Serializable]
        public class RigidNoiseSettings: SimpleNoiseSettings
        {
            public float weightMultiplier = .8f;
        }
    }
}