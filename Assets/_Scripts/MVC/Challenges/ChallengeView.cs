using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ChallengeView : MonoBehaviour, IChallengeObserver
{
    [SerializeField] private Transform challengeContainer;
    [SerializeField] private GameObject challengeItemPrefab;

    private enum ViewMode
    {
        Active,     // IN_PROGRESS + COMPLETED
        Collected   // COLLECTED
    }

    private ViewMode currentViewMode = ViewMode.Active;

    private void Start()
    {
        GameEvents.Instance.AddObserver(this);
        RefreshChallengeList();
    }

    private void OnEnable()
    {
        RefreshChallengeList();
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
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DelayedRefresh());
        }
    }

    public void RefreshChallengeList()
    {
        foreach (Transform child in challengeContainer)
        {
            Destroy(child.gameObject);
        }

        List<Challenge> filtered = new();

        foreach (var challenge in ChallengeManager.Instance.GetAllChallenges())
        {
            switch (currentViewMode)
            {
                case ViewMode.Active:
                    if (challenge.state == ChallengeState.IN_PROGRESS || challenge.state == ChallengeState.COMPLETED)
                        filtered.Add(challenge);
                    break;

                case ViewMode.Collected:
                    if (challenge.state == ChallengeState.COLLECTED)
                        filtered.Add(challenge);
                    break;
            }
        }

        foreach (Challenge challenge in filtered)
        {
            GameObject item = Instantiate(challengeItemPrefab, challengeContainer);
            ChallengeItemUI ui = item.GetComponent<ChallengeItemUI>();
            ui.Setup(challenge);
        }
    }

    public void ShowActiveTab()
    {
        currentViewMode = ViewMode.Active;
        RefreshChallengeList();
    }

    public void ShowCollectedTab()
    {
        currentViewMode = ViewMode.Collected;
        RefreshChallengeList();
    }

    // This is needed to wait until the progress amount updates in ChallengeSO (1 frame).
    private System.Collections.IEnumerator DelayedRefresh()
    {
        yield return null;
        RefreshChallengeList();
    }
}
