using UnityEngine;

/// <summary>
/// Serializable data container for all adjustable game settings,
/// including graphics, display, and audio preferences.
/// </summary>
[System.Serializable]
public class GameSettingsModel
{
    /// <summary>
    /// Quality levels available for rendering graphics.
    /// </summary>
    public enum GraphicsQuality { Low, Medium, High }

    /// <summary>
    /// Frame rate limits supported by the game.
    /// </summary>
    public enum FrameRate { FPS30 = 30, FPS60 = 60, FPS144 = 144, Unlimited = -1 }

    /// <summary>
    /// Window display modes allowed for the application.
    /// </summary>
    public enum WindowMode { Windowed, Borderless, Fullscreen }

    /// <summary>
    /// Current selected graphics quality level.
    /// </summary>
    public GraphicsQuality Quality = GraphicsQuality.High;

    /// <summary>
    /// Enables or disables lens flare in the scene.
    /// </summary>
    public bool LensFlare = true;

    /// <summary>
    /// Maximum frame rate limit applied to the application.
    /// </summary>
    public FrameRate FrameRateLimit = FrameRate.FPS60;

    /// <summary>
    /// Current window mode (windowed, borderless, fullscreen).
    /// </summary>
    public WindowMode Mode = WindowMode.Fullscreen;

    /// <summary>
    /// Resolution width and height for the game window.
    /// </summary>
    public Vector2Int Resolution = new Vector2Int(1920, 1080);

    /// <summary>
    /// Master audio volume (0.0 to 1.0).
    /// </summary>
    [Range(0, 1)] public float MasterVolume = 1f;

    /// <summary>
    /// Music audio volume (0.0 to 1.0).
    /// </summary>
    [Range(0, 1)] public float MusicVolume = 0.8f;

    /// <summary>
    /// Sound effects audio volume (0.0 to 1.0).
    /// </summary>
    [Range(0, 1)] public float SfxVolume = 0.8f;

    /// <summary>
    /// Ambient sound audio volume (0.0 to 1.0).
    /// </summary>
    [Range(0, 1)] public float AmbientVolume = 0.8f;
}