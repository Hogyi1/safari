/// <summary>
/// Represents the different states a challenge can be in.
/// </summary>
public enum ChallengeState
{
    /// <summary>The challenge has not been unlocked.</summary>
    LOCKED,
    /// <summary>The challenge is currently in progress.</summary>
    IN_PROGRESS,
    /// <summary>The challenge has been completed but reward not yet collected.</summary>
    COMPLETED,
    /// <summary>The challenge reward has been collected.</summary>
    COLLECTED
}
