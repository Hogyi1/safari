using System;

/// <summary>
/// Represents basic information about a park, including its name and difficulty level.
/// </summary>
[System.Serializable]
public class ParkData
{
    /// <summary>
    /// The name of the park.
    /// </summary>
    public string ParkName;

    /// <summary>
    /// The selected difficulty level for the park.
    /// </summary>
    public DifficultyEnum Difficulty;

    /// <summary>
    /// Default constructor initializing with a default park name and easy difficulty.
    /// </summary>
    public ParkData()
    {
        ParkName = "Default";
        Difficulty = DifficultyEnum.EASY;
    }
}
