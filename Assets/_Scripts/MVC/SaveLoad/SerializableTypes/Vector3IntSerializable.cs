using UnityEngine;

[System.Serializable]
public struct Vector3IntSerializable
{
    public int x, y, z;

    public Vector3IntSerializable(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3IntSerializable(Vector3Int v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(x, y, z);
    }
}
