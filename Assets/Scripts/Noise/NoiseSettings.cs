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


        public NoiseSettings(FilterType filterType, SimpleNoiseSettings simpleNoiseSettings,
                             RigidNoiseSettings rigidNoiseSettings)
        {
            this.filterType = filterType;
            this.simpleNoiseSettings = simpleNoiseSettings;
            this.rigidNoiseSettings = rigidNoiseSettings;
        }


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


            public SimpleNoiseSettings(float strength, int layersCount, float baseRoughness, float roughness,
                                       float persistence, Vector3 center, float minValue)
            {
                this.strength = strength;
                this.layersCount = layersCount;
                this.baseRoughness = baseRoughness;
                this.roughness = roughness;
                this.persistence = persistence;
                this.center = center;
                this.minValue = minValue;
            }


            public RigidNoiseSettings ToRigid(float weight) => new RigidNoiseSettings(this, weight);
        }

        [Serializable]
        public class RigidNoiseSettings : SimpleNoiseSettings
        {
            public float weightMultiplier = .8f;


            public RigidNoiseSettings(SimpleNoiseSettings simpleNoiseSettings, float weightMultiplier)
                : base(simpleNoiseSettings.strength, simpleNoiseSettings.layersCount, simpleNoiseSettings.baseRoughness,
                    simpleNoiseSettings.roughness, simpleNoiseSettings.persistence, simpleNoiseSettings.center,
                    simpleNoiseSettings.minValue)
            {
                this.weightMultiplier = weightMultiplier;
            }
        }
    }
}