using System.Collections.Generic;
using UnityEngine;

public class ChallangeSOReseter : MonoBehaviour
{
    public static ChallangeSOReseter Instance;
    private List<Challenge> challenges = new();

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

    public void ResetChallangesToInitial()
    {
        if (challenges != null)
            challenges = new List<Challenge>(Resources.LoadAll<Challenge>(""));
       
        foreach (Challenge challenge in challenges) {
            challenge.SetState(ChallengeState.LOCKED);
            challenge.progress = 0;
        }
    }
}
