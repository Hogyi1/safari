using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private BuildingPopup billboard;

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

    void Start()
    {
        billboard.gameObject.SetActive(false);
    }

    public void ActivateStructurePopup(Dictionary<UIComponent, object> Data, GameObject go)
    {
        billboard.gameObject.SetActive(Data != null);
        billboard.SetPopupData(Data);

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

        billboard.transform.position = UIPos;
    }

    public void HidePopup()
    {
        billboard.gameObject.SetActive(false);
    }
}


public interface IUIComponent
{
    public void TrySetup(Dictionary<UIComponent, object> data);
}

public enum UIComponent
{
    ID,
    Name_text,
    Refillprice_button,
    Upgradeprice_button,
    Value_slider,
    MaxValue_slider,
    Sprite_icon
}
