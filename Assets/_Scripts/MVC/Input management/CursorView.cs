using UnityEngine;

/// <summary>
/// Manages the OS cursor appearance based on input device changes and user actions.
/// </summary>
public class CursorView : MonoBehaviour
{
    /// <summary>
    /// Texture used as the default cursor for mouse and keyboard input.
    /// </summary>
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;

    /// <summary>
    /// Texture used as the "look" cursor when interacting.
    /// </summary>
    [SerializeField] private Texture2D lookCursor;

    /// <summary>
    /// Hotspot position within the cursor texture.
    /// </summary>
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    /// <summary>
    /// Subscribes to input device change events.
    /// </summary>
    private void OnEnable()
    {
        InputDeviceDetector.OnDeviceChanged += HandleDeviceChange;
    }

    /// <summary>
    /// Unsubscribes from input device change events.
    /// </summary>
    private void OnDisable()
    {
        InputDeviceDetector.OnDeviceChanged -= HandleDeviceChange;
    }

    /// <summary>
    /// Updates cursor visibility and texture based on the current input device.
    /// </summary>
    /// <param name="device">The newly active input device type.</param>
    private void HandleDeviceChange(InputDeviceDetector.InputDeviceType device)
    {
        if (device == InputDeviceDetector.InputDeviceType.Gamepad)
        {
            Cursor.visible = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else if (device == InputDeviceDetector.InputDeviceType.MouseKeyboard)
        {
            Cursor.visible = true;
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        }
    }

    /// <summary>
    /// Sets the cursor texture to the lookCursor when using a mouse and keyboard.
    /// </summary>
    public void SetLookCursor()
    {
        if (InputDeviceDetector.LastUsedDevice == InputDeviceDetector.InputDeviceType.MouseKeyboard)
            Cursor.SetCursor(lookCursor, hotspot, CursorMode.Auto);
    }

    /// <summary>
    /// Resets the cursor texture to the defaultCursor when using a mouse and keyboard.
    /// </summary>
    public void SetDefaultCursor()
    {
        if (InputDeviceDetector.LastUsedDevice == InputDeviceDetector.InputDeviceType.MouseKeyboard)
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }
}
