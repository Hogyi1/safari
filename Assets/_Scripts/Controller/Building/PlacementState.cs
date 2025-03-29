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

        float height = mousePosition.y;
        if (Data.type == BuildingType.ROAD)
        {
            height = grid.transform.position.y;
        }

        Vector3 buildingPos = new Vector3(gridPosition.x, height, gridPosition.z);

        int index = BuildingManager.Instance.AddBuilding(Data, buildingPos);

        Debug.Log("Építmény ezen a pozicion lehelyezve! " + buildingPos + "\n Az építmény ezen a pozicion lesz elmentve " + flatGridPosition);
        MapData.AddObjectAt(flatGridPosition, Data.SpaceTaken, Data.BuildingID, index);
        previewSystem.UpdatePosition(buildingPos, false);
    }

    public void UpdateState(Vector3 mousePosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3Int flatGridPosition = new Vector3Int(gridPosition.x, 0, gridPosition.z);

        bool placementValidity = MapData.CanPlaceObjectAt(flatGridPosition, Data.SpaceTaken);

        float height = mousePosition.y;
        if (Data.type == BuildingType.ROAD)
        {
            height = grid.transform.position.y;
        }


        Vector3 buildingPos = new Vector3(gridPosition.x, height, gridPosition.z);

        previewSystem.UpdatePosition(buildingPos, placementValidity);
    }
}
