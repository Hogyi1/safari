using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Handles UI cancellation input, allowing the main menu to reset on cancel action.
/// </summary>
public class UIInputHandler : MonoBehaviour
{
    /// <summary>
    /// The input action triggered when the cancel input is performed.
    /// </summary>
    private InputAction cancelAction;

    /// <summary>
    /// Reference to the MainMenuController to hide menus on cancel.
    /// </summary>
    [SerializeField] MainMenuController mainMenuController;

    /// <summary>
    /// Subscribes to the cancel input action on start.
    /// </summary>
    void Start()
    {
        var inputModule = EventSystem.current?.GetComponent<InputSystemUIInputModule>();

        if (inputModule != null)
        {
            cancelAction = inputModule.cancel.action;
            cancelAction.performed += ctx => OnCancel();
        }
    }

    /// <summary>
    /// Invoked when the cancel action is performed; hides all main menu panels.
    /// </summary>
    private void OnCancel()
    {
        mainMenuController.mm_HideAllMenus();
    }

    /// <summary>
    /// Unsubscribes from the cancel input action when this handler is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (cancelAction != null)
        {
            cancelAction.performed -= ctx => OnCancel();
        }
    }
}
