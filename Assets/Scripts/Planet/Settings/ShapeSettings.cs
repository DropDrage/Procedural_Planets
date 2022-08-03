using Noise;
using UnityEngine;
using UnityEngine.Serialization;

namespace Planet.Settings
{
    [CreateAssetMenu(fileName = "ShapeSettings", menuName = "Planet Shape", order = 0)]
    public class ShapeSettings : ScriptableObject
    {
        public float planetRadius = 1;
        [FormerlySerializedAs("NoiseLayers")] public NoiseLayer[] noiseLayers;


        public ShapeSettings(float planetRadius, NoiseLayer[] noiseLayers)
        {
            this.planetRadius = planetRadius;
            this.noiseLayers = noiseLayers;
        }


        [System.Serializable]
        public class NoiseLayer
        {
            public bool isEnabled;
            public bool useFirstLayerAsMask;

            [FormerlySerializedAs("NoiseSettings")]
            public NoiseSettings noiseSettings;


            public NoiseLayer(bool useFirstLayerAsMask, NoiseSettings noiseSettings, bool isEnabled = true)
            {
                this.isEnabled = isEnabled;
                this.useFirstLayerAsMask = useFirstLayerAsMask;
                this.noiseSettings = noiseSettings;
            }
        }
    }
}
