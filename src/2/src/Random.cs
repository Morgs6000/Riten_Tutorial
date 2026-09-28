public static class Random
{
    private static readonly System.Random _rand = new System.Random();

    public static float Range(float minInclusive, float maxInclusive)
    {
        return (float)(_rand.NextDouble() * (maxInclusive - minInclusive) + minInclusive);
    }

    public static int Range(int minInclusive, int maxExclusive)
    {
        return _rand.Next(minInclusive, maxExclusive);
    }
}
