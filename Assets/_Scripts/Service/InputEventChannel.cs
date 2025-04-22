/*
    >> USAGE <<

    Subscribe to an event where you prefer:
    InputEventChannel.OnClick += MethodName;

    then

    Unsubscribe from an event where you prefer:
    InputEventChannel.OnClick -= MethodName;
*/

using System;
using UnityEngine;

public static class InputEventChannel
{
    public static event Action<Vector2> OnMove;
    public static event Action<Vector2> OnLook;
    public static event Action OnClick;
    public static event Action OnRightClick;
    public static event Action<float> OnZoom;
    public static event Action<Vector2> OnCameraMove;
    public static event Action<Vector2> OnCameraRotate;
    public static event Action<float> OnCameraHeight;
    public static event Action<bool> OnToggleUIMode;

    internal static void RaiseMove(Vector2 dir) => OnMove?.Invoke(dir);
    internal static void RaiseLook(Vector2 delta) => OnLook?.Invoke(delta);
    internal static void RaiseClick() => OnClick?.Invoke();
    internal static void RaiseRightClick() => OnRightClick?.Invoke();
    internal static void RaiseZoom(float zoom) => OnZoom?.Invoke(zoom);
    internal static void RaiseCameraMove(Vector2 dir) => OnCameraMove?.Invoke(dir);
    internal static void RaiseCameraRotate(Vector2 delta) => OnCameraRotate?.Invoke(delta);
    internal static void RaiseCameraHeight(float value) => OnCameraHeight?.Invoke(value);
    internal static void RaiseToggleUIMode(bool active) => OnToggleUIMode?.Invoke(active);

}