using UnityEngine;

/// <summary>
/// A ScriptableObject representing a challenge, including its progress and reward logic.
/// </summary>
[CreateAssetMenu(fileName = "NewChallenge", menuName = "Challenge System/Challenge")]
public class Challenge : ScriptableObject
{
    public int prize;
    public EventType eventType;
    public ChallengeState state;
    public string description;
    public float goal;
    public float progress;

    /// <summary>
    /// Sets the state of the challenge.
    /// </summary>
    /// <param name="newState">The new state to assign.</param>
    public void SetState(ChallengeState newState)
    {
        state = newState;
    }

    /// <summary>
    /// Increments progress by a given amount and marks as completed if the goal is reached or surpassed.
    /// </summary>
    /// <param name="amount">The amount to add to the current progress.</param>
    public void CalculateProgress(int amount)
    {
        if (state != ChallengeState.IN_PROGRESS)
            return;

        progress += amount;

        if (progress >= goal)
        {
            progress = goal;
            state = ChallengeState.COMPLETED;
        }
    }

    /// <summary>
    /// Collects the reward if the challenge is completed, updating the state accordingly.
    /// </summary>
    /// <returns>True if reward collection was successful; otherwise false.</returns>
    public bool CollectReward()
    {
        if (state == ChallengeState.COMPLETED)
        {
            state = ChallengeState.COLLECTED;
            return true;
        }
        return false;
    }

}
