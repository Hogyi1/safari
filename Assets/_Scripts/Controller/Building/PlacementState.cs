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
        Vector3Int flatGridPosition = new Vector3Int(gridPosition.x, 0, gridPosition.z);

        if (!MapData.CanPlaceObjectAt(flatGridPosition, Data.SpaceTaken)) return;

        Vector3 buildingPos = new Vector3(gridPosition.x, mousePosition.y, gridPosition.z);

        int index = BuildingManager.Instance.AddBuilding(Data, buildingPos);

        Debug.Log("Építmény ezen a pozicion lehelyezve " + buildingPos + "\n Az építmény ezen a pozicion lesz elmentve " + flatGridPosition);
        MapData.AddObjectAt(flatGridPosition, Data.SpaceTaken, Data.BuildingID, index);
        previewSystem.UpdatePosition(mousePosition, false);
    }

    public void UpdateState(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3Int flatGridPosition = new Vector3Int(gridPosition.x, 0, gridPosition.z);

        bool placementValidity = MapData.CanPlaceObjectAt(flatGridPosition, Data.SpaceTaken);

        Vector3 buildingPos = new Vector3(gridPosition.x, mousePosition.y, gridPosition.z);

        previewSystem.UpdatePosition(buildingPos, placementValidity);
    }
}
