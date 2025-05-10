using System;
using UnityEngine;

/// <summary>
/// Static channel providing input events for movement, look, clicks, zoom, camera controls,
/// UI toggle, and pause toggling.
/// </summary>
public static class InputEventChannel
{
    /// <summary>Invoked when movement input is received.</summary>
    public static event Action<Vector2> OnMove;

    /// <summary>Invoked when look input is received.</summary>
    public static event Action<Vector2> OnLook;

    /// <summary>Invoked when a primary click occurs.</summary>
    public static event Action OnClick;

    /// <summary>Invoked when a right-click occurs.</summary>
    public static event Action OnRightClick;

    /// <summary>Invoked when zoom input is received.</summary>
    public static event Action<float> OnZoom;

    /// <summary>Invoked for camera move input.</summary>
    public static event Action<Vector2> OnCameraMove;

    /// <summary>Invoked for camera rotate input.</summary>
    public static event Action<Vector2> OnCameraRotate;

    /// <summary>Invoked for camera height input.</summary>
    public static event Action<float> OnCameraHeight;

    /// <summary>Invoked when UI mode is toggled on or off.</summary>
    public static event Action<bool> OnToggleUIMode;

    /// <summary>Invoked when pause state is toggled.</summary>
    public static event Action<bool> OnPauseToggled;

    internal static void RaiseMove(Vector2 dir) => OnMove?.Invoke(dir);
    internal static void RaiseLook(Vector2 delta) => OnLook?.Invoke(delta);
    internal static void RaiseClick() => OnClick?.Invoke();
    internal static void RaiseRightClick() => OnRightClick?.Invoke();
    internal static void RaiseZoom(float zoom) => OnZoom?.Invoke(zoom);
    internal static void RaiseCameraMove(Vector2 dir) => OnCameraMove?.Invoke(dir);
    internal static void RaiseCameraRotate(Vector2 delta) => OnCameraRotate?.Invoke(delta);
    internal static void RaiseCameraHeight(float value) => OnCameraHeight?.Invoke(value);
    internal static void RaiseToggleUIMode(bool active) => OnToggleUIMode?.Invoke(active);
    internal static void RaisePauseToggled(bool isPaused) => OnPauseToggled?.Invoke(isPaused);
}
