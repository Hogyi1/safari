using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehicleSaveData
{
    public int maxCapacity ;
    public List<VehicleType> activevehicle;
    public VehicleSaveData()
    {
        maxCapacity = 5;
        activevehicle = new List<VehicleType>();
    }
}
