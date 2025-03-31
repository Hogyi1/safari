using UnityEngine;

public class CursorView : MonoBehaviour
{
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D lookCursor;
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private void Start()
    {
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }

    public void SetLookCursor()
    {
        Cursor.SetCursor(lookCursor, hotspot, CursorMode.Auto);
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Implement more cursor methods here...
}
