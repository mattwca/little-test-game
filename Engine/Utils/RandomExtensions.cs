using System;

namespace Engine.Utils;

public static class RandomExtensions
{
    public static float NextFloat(this Random random, float min, float max)
    {
        return min + (max - min) * random.NextSingle();
    }
}
