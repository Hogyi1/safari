using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputDeviceDetector : MonoBehaviour
{
    public enum InputDeviceType { None, MouseKeyboard, Gamepad }

    public static event Action<InputDeviceType> OnDeviceChanged;

    public static InputDeviceType LastUsedDevice { get; private set; } = InputDeviceType.None;

    private void OnEnable()
    {
        InputSystem.onEvent += DetectDevice;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= DetectDevice;
    }

    private void DetectDevice(InputEventPtr eventPtr, InputDevice device)
    {
        if (device is Gamepad)
            SetDevice(InputDeviceType.Gamepad);
        else if (device is Keyboard || device is Mouse)
            SetDevice(InputDeviceType.MouseKeyboard);
    }

    private void SetDevice(InputDeviceType device)
    {
        if (LastUsedDevice != device)
        {
            LastUsedDevice = device;
            OnDeviceChanged?.Invoke(device);
        }
    }
}
