using System;

/// <summary>
/// Pure model class for evaluating end-of-game conditions based on mood, money, and trophy challenge.
/// </summary>
public class GameEndModel
{
    /// <summary>
    /// Mood threshold at or below which the game is considered lost.
    /// </summary>
    private readonly float moodThreshold;

    /// <summary>
    /// Money threshold at or below which the game is considered lost.
    /// </summary>
    private readonly int moneyThreshold;

    /// <summary>
    /// Challenge ID whose completion or collection triggers a win.
    /// </summary>
    private readonly int trophyChallengeId;

    /// <summary>
    /// Tracks whether an end condition has already fired to prevent duplicates.
    /// </summary>
    private bool hasEnded;

    /// <summary>
    /// Event invoked when a loss condition is met.
    /// </summary>
    public event Action GameLost;

    /// <summary>
    /// Event invoked when a win condition is met.
    /// </summary>
    public event Action GameWon;

    /// <summary>
    /// Constructs a new <see cref="GameEndModel"/> with specified thresholds and challenge ID.
    /// </summary>
    /// <param name="moodThreshold">Mood level threshold for loss.</param>
    /// <param name="moneyThreshold">Money amount threshold for loss.</param>
    /// <param name="trophyChallengeId">Identifier for the trophy challenge that triggers a win.</param>
    public GameEndModel(float moodThreshold, int moneyThreshold, int trophyChallengeId)
    {
        this.moodThreshold = moodThreshold;
        this.moneyThreshold = moneyThreshold;
        this.trophyChallengeId = trophyChallengeId;
    }

    /// <summary>
    /// Evaluates win/lose conditions once per frame and fires the appropriate event exactly once.
    /// </summary>
    public void CheckConditions()
    {
        if (hasEnded)
            return;

        if (TouristManager.Instance.OverallMood <= moodThreshold
            || EconomyManager.Instance.GetEconomy().CurrentMoney <= moneyThreshold)
        {
            hasEnded = true;
            GameLost?.Invoke();
            return;
        }

        var challenge = ChallengeManager.Instance.GetChallengeById(trophyChallengeId);
        if (challenge.state == ChallengeState.COMPLETED
            || challenge.state == ChallengeState.COLLECTED)
        {
            hasEnded = true;
            GameWon?.Invoke();
        }
    }

    /// <summary>
    /// Resets the model to allow conditions to fire again, such as after a scene reload.
    /// </summary>
    public void Reset() => hasEnded = false;
}