using UnityEngine;

[CreateAssetMenu(fileName = "VehicleData", menuName = "Scriptable Objects/VehicleData")]
public class VehicleData : ScriptableObject
{
    [Range(1, 10)]
    [SerializeField]
    public int capacity;

    [SerializeField]
    public int price;

    [SerializeField]
    public float speed = 5;

    [Range(1, 3)]
    [SerializeField]
    public int spacetaken;

    [SerializeField]
    public VehicleType type;

    [SerializeField]
    public GameObject vehiclePrefab;
}
