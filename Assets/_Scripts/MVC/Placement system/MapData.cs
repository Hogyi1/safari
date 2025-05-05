using System.Collections.Generic;
using System;
using UnityEngine;

public class MapData
{
    Dictionary<Vector2Int, PlacementData> ObjectsPlacedOnGrid = new();

    public void AddObjectAt(Vector2Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector2Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (ObjectsPlacedOnGrid.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            ObjectsPlacedOnGrid[pos] = data;
        }
    }

    private List<Vector2Int> CalculatePositions(Vector2Int gridPosition, Vector2Int objectSize)
    {
        List<Vector2Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector2Int(x, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector2Int gridPosition, Vector2Int objectSize)
    {
        List<Vector2Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (ObjectsPlacedOnGrid.ContainsKey(pos))
                return false;
        }
        return true;
    }

    public void RemoveObjectAt(Vector2Int gridPosition)
    {
        foreach (var pos in ObjectsPlacedOnGrid[gridPosition].occupiedPositions)
        {
            ObjectsPlacedOnGrid.Remove(pos);
        }
    }

    internal int GetRepresentationIndex(Vector2Int gridPosition)
    {
        if (ObjectsPlacedOnGrid.ContainsKey(gridPosition) == false)
            return -1;
        return ObjectsPlacedOnGrid[gridPosition].PlacedObjectIndex;
    }

    // Visszaadja a buildingindexet, hogy meg tudjam keresni az eredeti View-t hozzá
    public int IsOccupied(Vector2Int gridPosition)
    {
        if (ObjectsPlacedOnGrid.ContainsKey(gridPosition))
            return ObjectsPlacedOnGrid[gridPosition].PlacedObjectIndex;
        return -1;
    }
}

public class PlacementData
{
    public List<Vector2Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector2Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}