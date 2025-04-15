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

        Vector2Int MapPosition = new Vector2Int(gridPosition.x, gridPosition.z);

        if (!MapData.CanPlaceObjectAt(MapPosition, Data.SpaceTaken)) return;

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 buildingPos = new Vector3(worldPosition.x, mousePosition.y, worldPosition.z);

        int index = StructureManager.Instance.CreateStructure(Data, buildingPos);

        MapData.AddObjectAt(MapPosition, Data.SpaceTaken, Data.BuildingID, index);
        previewSystem.UpdatePosition(buildingPos, false);

        Debug.Log("Epitmeny ezen a pozicion lehelyezve: " + buildingPos + "\n Az Epitmeny ezen a pozicion lesz elmentve " + MapPosition);
    }

    public void UpdateState(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        Vector2Int MapPosition = new Vector2Int(gridPosition.x, gridPosition.z);

        bool placementValidity = MapData.CanPlaceObjectAt(MapPosition, Data.SpaceTaken);

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 buildingPos = new Vector3(worldPosition.x, mousePosition.y, worldPosition.z);

        previewSystem.UpdatePosition(buildingPos, placementValidity);
    }
}
