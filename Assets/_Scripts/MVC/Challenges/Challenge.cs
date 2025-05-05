/*
 * Note:
 * 
 * The CalculateProgress(int amount) method expects to receive 
 * the current event's amount from the observer (ChallengeManager).
 * 
 * It increments the progress and sets the challenge to COMPLETED 
 * when the goal is reached or surpassed.
*/

using UnityEngine;

[CreateAssetMenu(fileName = "NewChallenge", menuName = "Challenge System/Challenge")]
public class Challenge : ScriptableObject
{
    public int prize;
    public EventType eventType;
    public ChallengeState state;
    public string description;
    public float goal;
    public float progress;

    public void SetState(ChallengeState newState)
    {
        state = newState;
    }

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
