using System;

namespace MonoStacker.Source.Global;

public static class ExtendedMath
{
    public static readonly Random Rng = new();

    public static float RandomFloat(float min, float max)
    {
        return (float)(Rng.NextDouble() * (max - min)) + min;
    }
}