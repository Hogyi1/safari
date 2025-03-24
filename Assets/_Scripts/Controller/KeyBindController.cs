using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class KeyBindController : MonoBehaviour
{
    public InputActionAsset inputActions;
    public Transform keybindContainer;
    public GameObject keybindPrefab;

    private KeyBindModel model;
    private Dictionary<string, KeyBindView> views = new();

    private void Awake()
    {
        model = new KeyBindModel(inputActions);
        model.OnKeyBindUpdated += UpdateKeyBindUI;
        model.LoadBindings();
    }

    private void Start()
    {
        PopulateKeybindUI();
        InputSystem.onAnyButtonPress.CallOnce(_ => DetectActiveDevice());
    }

    private void PopulateKeybindUI()
    {
        foreach (var action in model.GetBindings())
        {
            GameObject entry = Instantiate(keybindPrefab, keybindContainer);
            KeyBindView keyBindView = entry.GetComponent<KeyBindView>();
            keyBindView.Initialize(action.Key, action.Value.bindings[0].effectivePath, this);
            views[action.Key] = keyBindView;
        }
    }

    public void StartRebind(string actionName)
    {
        model.RebindKey(actionName);
    }

    private void UpdateKeyBindUI(string actionName, string newBinding)
    {
        if (views.TryGetValue(actionName, out var view))
        {
            view.UpdateKeyDisplay(newBinding);
        }
    }

    private void DetectActiveDevice()
    {
        InputDevice device = InputSystem.GetDevice<Keyboard>();

        if (device == null)
            device = InputSystem.GetDevice<Gamepad>();

        if (device == null)
            device = InputSystem.GetDevice<Mouse>();

        if (device != null)
        {
            foreach (var view in views.Values)
            {
                view.UpdateIcon(device);
            }
        }
    }

}
