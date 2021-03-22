using UnityEngine;

namespace Noise
{
    public interface INoiseFilter
    {
        public float Evaluate(Vector3 point);
    }
}