/* 
    >> THIS SCRIPT CONTAINS A TEMPORARY SAVING SOLUTION <<

    Be sure to change the system in the
    future, with the saving system.
    
    Look for comment:
    // Implement permanent saving solution here...
*/

using UnityEngine;

public class GameSettingsController : MonoBehaviour
{
    public static GameSettingsController Instance;

    public GameSettingsModel CurrentSettings = new GameSettingsModel();

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

    public void ApplyGraphicsSettings()
    {
        QualitySettings.SetQualityLevel((int)CurrentSettings.Quality);
        Application.targetFrameRate = (int)CurrentSettings.FrameRateLimit;
        RenderSettings.fog = CurrentSettings.LensFlare;
    }

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

    public void SetGraphicsQuality(GameSettingsModel.GraphicsQuality quality)
    {
        CurrentSettings.Quality = quality;
        ApplyGraphicsSettings();
        SaveSettings();
    }

    public void SetLensFlare(bool enabled)
    {
        CurrentSettings.LensFlare = enabled;
        ApplyGraphicsSettings();
        SaveSettings();
    }

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

    public void SetWindowMode(GameSettingsModel.WindowMode mode)
    {
        CurrentSettings.Mode = mode;
        ApplyDisplaySettings();
        SaveSettings();
    }

    public void SetResolutionByDropdownIndex(int index)
    {
        Vector2Int res = ResolutionManager.GetResolutionByIndex(index);
        SetResolution(res);
    }

    public void SetResolution(Vector2Int resolution)
    {
        CurrentSettings.Resolution = resolution;
        ApplyDisplaySettings();
        SaveSettings();
    }

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
    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(CurrentSettings);
        PlayerPrefs.SetString("GameSettings", json);
        PlayerPrefs.Save();
    }

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
}
