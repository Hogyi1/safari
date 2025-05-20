/// <summary>
/// Stores the state and progress of a single challenge for saving and loading.
/// </summary>
[System.Serializable]
public class ChallengeSaveData
{
    /// <summary>
    /// Unique identifier of the challenge.
    /// </summary>
    public int ID;

    /// <summary>
    /// Current state of the challenge (e.g., InProgress, Completed).
    /// </summary>
    public ChallengeState State;

    /// <summary>
    /// Progress of the challenge represented as a float between 0 and 1.
    /// </summary>
    public float Progress;

    /// <summary>
    /// Initializes a new instance of ChallengeSaveData with the given values.
    /// </summary>
    /// <param name="id">Challenge ID.</param>
    /// <param name="state">Current challenge state.</param>
    /// <param name="prog">Progress value.</param>
    public ChallengeSaveData(int id, ChallengeState state, float prog)
    {
        ID = id;
        State = state;
        Progress = prog;
    }
}
