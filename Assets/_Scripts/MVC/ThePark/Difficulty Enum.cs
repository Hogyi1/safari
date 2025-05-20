using UnityEngine;

/// <summary>
/// Represents the available difficulty levels for the game.
/// </summary>
public enum DifficultyEnum
{
    /// <summary>
    /// Easy difficulty setting. Ideal for beginners.
    /// </summary>
    EASY,

    /// <summary>
    /// Normal difficulty setting. Balanced experience.
    /// </summary>
    NORMAL,

    /// <summary>
    /// Hard difficulty setting. For experienced players.
    /// </summary>
    HARD
}

/// <summary>
/// Provides extension methods for the DifficultyEnum type.
/// </summary>
public static class DifficultyExtensions
{
    /// <summary>
    /// Returns a multiplier value based on the selected difficulty.
    /// </summary>
    /// <param name="difficulty">The difficulty level.</param>
    /// <returns>A float multiplier (0.75 for Easy, 1 for Normal, 1.5 for Hard).</returns>
    public static float GetMultiplier(this DifficultyEnum difficulty)
    {
        return difficulty switch
        {
            DifficultyEnum.EASY => 0.75f,
            DifficultyEnum.NORMAL => 1,
            DifficultyEnum.HARD => 1.5f,
            _ => 0.75f
        };
    }
}
