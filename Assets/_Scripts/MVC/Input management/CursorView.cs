using UnityEngine;

public class CursorView : MonoBehaviour
{
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D lookCursor;
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private void OnEnable()
    {
        InputDeviceDetector.OnDeviceChanged += HandleDeviceChange;
    }

    private void OnDisable()
    {
        InputDeviceDetector.OnDeviceChanged -= HandleDeviceChange;
    }

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

    public void SetLookCursor()
    {
        if (InputDeviceDetector.LastUsedDevice == InputDeviceDetector.InputDeviceType.MouseKeyboard)
            Cursor.SetCursor(lookCursor, hotspot, CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        if (InputDeviceDetector.LastUsedDevice == InputDeviceDetector.InputDeviceType.MouseKeyboard)
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }
}
