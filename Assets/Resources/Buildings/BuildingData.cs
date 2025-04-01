using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    /// <summary>
    /// Építéshez szükséseges adatok
    /// </summary>
    [SerializeField]
    [Tooltip("EZ EGY EGYEDI AZONOSÍTÓ! Minden épület rendelkezik eggyel. NEM UGYANAZ MINT A VIEW ID")]
    public int BuildingID;

    [SerializeField]
    [Tooltip("Szemből nézve, szélesség, mélység/hosszúság")]
    public Vector2Int SpaceTaken;


    /// <summary>
    /// Building specifikus adatok
    /// </summary>
    [SerializeField]
    [Tooltip("Az újratöltés ára")]
    public int Price;

    [SerializeField]
    [Tooltip("Éhség visszaállítására, vagy a ház/parkoló kapacitása")]
    public int Capacity;

    [SerializeField]
    [Tooltip("Etető-e")]
    public bool isFeeder;

    [SerializeField]
    [Tooltip("Növény-e")]
    public bool isPlant;

    [SerializeField]
    public BuildingType type;


    /// <summary>
    /// Megjelenítendő objektum
    /// </summary>
    [SerializeField]
    public GameObject BuildingPrefab;
}
