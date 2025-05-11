using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// Detects the last used input device (mouse/keyboard or gamepad)
/// and raises events when the device type changes.
/// </summary>
public class InputDeviceDetector : MonoBehaviour
{
    /// <summary>
    /// Enumeration of supported input device types.
    /// </summary>
    public enum InputDeviceType { None, MouseKeyboard, Gamepad }

    /// <summary>
    /// Event invoked when the active input device type changes.
    /// </summary>
    public static event Action<InputDeviceType> OnDeviceChanged;

    /// <summary>
    /// The last detected input device type.
    /// </summary>
    public static InputDeviceType LastUsedDevice { get; private set; } = InputDeviceType.None;

    /// <summary>
    /// Subscribes to low-level input system events on enable.
    /// </summary>
    private void OnEnable()
    {
        InputSystem.onEvent += DetectDevice;
    }

    /// <summary>
    /// Unsubscribes from input system events on disable.
    /// </summary>
    private void OnDisable()
    {
        InputSystem.onEvent -= DetectDevice;
    }

    /// <summary>
    /// Callback for every input event; identifies the device type and raises change events.
    /// </summary>
    /// <param name="eventPtr">The input event data pointer.</param>
    /// <param name="device">The input device that generated the event.</param>
    private void DetectDevice(InputEventPtr eventPtr, InputDevice device)
    {
        if (device is Gamepad)
            SetDevice(InputDeviceType.Gamepad);
        else if (device is Keyboard || device is Mouse)
            SetDevice(InputDeviceType.MouseKeyboard);
    }

    /// <summary>
    /// Updates the LastUsedDevice and invokes the OnDeviceChanged event if different.
    /// </summary>
    /// <param name="device">The newly detected device type.</param>
    private void SetDevice(InputDeviceType device)
    {
        if (LastUsedDevice != device)
        {
            LastUsedDevice = device;
            OnDeviceChanged?.Invoke(device);
        }
    }
}
