using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameSettingsView : MonoBehaviour
{
    [Header("Graphics")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown lensFlareDropdown;
    public TMP_Dropdown framerateDropdown;

    [Header("Display")]
    public TMP_Dropdown windowModeDropdown;
    public TMP_Dropdown resolutionDropdown;

    [Header("Audio")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider ambientSlider;

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