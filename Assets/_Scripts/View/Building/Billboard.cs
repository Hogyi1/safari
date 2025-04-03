using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    public Transform cam;
    public Building MyBuilding;
    public Button refill, remove, upgrade;
    public Slider slider;
    public TextMeshProUGUI Name, ProgressText, RefillText;

    public void SetBuilding(Building building)
    {
        if (building == null) return;
        MyBuilding = building;
        SetMax();
        SetName();

        remove.onClick.RemoveAllListeners();
        remove.onClick.AddListener(() => BuildingManager.Instance.RemoveBuilding(building.GetID()));

        refill.onClick.RemoveAllListeners();

    }
    void LateUpdate()
    {
        if (MyBuilding == null) return;
        transform.LookAt(transform.position + cam.forward);

        SetProgress();
        return;
        //if (!MyBuilding.isPlant)
        //    if (EconomyManager.Instance.HasEnoughMoney(MyBuilding.RefillPrice))
        //    {
        //        refill.enabled = true;
        //    }
        //    else
        //    {
        //        refill.enabled = false;
        //    }
    }

    private void SetMax()
    {
        slider.maxValue = MyBuilding.MaxCapacity;
        slider.value = 0;
    }

    private void SetProgress()
    {
        slider.value = MyBuilding.Capacity;
        ProgressText.text = MyBuilding.GetCapacityString();
    }

    private void SetName()
    {
        Name.text = MyBuilding.name;
    }
}
