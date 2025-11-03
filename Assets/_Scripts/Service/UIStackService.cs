using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages a stack of active UI GameObjects to track which UI panel is currently on top.
/// Supports push/pop operations and controls visibility of UI elements accordingly.
/// </summary>
public class UIStackService : MonoBehaviour
{
    private static readonly Stack<GameObject> uiStack = new();

    /// <summary>
    /// Pushes a UI GameObject onto the stack and activates it.
    /// If already present in the stack, it will not be added again.
    /// </summary>
    /// <param name="uiElement">The UI GameObject to display and track.</param>
    public static void Push(GameObject uiElement)
    {
        if (!uiStack.Contains(uiElement))
        {
            uiStack.Push(uiElement);
            uiElement.SetActive(true);
        }

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Pops the top UI GameObject off the stack and deactivates it.
    /// </summary>
    public static void Pop()
    {
        if (uiStack.Count > 0)
        {
            var top = uiStack.Pop();
            if (top != null) top.SetActive(false);
        }

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Removes a specific UI GameObject from anywhere in the stack without activating or deactivating it.
    /// Use this when you want to hide a panel and prevent it from reappearing.
    /// </summary>
    /// <param name="uiElement">The UI GameObject to remove from tracking.</param>
    public static void Remove(GameObject uiElement)
    {
        if (!uiStack.Contains(uiElement))
            return;

        var buffer = new Stack<GameObject>();
        // Pop until we hit the element
        while (uiStack.Count > 0)
        {
            var top = uiStack.Pop();
            if (top == uiElement)
                break;
            buffer.Push(top);
        }
        // Push back everything else
        while (buffer.Count > 0)
            uiStack.Push(buffer.Pop());
    }

    /// <summary>
    /// Checks if there are any UI GameObjects currently active in the stack.
    /// </summary>
    /// <returns>True if one or more UI elements are open; otherwise, false.</returns>
    public static bool IsUIOpen() => uiStack.Count > 0;

    /// <summary>
    /// Returns the top-most UI GameObject currently on the stack, without removing it.
    /// </summary>
    /// <returns>The top UI GameObject, or null if the stack is empty.</returns>
    public static GameObject Peek() => uiStack.Count > 0 ? uiStack.Peek() : null;

    /// <summary>
    /// Clears all UI GameObjects from the stack and deactivates them.
    /// </summary>
    public static void Clear()
    {
        while (uiStack.Count > 0)
        {
            var top = uiStack.Pop();
            if (top != null) top.SetActive(false);
        }
    }
}
