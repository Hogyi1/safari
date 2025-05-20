using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Singleton manager responsible for controlling and displaying all world-space UI popups
/// such as buildings and animals. Can be extended to support more popup types.
/// </summary>
public class PopupManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the PopupManager.
    /// </summary>
    public static PopupManager Instance;

    /// <summary>
    /// Building popup component used for structure UI.
    /// </summary>
    [SerializeField] private WorldSpaceUI worldSpaceUI;

    /// <summary>
    /// Initializes the singleton instance and persists across scenes.
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


    /// <summary>
    /// Activates and displays the structure popup UI (e.g., for a building).
    /// Automatically positions the popup above the target object.
    /// </summary>
    /// <param name="Data">The UI data to populate the popup with.</param>
    /// <param name="go">The target GameObject the popup belongs to.</param>
    public void ActivatePopup(Dictionary<UIKeys, object> Data, GameObject go)
    {
        if (Data.IsUnityNull()) return;

        try
        {
            Transform UIPos = go.transform.Find("Popup");
            if (worldSpaceUI.CheckDistance(UIPos.transform.position)) { InputManager.Instance.DisableView(); return; }// If too far away or too close
            worldSpaceUI.SetPopupData(Data, UIPos);
        }
        catch (Exception)
        {
            Debug.LogWarning("Nincsen Popup pozici� be�ll�tva, az alap be�ll�t�sokat fogom haszn�lni.");
            Bounds bounds = go.GetComponentInChildren<Renderer>().bounds;
            Vector3 UIPos = new Vector3(bounds.center.x, bounds.max.y + 1f, bounds.center.z);
            if (worldSpaceUI.CheckDistance(UIPos)) { InputManager.Instance.DisableView(); return; } // If too far away or too close
            worldSpaceUI.SetPopupData(Data, UIPos);
        }

        worldSpaceUI.Show();
    }

    /// <summary>
    /// Hides all active popups (both structure and animal).
    /// </summary>
    public void HidePopup()
    {
        if (worldSpaceUI.isActiveAndEnabled) worldSpaceUI.Hide();
    }
}

/// <summary>
/// Interface that all UI components inside a structure popup must implement
/// to be initialized with dynamic data.
/// </summary>
public interface IUIComponent
{
    public void TrySetup(Dictionary<UIKeys, object> data);
    public void OnPopupUpdate();
}

