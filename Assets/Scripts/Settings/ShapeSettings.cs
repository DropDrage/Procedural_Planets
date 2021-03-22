using Noise;
using UnityEngine;


[CreateAssetMenu(fileName = "ShapeSettings", menuName = "Planet Shape", order = 0)]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1;
    public NoiseLayer[] NoiseLayers;
    
    [System.Serializable]
    public class NoiseLayer
    {
        public bool isEnabled = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings NoiseSettings;
    }
}