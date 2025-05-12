/* 
    >> THIS SCRIPT CONTAINS A TEMPORARY SAVING SOLUTION <<

    Be sure to change the system in the
    future, with the saving system.
    
    Look for comment:
    // Implement permanent saving solution here...
*/

using UnityEngine;

/// <summary>
/// Controller responsible for applying, saving, and loading game settings.
/// Implements a simple persistence mechanism via PlayerPrefs.
/// </summary>
public class GameSettingsController : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// Singleton instance for global access to game settings operations.
    /// </summary>
    public static GameSettingsController Instance;

    /// <summary>
    /// Current settings data used by the game.
    /// </summary>
    public GameSettingsModel CurrentSettings = new GameSettingsModel();

    /// <summary>
    /// Ensures only one instance exists and loads saved settings on awake.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    /// <summary>
    /// Applies graphical quality, frame rate, and lens flare settings.
    /// </summary>
    public void ApplyGraphicsSettings()
    {
        QualitySettings.SetQualityLevel((int)CurrentSettings.Quality);
        Application.targetFrameRate = (int)CurrentSettings.FrameRateLimit;
        RenderSettings.fog = CurrentSettings.LensFlare;
    }

    /// <summary>
    /// Applies display settings including resolution and window mode.
    /// </summary>
    public void ApplyDisplaySettings()
    {
        FullScreenMode mode = FullScreenMode.FullScreenWindow;
        switch (CurrentSettings.Mode)
        {
            case GameSettingsModel.WindowMode.Windowed: mode = FullScreenMode.Windowed; break;
            case GameSettingsModel.WindowMode.Borderless: mode = FullScreenMode.FullScreenWindow; break;
            case GameSettingsModel.WindowMode.Fullscreen: mode = FullScreenMode.ExclusiveFullScreen; break;
        }
        Screen.SetResolution(CurrentSettings.Resolution.x, CurrentSettings.Resolution.y, mode);
    }

    /// <summary>
    /// Changes the graphics quality and persists the change.
    /// </summary>
    /// <param name="quality">New graphics quality level.</param>
    public void SetGraphicsQuality(GameSettingsModel.GraphicsQuality quality)
    {
        CurrentSettings.Quality = quality;
        ApplyGraphicsSettings();
        SaveSettings();
    }

    /// <summary>
    /// Toggles lens flare on or off and saves the updated setting.
    /// </summary>
    /// <param name="enabled">Whether lens flare should be enabled.</param>
    public void SetLensFlare(bool enabled)
    {
        CurrentSettings.LensFlare = enabled;
        ApplyGraphicsSettings();
        SaveSettings();
    }

    /// <summary>
    /// Sets the frame rate limit based on dropdown index and persists it.
    /// </summary>
    /// <param name="index">Index of the selected frame rate option.</param>
    public void SetFramerateFromDropdown(int index)
    {
        GameSettingsModel.FrameRate setting = GameSettingsModel.FrameRate.FPS60;
        switch (index)
        {
            case 0: setting = GameSettingsModel.FrameRate.FPS30; break;
            case 1: setting = GameSettingsModel.FrameRate.FPS60; break;
            case 2: setting = GameSettingsModel.FrameRate.FPS144; break;
            case 3: setting = GameSettingsModel.FrameRate.Unlimited; break;
        }
        CurrentSettings.FrameRateLimit = setting;
        ApplyGraphicsSettings();
        SaveSettings();
    }

    /// <summary>
    /// Updates the window mode and persists the change.
    /// </summary>
    /// <param name="mode">Desired window mode.</param>
    public void SetWindowMode(GameSettingsModel.WindowMode mode)
    {
        CurrentSettings.Mode = mode;
        ApplyDisplaySettings();
        SaveSettings();
    }

    /// <summary>
    /// Sets resolution based on a dropdown index and applies the change.
    /// </summary>
    /// <param name="index">Index of the chosen resolution.</param>
    public void SetResolutionByDropdownIndex(int index)
    {
        Vector2Int res = ResolutionManager.GetResolutionByIndex(index);
        SetResolution(res);
    }

    /// <summary>
    /// Directly assigns a resolution, applies it, and saves settings.
    /// </summary>
    /// <param name="resolution">Vector2Int containing width and height.</param>
    public void SetResolution(Vector2Int resolution)
    {
        CurrentSettings.Resolution = resolution;
        ApplyDisplaySettings();
        SaveSettings();
    }

    /// <summary>
    /// Updates a specified audio volume slider and persists the change.
    /// </summary>
    /// <param name="type">Volume type (Master, Music, SFX, Ambient).</param>
    /// <param name="value">New volume level (0.0 to 1.0).</param>
    public void SetVolume(string type, float value)
    {
        switch (type)
        {
            case "Master": CurrentSettings.MasterVolume = value; break;
            case "Music": CurrentSettings.MusicVolume = value; break;
            case "SFX": CurrentSettings.SfxVolume = value; break;
            case "Ambient": CurrentSettings.AmbientVolume = value; break;
        }
        SaveSettings();
    }

    // Implement permanent saving solution here...
    /// <summary>
    /// Saves current settings to PlayerPrefs as JSON.
    /// </summary>
    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(CurrentSettings);
        PlayerPrefs.SetString("GameSettings", json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads settings from PlayerPrefs, or applies defaults if none exist.
    /// </summary>
    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("GameSettings"))
        {
            string json = PlayerPrefs.GetString("GameSettings");
            CurrentSettings = JsonUtility.FromJson<GameSettingsModel>(json);
        }
        ApplyGraphicsSettings();
        ApplyDisplaySettings();
    }

    public void LoadData(GameData data)
    {
        this.CurrentSettings = data.settingsModel;
        ApplyGraphicsSettings();
        ApplyDisplaySettings();
    }

    public void SaveData(GameData data)
    {
        data.settingsModel = this.CurrentSettings;
    }
}
