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

    public void OnAction(Vector3Int gridPosition)
    {
        if (!MapData.CanPlaceObjectAt(gridPosition, Data.SpaceTaken)) return;

        int index = BuildingManager.Instance.AddBuilding(Data, grid.CellToWorld(gridPosition));

        MapData.AddObjectAt(gridPosition, Data.SpaceTaken, Data.BuildingID, index);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = MapData.CanPlaceObjectAt(gridPosition, Data.SpaceTaken);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
