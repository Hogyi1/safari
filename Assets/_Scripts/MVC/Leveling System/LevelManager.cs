using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player leveling by tracking experience, handling level-up logic,
/// and unlocking content based on completed levels.
/// Implements ILevelObserver to respond to experience gain events.
/// </summary>
public class LevelManager : MonoBehaviour, ILevelObserver
{
    /// <summary>
    /// Singleton instance of the LevelManager for global access.
    /// </summary>
    public static LevelManager Instance { get; private set; }

    /// <summary>
    /// List of all level configurations, including required experience and unlock data.
    /// </summary>
    [SerializeField] private List<LevelData> levels = new();

    /// <summary>
    /// The player's current level, starting at 1.
    /// </summary>
    [SerializeField] private int currentLevel = 1;

    /// <summary>
    /// Current accumulated experience towards the next level.
    /// </summary>
    [SerializeField] private int currentExp = 0;

    /// <summary>
    /// Experience required to advance from the current level to the next.
    /// </summary>
    private int requiredExp;

    /// <summary>
    /// Normalized progress (0.0 to 1.0) towards completing the current level.
    /// </summary>
    private float progress;

    /// <summary>
    /// Ensures only one instance of LevelManager exists and persists across scenes.
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
    }


    /// <summary>
    /// Initializes the required experience for the starting level
    /// and registers this manager as a level observer.
    /// </summary>
    private void Start()
    {
        if (levels.Count > 0)
        {
            requiredExp = levels[currentLevel - 1].expRequired;
        }

        GameEvents.Instance.AddObserver(this);
    }

    /// <summary>
    /// Unregisters this manager from level events when destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RemoveObserver(this);
        }
    }

    /// <summary>
    /// Called when a game event occurs; processes experience gain events.
    /// </summary>
    /// <param name="eventType">Type of the event received.</param>
    /// <param name="amount">Amount of experience associated with the event.</param>
    public void OnNotify(EventType eventType, int amount)
    {
        if (eventType == EventType.EXP_GAIN)
        {
            AddExp(amount);
        }
    }

    /// <summary>
    /// Adds experience points, updates progress, and checks for level-up.
    /// </summary>
    /// <param name="amount">Amount of experience to add.</param>
    public void AddExp(int amount)
    {
        currentExp += amount;
        progress = (float)currentExp / requiredExp;
        Debug.Log("Current exp: " + currentExp);
        Debug.Log("Current progress: " + progress);

        if (currentExp >= requiredExp)
        {
            if ((currentLevel - 1) < levels.Count - 1)
            {
                LevelUp();
            }
            else
            {
                Debug.Log("You have reached the max level!");

                currentExp = requiredExp; // Lock exp to max
                progress = 1f;

                Debug.Log("Max level. Current exp: " + currentExp);
                Debug.Log("Max level. Current progress: " + progress);
            }
        }

    }

    /// <summary>
    /// Handles leveling up: increments level, triggers alerts, resets experience,
    /// updates required experience, and unlocks new content.
    /// </summary>
    private void LevelUp()
    {
        currentLevel++;

        // Request an alert that tells the player they leveled up and shows the current level.
        GameEvents.Instance.RequestAlert(
            success: true,
            message: "Level Up! You have reached level " + currentLevel.ToString() + "!",
            fadeInTime: 0.25f,
            displayTime: 2.5f,
            fadeOutTime: 0.4f
        );
        
        if ((currentLevel - 1) < levels.Count)
        {
            UnlockContent(levels[currentLevel - 1]);
            currentExp = 0;
            requiredExp = levels[currentLevel - 1].expRequired;
            progress = 0f;
        }
        else
        {
            Debug.Log("You are at max level.");
        }
    }

    /// <summary>
    /// Unlocks challenges and items associated with a given level.
    /// </summary>
    /// <param name="level">LevelData containing descriptions of content to unlock.</param>
    private void UnlockContent(LevelData level)
    {
        // Unlock Challenges
        foreach (var challengeDesc in level.unlockChallenges)
        {
            ChallengeManager.Instance.UnlockChallenge(challengeDesc);
        }

        // Unlock Items
        foreach (var itemId in level.unlockItemIds)
        {
            // ItemManager.Instance.UnlockItem(itemId); // Enable when ItemManager is implemented
        }
    }
}
