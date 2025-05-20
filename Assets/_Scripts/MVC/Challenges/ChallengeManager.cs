using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central manager for all challenges, handling their progress updates and reward collection.
/// </summary>
public class ChallengeManager : MonoBehaviour, IChallengeObserver, IDataPersistence
{
    /// <summary>
    /// Singleton instance of the ChallengeManager.
    /// </summary>
    public static ChallengeManager Instance { get; private set; }

    public float Priority => 0f;

    private List<Challenge> challenges = new();

    /// <summary>
    /// Ensures a single instance and persists this object across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        challenges = new List<Challenge>(Resources.LoadAll<Challenge>(""));
    }

    /// <summary>
    /// Subscribes to game events on start.
    /// </summary>
    private void Start()
    {
        GameEvents.Instance.AddObserver(this);
    }

    /// <summary>
    /// Unsubscribes from game events when destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RemoveObserver(this);
        }
    }

    /// <summary>
    /// Called when a subscribed event occurs; updates progress on matching challenges.
    /// </summary>
    /// <param name="eventType">The type of event that occurred.</param>
    /// <param name="amount">The value associated with the event.</param>
    public void OnNotify(EventType eventType, int amount)
    {
        foreach (var challenge in challenges)
        {
            if (challenge.state == ChallengeState.IN_PROGRESS && challenge.eventType == eventType)
            {
                challenge.CalculateProgress(amount);
            }
        }
    }

    /// <summary>
    /// Unlocks a locked challenge by its description, setting it to in-progress.
    /// </summary>
    /// <param name="description">The description identifying the challenge.</param>
    public void UnlockChallenge(string description)
    {
        foreach (var challenge in challenges)
        {
            if (challenge.description == description && challenge.state == ChallengeState.LOCKED)
            {
                challenge.SetState(ChallengeState.IN_PROGRESS);
                break;
            }
        }
    }

    /// <summary>
    /// Collects the reward for a completed challenge and adds the prize to the player's money.
    /// </summary>
    /// <param name="description">The description identifying the challenge.</param>
    /// <returns>True if the reward was successfully collected; otherwise false.</returns>
    public bool CollectChallenge(string description)
    {
        foreach (var challenge in challenges)
        {
            if (challenge.description == description && challenge.state == ChallengeState.COMPLETED)
            {
                if (challenge.CollectReward())
                {
                    // TODO: Add prize to currency
                    Debug.Log($"Collected {challenge.prize} coins from: {challenge.description}");
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Retrieves all challenges managed by this manager.
    /// </summary>
    /// <returns>A list of all challenges.</returns>
    public List<Challenge> GetAllChallenges()
    {
        return challenges;
    }

    /// <summary>
    /// Retrieves challenges filtered by a specific state.
    /// </summary>
    /// <param name="state">The state to filter challenges by.</param>
    /// <returns>A list of challenges matching the specified state.</returns>
    public List<Challenge> GetChallengesByState(ChallengeState state)
    {
        return challenges.FindAll(c => c.state == state);
    }

    /// <summary>
    /// Gets challenge by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Challenge</returns>
    public Challenge GetChallengeById(int id)
    {
        foreach (var challenge in challenges)
        {
            if (challenge.id == id)
                return challenge;
        }
        return null;
    }

    public IEnumerator LoadData(GameData data)
    {
        data.challengeDatas.ForEach(t =>
        {
            Challenge c = GetChallengeById(t.ID);
            c.SetState(t.State);
            c.progress = t.Progress;
        });
        yield return null;
    }

    public void SaveData(GameData data)
    {
        data.challengeDatas.Clear();
        challenges.ForEach(t => data.challengeDatas.Add(new ChallengeSaveData(t.id, t.state, t.progress)));
    }
}
