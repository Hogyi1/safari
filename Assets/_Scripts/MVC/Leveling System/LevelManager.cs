using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, ILevelObserver
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<LevelData> levels = new();
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;
    private int requiredExp;
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
    /// Initializes the required experience for the current
    /// level and registers this manager as a level observer.
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
    /// Removes this manager from the list of level observers when destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.RemoveObserver(this);
        }
    }

    /// <summary>
    /// Receives event notifications and processes experience gain events.
    /// </summary>
    /// <param name="eventType">Type of the event received.</param>
    /// <param name="amount">Amount associated with the event (e.g., experience points).</param>
    public void OnNotify(EventType eventType, int amount)
    {
        if (eventType == EventType.EXP_GAIN)
        {
            AddExp(amount);
        }
    }

    /// <summary>
    /// Adds experience points and checks if the player should level up.
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
    /// Handles the logic for leveling up, resetting experience, 
    /// unlocking new content, and updating required experience.
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
    /// <param name="level">The LevelData containing the content to unlock.</param>
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
            //ItemManager.Instance.UnlockItem(itemId); <-- Remove comment when branch contains ItemManager
        }
    }
}
