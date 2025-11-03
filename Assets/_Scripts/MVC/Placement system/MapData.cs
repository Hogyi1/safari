using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Manages placement of objects on a 2D grid. 
/// Stores occupied cells and supports queries for placement, removal, and lookup.
/// </summary>
public class MapData
{
    /// <summary>
    /// Stores all occupied grid positions and the data about what object is placed there.
    /// </summary>
    Dictionary<Vector2Int, PlacementData> ObjectsPlacedOnGrid = new();

    /// <summary>
    /// Adds an object to the grid at the given position and size.
    /// Throws an exception if any of the target cells are already occupied.
    /// </summary>
    /// <param name="gridPosition">Top-left origin position of the object.</param>
    /// <param name="objectSize">Size in grid units (width x height).</param>
    /// <param name="ID">ID of the building in buildingdata</param>
    /// <param name="placedObjectIndex">Index to link back to its visual representation.</param>
    public void AddObjectAt(Vector2Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector2Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (!ObjectsPlacedOnGrid.ContainsKey(pos)) ObjectsPlacedOnGrid[pos] = data;
            // throw new Exception($"Dictionary already contains this cell position {pos}");

        }
    }

    /// <summary>
    /// Calculates a list of all grid positions that an object of given size would occupy from a starting position.
    /// </summary>
    /// <param name="gridPosition">Starting position (top-left).</param>
    /// <param name="objectSize">Object size in grid units.</param>
    /// <returns>List of occupied grid coordinates.</returns>
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

    /// <summary>
    /// Checks whether an object can be placed at the specified grid position and size.
    /// </summary>
    /// <param name="gridPosition">Top-left grid cell to attempt placement at.</param>
    /// <param name="objectSize">Size in grid units.</param>
    /// <returns>True if all required cells are unoccupied.</returns>
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

    /// <summary>
    /// Removes an object from the grid using any of its occupied positions.
    /// </summary>
    /// <param name="gridPosition">Any grid cell occupied by the object.</param>
    public void RemoveObjectAt(Vector2Int gridPosition)
    {
        foreach (var pos in ObjectsPlacedOnGrid[gridPosition].occupiedPositions)
        {
            ObjectsPlacedOnGrid.Remove(pos);
        }
    }

    /// <summary>
    /// Checks whether an object can be placed at the specified grid position and size.
    /// </summary>
    /// <param name="gridPosition">Top-left grid cell to attempt placement at.</param>
    /// <param name="objectSize">Size in grid units.</param>
    /// <returns>True if all required cells are unoccupied.</returns>
    public int GetRepresentationIndex(Vector2Int gridPosition)
    {
        if (ObjectsPlacedOnGrid.ContainsKey(gridPosition) == false)
            return -1;
        return ObjectsPlacedOnGrid[gridPosition].PlacedObjectIndex;
    }

    /// <summary>
    /// Returns the index of the placed object if a given cell is occupied.
    /// </summary>
    /// <param name="gridPosition">Grid position to check.</param>
    /// <returns>Object index if occupied, -1 otherwise.</returns>
    public int IsOccupied(Vector2Int gridPosition)
    {
        if (ObjectsPlacedOnGrid.ContainsKey(gridPosition))
            return ObjectsPlacedOnGrid[gridPosition].PlacedObjectIndex;
        return -1;
    }
}

/// <summary>
/// Contains information about a placed object, such as occupied grid cells,
/// unique identifier, and index used for representation lookup.
/// </summary>
public class PlacementData
{
    public List<Vector2Int> occupiedPositions; // Grid positions
    public int ID { get; private set; } // Unique ID
    public int PlacedObjectIndex { get; private set; } // View / Model ID

    /// <summary>
    /// Constructs a new PlacementData instance.
    /// </summary>
    /// <param name="occupiedPositions">All grid cells the object occupies.</param>
    /// <param name="iD">Unique object ID.</param>
    /// <param name="placedObjectIndex">Index to identify its representation.</param>
    public PlacementData(List<Vector2Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
