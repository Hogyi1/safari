using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Handles UI elements for the end-of-game screens and raises events when buttons are clicked.
/// </summary>
public class GameEndView : MonoBehaviour
{
    /// <summary>
    /// UI panel displayed when the player loses the game.
    /// </summary>
    [SerializeField]
    private GameObject lostGameUI;

    /// <summary>
    /// UI panel displayed when the player wins the game.
    /// </summary>
    [SerializeField]
    private GameObject wonGameUI;

    /// <summary>
    /// Button that closes the park and returns to the main menu.
    /// </summary>
    [SerializeField]
    private Button closeParkButton;

    /// <summary>
    /// Button that allows the player to continue playing after winning.
    /// </summary>
    [SerializeField]
    private Button continueButton;

    /// <summary>
    /// Event invoked when the Close Park button is clicked.
    /// </summary>
    public event Action CloseParkClicked;

    /// <summary>
    /// Event invoked when the Continue button is clicked.
    /// </summary>
    public event Action ContinueClicked;

    /// <summary>
    /// Subscribes to button click events on Awake.
    /// </summary>
    private void Awake()
    {
        closeParkButton.onClick.AddListener(() => CloseParkClicked?.Invoke());
        continueButton.onClick.AddListener(() => ContinueClicked?.Invoke());
    }

    /// <summary>
    /// Displays the lost game UI panel.
    /// </summary>
    public void ShowLostGame() => lostGameUI.SetActive(true);

    /// <summary>
    /// Displays the won game UI panel.
    /// </summary>
    public void ShowWonGame() => wonGameUI.SetActive(true);

    /// <summary>
    /// Hides the won game UI panel.
    /// </summary>
    public void HideWonGame() => wonGameUI.SetActive(false);
}
