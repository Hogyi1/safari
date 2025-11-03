using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Manages the visual list of challenges and updates it in response to game events.
/// </summary>
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

    /// <summary>
    /// Subscribes to game events and refreshes the challenge list on start.
    /// </summary>
    private void Start()
    {
        GameEvents.Instance.AddObserver(this);
        RefreshChallengeList();
    }

    /// <summary>
    /// Refreshes the challenge list when this view becomes enabled.
    /// </summary>
    private void OnEnable()
    {
        RefreshChallengeList();
    }

    /// <summary>
    /// Unsubscribes from game events when this view is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RemoveObserver(this);
        }
    }

    /// <summary>
    /// Called when an observed game event occurs; schedules a delayed refresh of the view.
    /// </summary>
    /// <param name="eventType">The type of event that occurred.</param>
    /// <param name="amount">The numerical value associated with the event.</param>
    public void OnNotify(EventType eventType, int amount)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DelayedRefresh());
        }
    }

    /// <summary>
    /// Clears and repopulates the challenge list based on the current view mode.
    /// </summary>
    public void RefreshChallengeList()
    {
        if (ChallengeManager.Instance == null)
        {
            Debug.LogWarning("ChallengeManager is not ready. Skipping RefreshChallengeList.");
            return;
        }

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

    /// <summary>
    /// Switches the view to show active (in-progress + completed) challenges.
    /// </summary>
    public void ShowActiveTab()
    {
        currentViewMode = ViewMode.Active;
        RefreshChallengeList();
    }

    /// <summary>
    /// Switches the view to show collected challenges only.
    /// </summary>
    public void ShowCollectedTab()
    {
        currentViewMode = ViewMode.Collected;
        RefreshChallengeList();
    }

    /// <summary>
    /// Waits one frame before refreshing to ensure progress updates are applied.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    private System.Collections.IEnumerator DelayedRefresh()
    {
        yield return null;
        RefreshChallengeList();
    }
}
