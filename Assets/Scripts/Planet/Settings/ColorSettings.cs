using System;
using Noise;
using UnityEngine;
using UnityEngine.Serialization;

namespace Planet.Settings
{
    [CreateAssetMenu(fileName = "ColorSettings", menuName = "Planet Color", order = 0)]
    public class ColorSettings : ScriptableObject
    {
        public Material planetMaterial;
        public BiomeColorSettings biomeColorSettings;
        public Gradient oceanGradient;


        public ColorSettings(Material planetMaterial, BiomeColorSettings biomeColorSettings, Gradient oceanGradient)
        {
            this.planetMaterial = planetMaterial;
            this.biomeColorSettings = biomeColorSettings;
            this.oceanGradient = oceanGradient;
        }


        [Serializable]
        public class BiomeColorSettings
        {
            public Biome[] biomes;
            public NoiseSettings noise;
            public float noiseOffset;
            public float noiseStrength;

            [Range(0, 1)]
            public float blendAmount;


            public BiomeColorSettings(Biome[] biomes, NoiseSettings noise, float noiseOffset, float noiseStrength,
                float blendAmount)
            {
                this.biomes = biomes;
                this.noise = noise;
                this.noiseOffset = noiseOffset;
                this.noiseStrength = noiseStrength;
                this.blendAmount = blendAmount;
            }


            [Serializable]
            public class Biome
            {
                [FormerlySerializedAs("Gradient")]
                public Gradient gradient;

                public Color tint;

                [Range(0, 1)]
                public float startHeight;

                [Range(0, 1)]
                public float tintPercent;


                public Biome(Gradient gradient, Color tint, float startHeight, float tintPercent)
                {
                    this.gradient = gradient;
                    this.tint = tint;
                    this.startHeight = startHeight;
                    this.tintPercent = tintPercent;
                }
            }
        }
    }
}
