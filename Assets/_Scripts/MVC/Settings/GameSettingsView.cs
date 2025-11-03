using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// View component for displaying and managing UI elements
/// that allow the user to adjust game settings.
/// </summary>
public class GameSettingsView : MonoBehaviour
{
    [Header("Graphics")]
    /// <summary>
    /// Dropdown for selecting graphics quality.
    /// </summary>
    public TMP_Dropdown qualityDropdown;

    /// <summary>
    /// Dropdown for toggling lens flare on/off.
    /// </summary>
    public TMP_Dropdown lensFlareDropdown;

    /// <summary>
    /// Dropdown for selecting frame rate limit.
    /// </summary>
    public TMP_Dropdown framerateDropdown;

    [Header("Display")]
    /// <summary>
    /// Dropdown for selecting the window mode.
    /// </summary>
    public TMP_Dropdown windowModeDropdown;

    /// <summary>
    /// Dropdown for selecting the screen resolution.
    /// </summary>
    public TMP_Dropdown resolutionDropdown;

    [Header("Audio")]
    /// <summary>
    /// Slider for adjusting master volume.
    /// </summary>
    public Slider masterSlider;

    /// <summary>
    /// Slider for adjusting music volume.
    /// </summary>
    public Slider musicSlider;

    /// <summary>
    /// Slider for adjusting sound effects volume.
    /// </summary>
    public Slider sfxSlider;

    /// <summary>
    /// Slider for adjusting ambient audio volume.
    /// </summary>
    public Slider ambientSlider;

    /// <summary>
    /// Populates all dropdowns with options and loads initial UI values on start.
    /// </summary>
    void Start()
    {
        PopulateDropdowns();
        LoadValuesToUI();

        qualityDropdown.onValueChanged.AddListener(value => GameSettingsController.Instance.SetGraphicsQuality((GameSettingsModel.GraphicsQuality)value));
        lensFlareDropdown.onValueChanged.AddListener(value => GameSettingsController.Instance.SetLensFlare(value == 1));
        framerateDropdown.onValueChanged.AddListener(value => GameSettingsController.Instance.SetFramerateFromDropdown(value));
        windowModeDropdown.onValueChanged.AddListener(value => GameSettingsController.Instance.SetWindowMode((GameSettingsModel.WindowMode)value));
        resolutionDropdown.onValueChanged.AddListener(value => GameSettingsController.Instance.SetResolutionByDropdownIndex(value));

        masterSlider.onValueChanged.AddListener(value => GameSettingsController.Instance.SetVolume("Master", value));
        musicSlider.onValueChanged.AddListener(value => GameSettingsController.Instance.SetVolume("Music", value));
        sfxSlider.onValueChanged.AddListener(value => GameSettingsController.Instance.SetVolume("SFX", value));
        ambientSlider.onValueChanged.AddListener(value => GameSettingsController.Instance.SetVolume("Ambient", value));
    }

    /// <summary>
    /// Fills each dropdown with the appropriate list of options.
    /// </summary>
    void PopulateDropdowns()
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(ResolutionManager.GetResolutionOptions());

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string> { "Low", "Medium", "High" });

        lensFlareDropdown.ClearOptions();
        lensFlareDropdown.AddOptions(new List<string> { "Off", "On" });

        framerateDropdown.ClearOptions();
        framerateDropdown.AddOptions(new List<string> { "30", "60", "144", "Unlimited" });

        windowModeDropdown.ClearOptions();
        windowModeDropdown.AddOptions(new List<string> { "Windowed", "Borderless", "Fullscreen" });
    }

    /// <summary>
    /// Loads current settings values into UI controls to reflect saved preferences.
    /// </summary>
    void LoadValuesToUI()
    {
        var settings = GameSettingsController.Instance.CurrentSettings;

        qualityDropdown.value = (int)settings.Quality;
        lensFlareDropdown.value = settings.LensFlare ? 1 : 0;

        switch (settings.FrameRateLimit)
        {
            case GameSettingsModel.FrameRate.FPS30: framerateDropdown.value = 0; break;
            case GameSettingsModel.FrameRate.FPS60: framerateDropdown.value = 1; break;
            case GameSettingsModel.FrameRate.FPS144: framerateDropdown.value = 2; break;
            case GameSettingsModel.FrameRate.Unlimited: framerateDropdown.value = 3; break;
        }

        windowModeDropdown.value = (int)settings.Mode;
        resolutionDropdown.value = ResolutionManager.FindResolutionIndex(settings.Resolution);

        masterSlider.value = settings.MasterVolume;
        musicSlider.value = settings.MusicVolume;
        sfxSlider.value = settings.SfxVolume;
        ambientSlider.value = settings.AmbientVolume;
    }
}