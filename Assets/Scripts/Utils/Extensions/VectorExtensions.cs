using UnityEngine;

namespace Utils.Extensions
{
    public static class VectorExtensions
    {
        public static void Deconstruct(this Vector2 vector, out float x, out float y)
        {
            x = vector.x;
            y = vector.y;
        }
    }
}
