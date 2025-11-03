using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// View component for Facility structures. Handles visual effects, user interaction,
/// upgrade visualization, and UI popup triggering.
/// Implements multiple interfaces for integration into the building system.
/// </summary>
[RequireComponent(typeof(UpgradeEffect))]
[RequireComponent(typeof(FadeEffect))]
public class FacilityView : MonoBehaviour, IPlaceable, IInteractable, IUpgradeable, IHasInteractingPosition
{
    /// <summary>
    /// The building data associated with this view (e.g., level costs, icons).
    /// </summary>
    [SerializeField] private BuildingData data;

    /// <summary>
    /// The corresponding model instance of this structure.
    /// </summary>
    private Structure MySelectable;

    /// <summary>
    /// Whether this facility is currently active (e.g., selected by player).
    /// </summary>
    private bool isActive;

    private FadeEffect fadeEffect;
    private UpgradeEffect upgradeEffect;


    /// <summary>
    /// Initializes required visual components on awake.
    /// </summary>
    private void Awake()
    {
        fadeEffect = GetComponent<FadeEffect>();
        if (fadeEffect.IsUnityNull())
            fadeEffect = gameObject.AddComponent<FadeEffect>();

        upgradeEffect = gameObject.AddComponent<UpgradeEffect>();
        if (upgradeEffect.IsUnityNull())
            upgradeEffect = gameObject.AddComponent<UpgradeEffect>();
    }


    /// <summary>
    /// Initializes this view with its corresponding model.
    /// </summary>
    /// <param name="selectable">The model this view represents.</param>
    public void Init(Structure selectable)
    {
        MySelectable = selectable;
        isActive = false;
    }


    /// <summary>
    /// Updates the view to reflect a level increase.
    /// </summary>
    /// <param name="amount">Amount of level change (usually 1).</param>
    public void LevelUp(int amount)
    {
        Facility facility = (Facility)MySelectable;
        int level = facility.GetCurrentLevel();
        upgradeEffect.RefreshView(level);
    }


    /// <summary>
    /// Updates the view to reflect a level decrease.
    /// </summary>
    /// <param name="amount">Amount of level change.</param>
    public void LevelDown(int amount)
    {
        Facility facility = (Facility)MySelectable;
        int level = facility.GetCurrentLevel();
        upgradeEffect.RefreshView(level);
    }


    /// <summary>
    /// Called when the mouse hovers over the facility. Triggers fade-in effect.
    /// </summary>
    public void OnHover()
    {
        fadeEffect.FadeIn();
    }


    /// <summary>
    /// Called when the mouse exits the facility. Triggers fade-out if inactive.
    /// </summary>
    public void OnExit()
    {
        if (!isActive)
        {
            fadeEffect.FadeOut();
        }
    }


    /// <summary>
    /// Called on click or activation. Triggers fade-in and shows popup.
    /// </summary>
    public void OnAction()
    {
        isActive = true;
        fadeEffect.FadeIn();
        PopupManager.Instance.ActivatePopup(((ISelectable)MySelectable).GetUIData(), GetGameObject());
    }


    /// <summary>
    /// Called when interaction is cancelled. Resets active state and fades out.
    /// </summary>
    public void OnCancel()
    {
        isActive = false;
        fadeEffect.FadeOut();
    }


    /// <summary>
    /// Returns the GameObject this view is attached to.
    /// </summary>
    public GameObject GetGameObject() => gameObject;


    /// <summary>
    /// Returns the unique structure ID.
    /// </summary>
    public int GetID() => MySelectable.GetID();


    /// <summary>
    /// Returns the building type of this structure.
    /// </summary>
    public BuildingType GetBuildingType() => MySelectable.GetBuildingType();


    /// <summary>
    /// Returns the model (Structure) associated with this view.
    /// </summary>
    public Structure GetStructure() => MySelectable;


    /// <summary>
    /// Returns the associated building data.
    /// </summary>
    public BuildingData GetData() => data;


    /// <summary>
    /// Returns the world position used for interaction (e.g., character movement).
    /// Tries to find a child object named "InteractPoint", falls back to bounds center.
    /// </summary>
    public Vector3 GetInteractingPosition()
    {
        Vector3 pos;
        try
        {
            pos = gameObject.transform.Find("InteractPoint").position;
        }
        catch (Exception)
        {
            Debug.LogWarning("InteractPoint not set — using default center.");
            pos = gameObject.GetComponent<Renderer>().bounds.center;
        }
        return pos;
    }


    /// <summary>
    /// Returns the world position used for spawning entities.
    /// Tries to find a child object named "Spawn", falls back to bounds center.
    /// </summary>
    public Vector3 GetSpawnPosition()
    {
        Vector3 pos;
        try
        {
            pos = gameObject.transform.Find("Spawn").position;
        }
        catch (Exception)
        {
            Debug.LogWarning("Spawn point not set — using default center.");
            pos = gameObject.GetComponent<Renderer>().bounds.center;
        }
        return pos;
    }


    /// <summary>
    /// Placeholder method for setting capacity. Currently unused.
    /// </summary>
    /// <param name="amount">The capacity value to set.</param>
    public void SetCapacity(int amount) { }
}
