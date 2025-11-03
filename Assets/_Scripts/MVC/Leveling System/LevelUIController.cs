using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controller for the leveling UI. Updates the current level display,
/// the remaining experience text, and the filled progress bar.
/// Implements ILevelObserver to respond to experience-gain events.
/// </summary>
public class LevelUIController : MonoBehaviour, ILevelObserver
{
    /// <summary>
    /// TMP text showing the player’s current level.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI levelText;

    /// <summary>
    /// TMP text showing how much experience remains until the next level.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI expRemainingText;

    /// <summary>
    /// UI Image (Fill Type = Filled) used as the experience progress bar.
    /// </summary>
    [SerializeField]
    private Image expProgressBar;

    /// <summary>
    /// Time in seconds that the progress bar takes to animate from old to new value.
    /// </summary>
    private float fillAnimationDuration = 0.3f;

    private void Start()
    {
        GameEvents.Instance.AddObserver(this);
        UpdateUI();
    }

    /// <summary>
    /// Called when this component is disabled or destroyed. Unregisters
    /// from level events to avoid memory leaks.
    /// </summary>
    private void OnDisable()
    {
        if (GameEvents.Instance != null)
            GameEvents.Instance.RemoveObserver(this);
    }

    /// <summary>
    /// ILevelObserver callback invoked by GameEvents on level events.
    /// </summary>
    /// <param name="eventType">Type of the event (for example, EXP_GAIN).</param>
    /// <param name="amount">Associated amount (for example, how much exp was gained).</param>
    public void OnNotify(EventType eventType, int amount)
    {
        if (eventType == EventType.EXP_ADD)
            StartCoroutine(DelayedUpdate());
    }

    /// <summary>
    /// Waits one frame so LevelManager has finished processing before updating the UI.
    /// </summary>
    /// <returns>Coroutine yield instruction for waiting one frame.</returns>
    private IEnumerator DelayedUpdate()
    {
        yield return null;
        UpdateUI();
    }

    /// <summary>
    /// Reads values from LevelManager and updates the level text,
    /// the experience-remaining text, and the progress bar fill.
    /// </summary>
    private void UpdateUI()
    {
        int lvl = LevelManager.Instance.CurrentLevel;
        int currExp = LevelManager.Instance.CurrentExp;
        int reqExp = LevelManager.Instance.RequiredExp;

        int remaining = Mathf.Max(0, reqExp - currExp);

        // Update level text
        levelText.text = "Level " + lvl.ToString();

        // Update remaining exp text
        expRemainingText.text = LevelManager.Instance.IsMaxLevel ? "MAX LVL" : "EXP " + remaining.ToString();

        // Update progress bar
        float target = LevelManager.Instance.IsMaxLevel ? 1f : (reqExp > 0) ? (currExp / (float)reqExp) : 0f;

        Animate.Instance.ProgressBarAnim(expProgressBar, target, fillAnimationDuration);
    }
}
