using UnityEngine;

public class RoadView : MonoBehaviour, IPlaceable
{
    private int ID;
    public BuildingType GetBuildingType()
    {
        return BuildingType.Road;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public int GetID()
    {
        return ID;
    }

    public void SetID(int ID)
    {
        this.ID = ID;
    }

    public void Init(Structure structure)
    {
        return;
    }
}
