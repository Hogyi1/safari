using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Scriptable Objects/AnimalData")]
public class AnimalData : ScriptableObject
{
    /// <summary>
    /// Állatnak adatok
    /// </summary>
    public int MaxAge;
    public DietType Diet;
    public AnimalType Type;
    public float Speed;
    public int animalID;

    /// <summary>
    /// Megjelenítendő objektum
    /// </summary>
    [SerializeField]
    public GameObject AnimalPrefab;
}
