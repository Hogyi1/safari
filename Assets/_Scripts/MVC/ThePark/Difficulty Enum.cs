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
    /// <returns>An integer multiplier (1 for Easy, 2 for Normal, 3 for Hard).</returns>
    public static int GetMultiplier(this DifficultyEnum difficulty)
    {
        return difficulty switch
        {
            DifficultyEnum.EASY => 1,
            DifficultyEnum.NORMAL => 2,
            DifficultyEnum.HARD => 3,
            _ => 1
        };
    }
}
