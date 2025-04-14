using UnityEngine;

/// <summary>
/// Egyszerű lineáris kongruens generátor (LCG) alapú egyedi ID generátor.
/// Nem megfelelő kriptográfiai elvárásoknak
/// https://www.youtube.com/watch?v=LUusa5Mhx_g
/// </summary>
public static class IDGenerator
{
    // A modulo érték – az ID-k a [0, m) tartományból kerülnek ki.
    private const int m = 100000;

    // A szorzótényező (LCG paraméter).
    private const int a = 321234523;

    // Az eltolás (más néven "increment") szintén egy LCG paraméter.
    private const int c = 1042341230;

    // A jelenlegi állapot, vagyis az utoljára generált ID. Ez alapján számítjuk a következőt.
    private static int seed = 1;

    // Egyszerre csak egy szál férhet hozzá, ne legyen versenyhelyzet
    private static readonly object LockObject = new object();

    /// <summary>
    /// Egyedi, 0 és (m-1) közé eső ID generálása.
    /// </summary>
    public static int GenerateID()
    {
        lock (LockObject)
        {
            seed = Mathf.Abs((a * seed + c) % m);
            return seed;
        }
    }
}

