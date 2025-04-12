using UnityEngine;

[System.Serializable]
public class GameSettingsModel
{
    public enum GraphicsQuality { Low, Medium, High }
    public enum FrameRate { FPS30 = 30, FPS60 = 60, FPS144 = 144, Unlimited = -1 }
    public enum WindowMode { Windowed, Borderless, Fullscreen }

    public GraphicsQuality Quality = GraphicsQuality.High;
    public bool LensFlare = true;
    public FrameRate FrameRateLimit = FrameRate.FPS60;

    public WindowMode Mode = WindowMode.Fullscreen;
    public Vector2Int Resolution = new Vector2Int(1920, 1080);

    [Range(0, 1)] public float MasterVolume = 1f;
    [Range(0, 1)] public float MusicVolume = 0.8f;
    [Range(0, 1)] public float SfxVolume = 0.8f;
    [Range(0, 1)] public float AmbientVolume = 0.8f;
}