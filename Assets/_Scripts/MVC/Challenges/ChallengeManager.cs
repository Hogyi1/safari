using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour, IChallengeObserver
{
    public static ChallengeManager Instance { get; private set; }

    [SerializeField] private List<Challenge> challenges = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameEvents.Instance.AddObserver(this);
    }

    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RemoveObserver(this);
        }
    }

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

    public List<Challenge> GetAllChallenges()
    {
        return challenges;
    }

    public List<Challenge> GetChallengesByState(ChallengeState state)
    {
        return challenges.FindAll(c => c.state == state);
    }
}
