using System.Collections.Generic;
using System.Linq;
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
    /// Handles game events by updating progress for any challenges that match the given event type.
    /// Also triggers the ALL_CHALLENGES_COMPLETED event if all challenges (except one) are completed or collected.
    /// </summary>
    /// <param name="eventType">The type of game event that occurred.</param>
    /// <param name="amount">The numerical value associated with the event, used to update challenge progress.</param>
    public void OnNotify(EventType eventType, int amount)
    {
        foreach (var challenge in challenges)
        {
            if (challenge.state == ChallengeState.IN_PROGRESS && challenge.eventType == eventType)
            {
                challenge.CalculateProgress(amount);
            }
        }

        // Check challenge state: Trophy Collector
        if (AllChallengesCompletedExceptOne(8)) GameEvents.Instance.NotifyObservers(EventType.ALL_CHALLENGES_COMPLETED, 1);
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
                    EconomyManager.Instance.AddMoney(challenge.prize);
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

    public Challenge GetChallengeById(int id)
    {
        foreach (var challenge in challenges)
        {
            if (challenge.id == id)
                return challenge;
        }
        return null;
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
    /// Determines whether all challenges - excluding the one with the specified ID - are either completed or collected.
    /// Used to check near-completion conditions for triggering related achievements or events.
    /// </summary>
    /// <param name="excludedChallengeId">The ID of the challenge to exclude from the completion check.</param>
    /// <returns>True if all other challenges are completed or collected; otherwise, false.</returns>
    public bool AllChallengesCompletedExceptOne(int excludedChallengeId)
    {
        List<Challenge> otherChallenges = challenges.Where(ch => ch.id != excludedChallengeId).ToList();

        int completedCount = otherChallenges.Count(ch => 
            ch.state == ChallengeState.COMPLETED 
            || ch.state == ChallengeState.COLLECTED);

        return completedCount == otherChallenges.Count;
    }

    public void LoadData(GameData data)
    {
        foreach (var challenge in data.challangeDataList) {
            Challenge c = GetChallengeById(challenge.id);
            c.SetState(challenge.state);
            c.progress = challenge.progress;
        }
    }

    public void SaveData(GameData data)
    {
        data.challangeDataList.Clear();
        foreach (var challenge in challenges) {
            data.challangeDataList.Add(new ChallangeData(challenge.id,challenge.state,challenge.progress));
        }
    }
}
