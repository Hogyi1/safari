using UnityEngine;

/// <summary>
/// Simple linear congruential generator (LCG) for unique ID generation.
/// Not suitable for cryptographic applications.
/// </summary>
public static class IDGenerator
{
    // The modulus value – IDs are generated in the [0, m) range.
    private const int m = 100000;

    // The multiplier factor (an LCG parameter).
    private const int a = 321234523;

    // The increment (another LCG parameter).
    private const int c = 1042341230;

    // The current state, i.e., the last generated ID. The next ID is computed from this.
    private static int seed = 1;

    // Ensures only one thread can access at a time to prevent race conditions.
    private static readonly object LockObject = new object();

    public static void SetSeed(int newSeed) => seed = newSeed;
    public static int GetSeed() => seed;

    /// <summary>
    /// Generates a new unique ID in the range [0, m).
    /// </summary>
    /// <returns>An integer ID.</returns>
    public static int GenerateID()
    {
        lock (LockObject)
        {
            seed = Mathf.Abs((a * seed + c) % m);
            return seed;
        }
    }
}

