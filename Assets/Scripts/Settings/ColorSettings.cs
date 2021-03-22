using System;
using Noise;
using UnityEngine;
using UnityEngine.Serialization;

namespace Settings
{
    [CreateAssetMenu(fileName = "ColorSettings", menuName = "Planet Color", order = 0)]
    public class ColorSettings : ScriptableObject
    {
        public Material planetMaterial;
        public BiomeColorSettings biomeColorSettings;
        public Gradient oceanGradient;

        [Serializable]
        public class BiomeColorSettings
        {
            public Biome[] Biomes;
            public NoiseSettings noise;
            public float noiseOffset;
            public float noiseStrength;

            [Range(0, 1)]
            public float blendAmount;

            [Serializable]
            public class Biome
            {
                [FormerlySerializedAs("Gradient")] public Gradient gradient;
                public Color tint;

                [Range(0, 1)]
                public float startHeight;

                [Range(0, 1)]
                public float tintPercent;
            }
        }
    }
}