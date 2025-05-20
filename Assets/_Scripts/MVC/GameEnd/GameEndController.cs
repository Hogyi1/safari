using UnityEngine;

/// <summary>
/// Connects the pure model and the view, driving the game-end update loop based on configured thresholds.
/// </summary>
/// <remarks>
/// Attach this component alongside a <see cref="GameEndView"/> on a GameObject to handle win/lose UI flow.
/// </remarks>
public class GameEndController : MonoBehaviour
{
    /// <summary>
    /// Mood threshold at or below which the game is considered lost.
    /// </summary>
    [Header("Thresholds")]
    [Tooltip("Mood threshold at or below which the game is lost.")]
    [SerializeField] private float moodThreshold = 30f;

    /// <summary>
    /// Money threshold at or below which the game is considered lost.
    /// </summary>
    [Tooltip("Money threshold at or below which the game is lost.")]
    [SerializeField] private int moneyThreshold = 0;

    /// <summary>
    /// Challenge ID whose completion or collection triggers a win.
    /// </summary>
    [Tooltip("ID of the trophy challenge that triggers a win.")]
    [SerializeField] private int trophyChallengeId = 8;

    /// <summary>
    /// Reference to the <see cref="GameEndView"/> for UI interactions.
    /// </summary>
    private GameEndView view;

    /// <summary>
    /// Pure logic model evaluating win/lose conditions.
    /// </summary>
    private GameEndModel model;

    /// <summary>
    /// Initializes the <see cref="GameEndModel"/> and retrieves the <see cref="GameEndView"/> component.
    /// </summary>
    private void Awake()
    {
        model = new GameEndModel(moodThreshold, moneyThreshold, trophyChallengeId);
        view = GetComponent<GameEndView>();
    }

    /// <summary>
    /// Subscribes to model events and view button clicks when the controller is enabled.
    /// </summary>
    private void OnEnable()
    {
        model.GameLost += HandleGameLost;
        model.GameWon += HandleGameWon;
        view.CloseParkClicked += HandleClosePark;
        view.ContinueClicked += HandleContinue;
    }

    /// <summary>
    /// Unsubscribes from model events and view button clicks when the controller is disabled.
    /// </summary>
    private void OnDisable()
    {
        model.GameLost -= HandleGameLost;
        model.GameWon -= HandleGameWon;
        view.CloseParkClicked -= HandleClosePark;
        view.ContinueClicked -= HandleContinue;
    }

    /// <summary>
    /// Called once per frame to evaluate win/lose conditions via the model.
    /// </summary>
    private void Update()
    {
        model.CheckConditions();
    }

    /// <summary>
    /// Responds to the model's GameLost event by displaying the loss UI and pausing the game.
    /// </summary>
    private void HandleGameLost()
    {
        view.ShowLostGame();
        TimeManager.Instance.PauseTime();
    }

    /// <summary>
    /// Responds to the model's GameWon event by displaying the win UI and pausing the game.
    /// </summary>
    private void HandleGameWon()
    {
        view.ShowWonGame();
        TimeManager.Instance.PauseTime();
    }

    /// <summary>
    /// Handles the "Close Park" button click: resumes time and loads the Main Menu scene.
    /// </summary>
    private void HandleClosePark()
    {
        TimeManager.Instance.ResumeTime();
        // DataPersistenceManager.Instance.DeleteGame();
        SceneLoadManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Handles the "Continue" button click: resumes time and hides the win UI.
    /// </summary>
    private void HandleContinue()
    {
        TimeManager.Instance.ResumeTime();
        view.HideWonGame();
    }
}