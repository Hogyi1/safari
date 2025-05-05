using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    /// <summary>
    /// Építéshez szükséseges adatok
    /// </summary>
    [Tooltip("EZ EGY EGYEDI AZONOSÍTÓ! Minden épület rendelkezik eggyel. NEM UGYANAZ MINT A VIEW ID")]
    public int BuildingID;

    [Tooltip("Szemből nézve, szélesség, mélység/hosszúság")]
    public Vector2Int SpaceTaken;


    /// <summary>
    /// Building specifikus adatok
    /// </summary>
    [Tooltip("Az újratöltés ára")]
    public int Price;

    [Tooltip("Éhség visszaállítására, vagy a ház/parkoló kapacitása")]
    public int Capacity;

    public BuildingType type;

    public Sprite icon;

    public DietType diet;


    /// <summary>
    /// Megjelenítendő objektum
    /// </summary>
    [SerializeField]
    public GameObject BuildingPrefab;
}
