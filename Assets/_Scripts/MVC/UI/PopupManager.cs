using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private BuildingPopup buildingBB;
    [SerializeField] private AnimalPopup animalBB;

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

    public void ActivateStructurePopup(Dictionary<StructureUIValues, object> Data, GameObject go)
    {
        buildingBB.gameObject.SetActive(Data != null);
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

    public void ActivateAnimalPopup(int ID)
    {
        Animal Data = AnimalManager.Instance.GetAnimal(ID);

        animalBB.gameObject.SetActive(Data != null);
        animalBB.SetPopupData(Data);
    }


    public void HidePopup()
    {
        buildingBB.gameObject.SetActive(false);
        animalBB.gameObject.SetActive(false);
    }
}


public interface IStructureUIComponent
{
    public void TrySetup(Dictionary<StructureUIValues, object> data);
}

public enum StructureUIValues
{
    ID,
    Name_text,
    Refillprice_button,
    Upgradeprice_button,
    Value_slider,
    MaxValue_slider,
    Sprite_icon
}
