using System;

[Serializable]
public class Range<T>
{
    public T from;
    public T to;

    public Range(T from, T to)
    {
        this.from = from;
        this.to = to;
    }
}


[Serializable]
public class IntRange : Range<int>
{
    public int Random() => UnityEngine.Random.Range(from, to);

    public IntRange(int from, int to) : base(from, to)
    {
    }
}

[Serializable]
public class FloatRange : Range<float>
{
    public float Random() => UnityEngine.Random.Range(from, to);

    public FloatRange(float from, float to) : base(@from, to)
    {
    }
}