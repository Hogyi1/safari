using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Leveling System/LevelData")]

/// <summary>
/// ScriptableObject holding data for a specific player level, including experience requirements and unlockables.
/// </summary>
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int expRequired;
    public string[] unlockChallenges;
    public int[] unlockItemIds;
}
