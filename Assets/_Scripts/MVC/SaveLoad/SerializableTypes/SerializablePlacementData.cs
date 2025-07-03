using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializablePlacementData
{
    public List<Vector2Int> occupiedPositions;
    public int ID;
    public int PlacedObjectIndex;

    public SerializablePlacementData() { }

    public SerializablePlacementData(PlacementData original)
    {
        occupiedPositions = original.occupiedPositions;
        ID = original.ID;
        PlacedObjectIndex = original.PlacedObjectIndex;
    }

    public PlacementData ToPlacementData()
    {
        return new PlacementData(occupiedPositions, ID, PlacedObjectIndex);
    }
}
