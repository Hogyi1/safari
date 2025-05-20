/// <summary>
/// Stores the current leveling state of a player or entity, including level, experience, progress, and max-level status.
/// </summary>
[System.Serializable]
public class LevelSaveData
{
    /// <summary>
    /// The current level.
    /// </summary>
    public int CurrentLevel;

    /// <summary>
    /// The current experience points accumulated.
    /// </summary>
    public int CurrentExp;

    /// <summary>
    /// Progress toward the next level, represented as a float between 0 and 1.
    /// </summary>
    public float Progress;

    /// <summary>
    /// Whether the maximum level has been reached.
    /// </summary>
    public bool IsMaxLevel;

    /// <summary>
    /// Initializes a new instance of <see cref="LevelSaveData"/> with default starting values.
    /// </summary>
    public LevelSaveData()
    {
        this.CurrentLevel = 1;
        this.CurrentExp = 0;
        this.Progress = 0;
        this.IsMaxLevel = false;
    }
}
