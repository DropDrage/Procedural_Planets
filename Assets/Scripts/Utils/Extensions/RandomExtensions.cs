using System;
using UnityEngine;
using Random = System.Random;

namespace Utils.Extensions
{
    public static class RandomExtensions
    {
        public static float NextFloat(this Random random) => (float) random.NextDouble();

        public static float NextFloat(this Random random, float maxValue) => (float) random.NextDouble() * maxValue;

        public static float NextFloat(this Random random, float minValue, float maxValue) =>
            minValue + random.NextFloat() * Mathf.Abs(maxValue - minValue);


        public static Color ColorHSV(this Random random,
            float hueMin = 0f, float hueMax = 1f,
            float saturationMin = 0f, float saturationMax = 1f,
            float valueMin = 0f, float valueMax = 1f,
            float alphaMin = 1f, float alphaMax = 1f)
        {
            var color = Color.HSVToRGB(
                Mathf.Lerp(hueMin, hueMax, random.NextFloat()),
                Mathf.Lerp(saturationMin, saturationMax, random.NextFloat()),
                Mathf.Lerp(valueMin, valueMax, random.NextFloat()), true
            );
            color.a = Mathf.Lerp(alphaMin, alphaMax, random.NextFloat());
            return color;
        }


        public static Vector2 OnUnitCircle(this Random random)
        {
            var angle = random.NextDouble() * Math.PI;
            var x = Math.Sin(angle);
            var y = Math.Cos(angle);
            return new Vector2((float) x, (float) y);
        }

        public static Vector3 OnUnitSphere(this Random random)
        {
            var angle = random.NextDouble() * Math.PI;
            var x = Math.Sin(angle);
            var (z, y) = random.OnUnitCircle();
            return new Vector3((float) x, y, z);
        }
    }
}
