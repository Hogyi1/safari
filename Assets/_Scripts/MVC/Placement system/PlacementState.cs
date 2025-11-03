using UnityEngine;

/// <summary>
/// Represents the state responsible for handling placement logic of a building.
/// Handles grid snapping, map occupancy validation, and preview visualization.
/// </summary>
public class PlacementState : IBuildingState
{
    BuildingData Data;
    Grid grid;
    PreviewSystem previewSystem;
    MapData MapData;

    private int cellSize = 6;
    private bool isRoad = false;

    /// <summary>
    /// Constructor initializes the placement state and starts the preview.
    /// </summary>
    /// <param name="Data">Data of the building being placed.</param>
    /// <param name="grid">Reference to the Unity Grid system.</param>
    /// <param name="previewSystem">System that shows the building preview.</param>
    /// <param name="MapData">Map data to track occupied positions.</param>
    public PlacementState(
                          BuildingData Data,
                          Grid grid,
                          PreviewSystem previewSystem,
                          MapData MapData)
    {
        this.Data = Data;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.MapData = MapData;
        this.cellSize = Mathf.RoundToInt(grid.cellSize.x);
        this.isRoad = Data.Type == BuildingType.Road;

        previewSystem.StartShowingPlacementPreview(Data.BuildingPrefab, Data.SpaceTaken);
    }

    /// <summary>
    /// Cleans up the state by stopping the preview display.
    /// </summary>
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    /// <summary>
    /// Called when the player clicks to place the object.
    /// Checks if placement is valid and registers the structure if so.
    /// </summary>
    /// <param name="mousePosition">World position under the mouse.</param>
    public bool OnAction(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition); // The position in cell coordinate
        Vector2Int mapPosition = new Vector2Int(gridPosition.x * cellSize, gridPosition.z * cellSize); // Normalized cell coordinate

        if (!MapData.CanPlaceObjectAt(mapPosition, Data.SpaceTaken)) return false; // If occupied return

        Vector3 worldPosition = grid.CellToWorld(gridPosition); // Snapped world position
        float buildHeight = isRoad ? grid.transform.position.y : mousePosition.y; // Buildheight based on the type of the object
        Vector3 buildingPos = new Vector3(worldPosition.x, buildHeight, worldPosition.z); // World position of the building

        int index = StructureManager.Instance.CreateStructure(Data, buildingPos, new Vector2Int(gridPosition.x, gridPosition.z)); // The created structures index

        MapData.AddObjectAt(mapPosition, Data.SpaceTaken, Data.BuildingID, index); // Occupy the position in the mapData
        previewSystem.UpdatePosition(buildingPos, false); // Let the player know the position is occupied visually
        return true;
    }

    /// <summary>
    /// Called when the player clicks to place the object.
    /// Checks if placement is valid and registers the structure if so.
    /// </summary>
    /// <param name="mousePosition">World position under the mouse.</param>
    public void UpdateState(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector2Int MapPosition = new Vector2Int(gridPosition.x * cellSize, gridPosition.z * cellSize);

        bool placementValidity = MapData.CanPlaceObjectAt(MapPosition, Data.SpaceTaken);

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        float buildHeight = isRoad ? grid.transform.position.y : mousePosition.y;
        Vector3 buildingPos = new Vector3(worldPosition.x, buildHeight, worldPosition.z);

        previewSystem.UpdatePosition(buildingPos, placementValidity);
    }
}
