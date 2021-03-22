using UnityEngine;

namespace Noise
{
    public abstract class NoiseFilterAbstract: INoiseFilter
    {
        protected readonly NoiseGenerator Noise = new NoiseGenerator();


        public abstract float Evaluate(Vector3 point);
    }
}