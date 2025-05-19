using System;
using System.Collections.Generic;
using static UIKeys;

/// <summary>
/// Data model representing a vehicle, including capacity, assigned tourists, and state.
/// </summary>
public class Vehicle : ISelectable, ISaveable<VehicleSaveData>
{
    public int ID;
    private VehicleModel model;
    private VehicleView view;
    public VehicleModel Model => model;
    public VehicleView View => view;

    public Vehicle(int ID, VehicleModel model, VehicleView view)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;

        this.view.Init(this.model);
    }

    public BuildingType GetBuildingType() => BuildingType.None;
    public int GetID() => ID;
    public Dictionary<UIKeys, object> GetUIData()
    {
        return new Dictionary<UIKeys, object> {
            { Name_text, Model.Type.ToString() },
            { Sprite_icon, Model.Icon },
            { Value_slider, new Func<float>(() => Model.AssignedTouristIDs.Count) },
            { MaxValue_slider, Model.Capacity },
            { UIKeys.Vehicle, true },
            // { Pickup_action, new Action(() => { VehicleManager.Instance.RemoveVehicle(ID); PopupManager.Instance.HidePopup(); })}, Ide ki lehetne találni, hogy felvegyük-e vagy sem
        };
    }

    public VehicleSaveData GetSaveData()
    {
        return new VehicleSaveData
        {
            ID = ID,
            Type = Model.Type,
            Capacity = Model.Capacity,
            SpaceTaken = Model.SpaceTaken,
            WaitingTime = Model.WaitingTime,
            AssignedTourists = Model.AssignedTouristIDs,
            State = Model.State,
            CurrentRoute = View.CurrentRoute,
            NextState = View.NextState,
            CurrentPosition = View.transform.position,
            AnimalsInView = View.AnimalsInView
        };
    }
}

