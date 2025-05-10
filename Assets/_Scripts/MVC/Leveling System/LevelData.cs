using UnityEngine;

/// <summary>
/// ScriptableObject holding data for a specific player level, including experience requirements and unlockables.
/// </summary>
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Leveling System/LevelData")]
public class LevelData : ScriptableObject
{
    /// <summary>
    /// The sequential number of this level (starting from 1).
    /// </summary>
    public int levelNumber;

    /// <summary>
    /// The amount of experience points required to reach the next level.
    /// </summary>
    public int expRequired;

    /// <summary>
    /// List of challenge descriptions to unlock upon reaching this level.
    /// </summary>
    public string[] unlockChallenges;

    /// <summary>
    /// List of item IDs to unlock upon reaching this level.
    /// </summary>
    public int[] unlockItemIds;
}
