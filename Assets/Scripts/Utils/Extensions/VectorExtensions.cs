using System.Runtime.CompilerServices;
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(this ref Vector3 vector, ref Vector3 other)
        {
            vector.x += other.x;
            vector.y += other.y;
            vector.z += other.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Subtract(this ref Vector3 vector, ref Vector3 other)
        {
            vector.x -= other.x;
            vector.y -= other.y;
            vector.z -= other.z;
            return vector;
        }

        public static void Multiply(this ref Vector3 vector, ref Vector3 other)
        {
            vector.x *= other.x;
            vector.y *= other.y;
            vector.z *= other.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(this ref Vector3 vector, float other)
        {
            vector.x *= other;
            vector.y *= other;
            vector.z *= other;
        }
    }
}
