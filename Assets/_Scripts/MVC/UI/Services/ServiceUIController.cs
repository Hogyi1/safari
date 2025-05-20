using System.Collections.Generic;
using UnityEngine;

public class ServiceUIController : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the ServiceUIController.
    /// </summary>
    public static ServiceUIController Instance;

    private List<ISelectable> selectables = new();

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

    public void GetAllSelectables()
    {
        var animals = AnimalManager.Instance.AllAnimals;
        var vehicles = VehicleManager.Instance.AllVehicles;

        animals.ForEach(t => selectables.Add((ISelectable)t));
        vehicles.ForEach(t => selectables.Add((ISelectable)t));
    }
}
