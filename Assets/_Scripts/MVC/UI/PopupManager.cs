using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton manager responsible for controlling and displaying all world-space UI popups
/// such as buildings and animals. Can be extended to support more popup types.
/// </summary>
public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance; // Singleton Instance

    [SerializeField] private BuildingPopup buildingBB; // Building BillBoard
    [SerializeField] private AnimalPopup animalUI; // Animal UI
                                                   // You can add more Popups.
                                                   // In Future: Can be optimized to make these inherited from a base class and maintain them.

    /// <summary>
    /// Initializes the singleton instance and ensures only one exists across scenes.
    /// </summary>
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Popup Handling
    /// <summary>
    /// Activates and displays the structure popup UI (e.g., for a building).
    /// Automatically positions the popup above the target object.
    /// </summary>
    /// <param name="Data">The UI data to populate the popup with.</param>
    /// <param name="go">The target GameObject the popup belongs to.</param>
    public void ActivateStructurePopup(Dictionary<StructureUIValues, object> Data, GameObject go)
    {
        if (Data != null)
        {
            buildingBB.gameObject.SetActive(true);
            buildingBB.Show();
        }
        else return;

        buildingBB.SetPopupData(Data);

        Vector3 UIPos = Vector3.zero;

        try
        {
            UIPos = go.transform.Find("Popup").position;
        }
        catch (Exception)
        {
            Debug.LogWarning("Nincsen Popup pozició beállítva, az alap beállításokat fogom használni.");
            Bounds bounds = go.GetComponentInChildren<Renderer>().bounds;
            UIPos = new Vector3(bounds.center.x, bounds.max.y + 1f, bounds.center.z);
        }

        buildingBB.transform.position = UIPos;
    }

    /// <summary>
    /// Activates and displays the animal popup UI using the animal's ID.
    /// </summary>
    /// <param name="ID">The ID of the animal.</param>
    public void ActivateAnimalPopup(int ID)
    {
        Animal Data = AnimalManager.Instance.GetAnimal(ID);

        animalUI.gameObject.SetActive(Data != null);
        animalUI.SetPopupData(Data);
    }

    /// <summary>
    /// Hides all active popups (both structure and animal).
    /// </summary>
    public void HidePopup()
    {
        if (buildingBB.isActiveAndEnabled) buildingBB.Hide();
        if (animalUI.isActiveAndEnabled) animalUI.gameObject.SetActive(false);
    }
}

/// <summary>
/// Interface that all UI components inside a structure popup must implement
/// to be initialized with dynamic data.
/// </summary>
public interface IStructureUIComponent
{
    public void TrySetup(Dictionary<StructureUIValues, object> data);
}

/// <summary>
/// Enum listing all UI value keys that can be passed to structure popup components.
/// </summary>
public enum StructureUIValues
{
    ID, // Structure ID
    Name_text, // The Name of the structure
    Refillprice_button, // Refill Price
    Upgradeprice_button, // Upgrade Price
    Value_slider, // Slider's current value
    MaxValue_slider, // Slider's max value
    Sprite_icon, // The Icon of the structure
    Level // The current level of the structure
}
