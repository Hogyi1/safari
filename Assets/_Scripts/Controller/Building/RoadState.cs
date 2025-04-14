using System.Collections.Generic;
using UnityEngine;

public class RoadState : IBuildingState
{
    BuildingData Data;
    Grid grid;
    PreviewSystem previewSystem;
    MapData MapData;

    private int CellSize = 6;

    public RoadState(
                          BuildingData Data,
                          Grid grid,
                          PreviewSystem previewSystem,
                          MapData MapData)
    {
        this.Data = Data;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.MapData = MapData;
        this.CellSize = Data.SpaceTaken.x;

        previewSystem.StartShowingPlacementPreview(Data.BuildingPrefab, Data.SpaceTaken);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        Vector2Int MapPosition = new Vector2Int(gridPosition.x * CellSize, gridPosition.z * CellSize);

        if (!MapData.CanPlaceObjectAt(MapPosition, Data.SpaceTaken)) return;

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 buildingPos = new Vector3(worldPosition.x, grid.transform.position.y, worldPosition.z);

        int index = RoadManager.Instance.PlaceRoad(Data, new Vector2Int(gridPosition.x, gridPosition.z), buildingPos);

        MapData.AddObjectAt(MapPosition, Data.SpaceTaken, Data.BuildingID, index);
        previewSystem.UpdatePosition(buildingPos, false);
    }

    public void UpdateState(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        Vector2Int MapPosition = new Vector2Int(gridPosition.x * CellSize, gridPosition.z * CellSize);

        bool placementValidity = MapData.CanPlaceObjectAt(MapPosition, Data.SpaceTaken);

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 buildingPos = new Vector3(worldPosition.x, grid.transform.position.y, worldPosition.z);

        previewSystem.UpdatePosition(buildingPos, placementValidity);
    }
}
