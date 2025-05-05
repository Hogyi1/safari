using UnityEngine;
using UnityEngine.UIElements;

public class Structure
{
    public int ID;
    public string Name;
    public BuildingType Type;
    [Tooltip("Width | Height")]
    public Vector2Int Size;
    public int Capacity;

    private BuildingData Data;

    public Structure(int ID, BuildingData Data)
    {
        this.ID = ID;
        this.Data = Data;
    }

    public int GetID()
    {
        return ID;
    }
}
