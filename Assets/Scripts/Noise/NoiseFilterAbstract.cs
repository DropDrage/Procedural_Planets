using UnityEngine;

namespace Noise
{
    public abstract class NoiseFilterAbstract : INoiseFilter
    {
        protected readonly NoiseGenerator Noise = new();


        public abstract float Evaluate(Vector3 point);
    }
}
