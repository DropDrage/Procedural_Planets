using System;
using System.Runtime.CompilerServices;

namespace Utils
{
    [Serializable]
    public abstract class Range<T>
    {
        public T from;
        public T to;

        public abstract T RandomValue { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract T GetRandomValue(Random random);


        protected Range(T from, T to)
        {
            this.from = from;
            this.to = to;
        }
    }


    [Serializable]
    public class IntRange : Range<int>
    {
        public override int RandomValue => UnityEngine.Random.Range(from, to);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetRandomValue(Random random) => random.Next(from, to);


        public IntRange(int from, int to) : base(from, to)
        {
        }
    }

    [Serializable]
    public class FloatRange : Range<float>
    {
        public override float RandomValue => UnityEngine.Random.Range(from, to);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override float GetRandomValue(Random random) => random.NextFloat(from, to);


        public FloatRange(float from, float to) : base(from, to)
        {
        }
    }
}
