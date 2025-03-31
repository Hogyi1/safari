using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    BuildingData Data;
    Grid grid;
    PreviewSystem previewSystem;
    MapData MapData;



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

        previewSystem.StartShowingPlacementPreview(Data.BuildingPrefab, Data.SpaceTaken);

    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        int cellSize = Mathf.RoundToInt(grid.cellSize.x);

        Vector2Int GridPosition = new Vector2Int(gridPosition.x * cellSize, gridPosition.z * cellSize);

        if (!MapData.CanPlaceObjectAt(GridPosition, Data.SpaceTaken)) return;

        float height = Data.type == BuildingType.ROAD ? grid.transform.position.y : mousePosition.y;

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 buildingPos = new Vector3(worldPosition.x, height, worldPosition.z);

        int index = BuildingManager.Instance.AddBuilding(Data, buildingPos);

        MapData.AddObjectAt(GridPosition, Data.SpaceTaken, Data.BuildingID, index);
        previewSystem.UpdatePosition(buildingPos, false);

        Debug.Log("Építmény ezen a pozicion lehelyezve: " + buildingPos + "\n Az építmény ezen a pozicion lesz elmentve " + GridPosition);
    }

    public void UpdateState(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        int cellSize = Mathf.RoundToInt(grid.cellSize.x);

        Vector2Int GridPosition = new Vector2Int(gridPosition.x * cellSize, gridPosition.z * cellSize);

        bool placementValidity = MapData.CanPlaceObjectAt(GridPosition, Data.SpaceTaken);

        float height = Data.type == BuildingType.ROAD ? grid.transform.position.y : mousePosition.y;

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 buildingPos = new Vector3(worldPosition.x, height, worldPosition.z);

        previewSystem.UpdatePosition(buildingPos, placementValidity);

    }
}
