using System;

[Serializable]
public abstract class Range<T>
{
    public T from;
    public T to;

    public abstract T RandomValue { get; }


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


    public IntRange(int from, int to) : base(from, to)
    {
    }
}

[Serializable]
public class FloatRange : Range<float>
{
    public override float RandomValue => UnityEngine.Random.Range(from, to);


    public FloatRange(float from, float to) : base(from, to)
    {
    }
}