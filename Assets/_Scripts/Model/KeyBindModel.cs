using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBindModel
{
    private InputActionAsset inputActions;
    private Dictionary<string, InputAction> actionMap = new();

    public event Action<string, string> OnKeyBindUpdated; // Notify when a binding changes

    public KeyBindModel(InputActionAsset actions)
    {
        inputActions = actions;
        foreach (var map in inputActions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                actionMap[action.name] = action;
            }
        }
    }

    public Dictionary<string, InputAction> GetBindings() => actionMap;

    public void RebindKey(string actionName)
    {
        if (!actionMap.TryGetValue(actionName, out var action)) return;

        action.Disable();
        action.PerformInteractiveRebinding()
            .OnComplete(operation =>
            {
                string newBinding = action.bindings[0].effectivePath;
                action.Enable();
                operation.Dispose();

                // Implement save the new binding here...
                
                // Temporary saving solution
                PlayerPrefs.SetString(actionName, newBinding);
                PlayerPrefs.Save();

                OnKeyBindUpdated?.Invoke(actionName, newBinding); // Notify View
            })
            .Start();
    }

    public void LoadBindings()
    {
        foreach (var action in actionMap)
        {
            // Implement load existing bindings here...

            // Temporary loading solution
            string savedBinding = PlayerPrefs.GetString(action.Key, string.Empty);
            if (!string.IsNullOrEmpty(savedBinding))
            {
                action.Value.ApplyBindingOverride(0, savedBinding);
            }
        }
    }
}
