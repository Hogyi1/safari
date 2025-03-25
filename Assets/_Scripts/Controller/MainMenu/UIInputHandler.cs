using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class UIInputHandler : MonoBehaviour
{
    private InputAction cancelAction;
    [SerializeField] MainMenuController mainMenuController;

    void Start()
    {
        // Get the EventSystem's InputSystemUIInputModule
        var inputModule = EventSystem.current?.GetComponent<InputSystemUIInputModule>();

        if (inputModule != null)
        {
            cancelAction = inputModule.cancel.action;
            cancelAction.performed += ctx => OnCancel();
        }
    }

    private void OnCancel()
    {
        mainMenuController.mm_HideAllMenus();
    }

    private void OnDestroy()
    {
        if (cancelAction != null)
        {
            cancelAction.performed -= ctx => OnCancel();
        }
    }
}
