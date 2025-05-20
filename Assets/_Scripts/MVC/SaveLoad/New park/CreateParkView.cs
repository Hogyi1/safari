using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// Handles the UI logic for creating a new park,
/// including input for park name and difficulty selection.
/// </summary>
public class CreateParkView : MonoBehaviour
{
    [Header("UI References")]
    /// <summary>
    /// Input field for the park name.
    /// </summary>
    public TMP_InputField parkNameInput;

    /// <summary>
    /// Dropdown for selecting the difficulty level.
    /// </summary>
    public TMP_Dropdown difficultyDropdown;

    /// <summary>
    /// Button to confirm the creation of the park.
    /// </summary>
    public Button confirmButton;

    /// <summary>
    /// Input to validate
    /// </summary>
    private string input;

    /// <summary>
    /// Subscribes the confirm button to input validation and event invocation.
    /// </summary>
    private void Start()
    {
        parkNameInput.text = "";
        confirmButton.onClick.AddListener(ConfirmPressed);
    }

    /// <summary>
    /// Validates the park name input and invokes the confirmation event if valid.
    /// Displays an alert popup if the input is invalid.
    /// </summary>
    private void ConfirmPressed()
    {
        // GameEvents.Instance.RequestAlert(false, "The park name can't be empty", 0.25f, 2.5f, 0.4f); // Nincsen a sceneben
        DifficultyEnum difficulty = (DifficultyEnum)difficultyDropdown.value;
        CreateParkManager.Instance.HandleParkConfirmed(input, difficulty);
    }

    private bool ValidateInput()
    {
        input = parkNameInput.text.Trim();
        if (string.IsNullOrEmpty(input))
            return false;
        return true;
    }

    private void LateUpdate()
    {
        if (confirmButton != null)
            confirmButton.interactable = ValidateInput();
    }
}
