using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StructureManager : MonoBehaviour, IBuyableManager
{
    public static StructureManager Instance;

    [SerializeField] private VegetationManager vegetationManager;
    [SerializeField] private FeederManager feederManager;
    [SerializeField] private WaterManager waterManager;
    [SerializeField] private RoadManager roadManager;
    [SerializeField] private FacilityManager facilityManager;

    [SerializeField] private List<Structure> activeSelectables = new List<Structure>();
    public Dictionary<int, IPlaceable> IInteractables = new Dictionary<int, IPlaceable>();

    //Singleton design
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() { }


    void Update() { }


    // Visszaadja a megfelelő managert az egyes típusokhoz
    // Ha nem talált megfelelő managert null-t ad vissza
    public IStructureManager GetManager(BuildingType type)
    {
        return type switch
        {
            BuildingType.Vegetation => vegetationManager,
            BuildingType.Water => waterManager,
            BuildingType.Feeder => feederManager,
            BuildingType.Road => roadManager,
            BuildingType.Facility => facilityManager,
            _ => null
        };
    }


    // Szól a megfelelő managernek, hogy készítsen el egy ISelectable-t
    // Szól a PlacementManagernek, hogy rakja le a megadott építményt a megfelelő pozícióra.
    // Visszatérési értéke a generált ID amit, később a MapData tárol el
    public int CreateStructure(BuildingData Data, Vector3 position, Vector2Int gridPosition)
    {
        IStructureManager manager = GetManager(Data.Type);
        if (manager.IsUnityNull()) throw new Exception("Nem található a következő manager: " + Data.Type + "Manager");
        GameEvents.Instance.NotifyObservers(EventType.EXP_ADD, 10);

        int ID = IDGenerator.GenerateID();

        Structure newStructure = manager.AddStructure(Data, ID, gridPosition);
        activeSelectables.Add(newStructure);

        IPlaceable view = PlacementManager.Instance.Place(Data, position, newStructure);

        manager.SetView(view, ID);

        IInteractables[ID] = view;

        return ID;
    }


    // Törli a saját listájából a megfelelő View-t és Model-t
    // Szól az őt kezelő Managernek is, hogy törölje.
    public void RemoveStructure(int ID)
    {
        Structure newStructure = activeSelectables.Find(t => t.GetID() == ID);
        if (newStructure != null)
        {
            activeSelectables.Remove(newStructure);

            IStructureManager manager = GetManager(newStructure.GetBuildingType());

            manager.RemoveStructure(ID);
        }

        if (IInteractables.TryGetValue(ID, out IPlaceable view))
        {
            PlacementManager.Instance.RemoveStructure(view);
            IInteractables.Remove(ID);
            Destroy(view.GetGameObject());
        }
    }

    public bool RegisterStructures(BuildingData Data, int ID, IPlaceable view, Vector2Int nodePosition)
    {
        IStructureManager manager = GetManager(Data.Type);
        if (manager.IsUnityNull()) return false;

        Structure newStructure = manager.AddStructure(Data, ID, nodePosition); // Nem jó az utakhoz
        activeSelectables.Add(newStructure);
        view.Init(newStructure);

        manager.SetView(view, ID);
        IInteractables[ID] = view;
        return true;
    }

    // Visszaadja a megfelelő view-t a másik managernek, így csak egy helyen kell tárolni
    public IPlaceable GetCorrespondingView(int ID)
    {
        return IInteractables[ID];
    }

    // Egy poziciohoz megkeressük a legközelebbi építményt
    public Structure GetStructureByPosition(Vector3 position)
    {
        const float tolerance = 2.5f;
        foreach (var structure in IInteractables)
        {
            var view = structure.Value;
            Debug.Log(position + " Ezen poziciot akarom lecsekkolni");
            if (Vector3.Distance(view.GetGameObject().transform.position, position) <= tolerance)
            {
                return view.GetStructure();
            }
        }
        return null;
    }

    public bool CanBuy() => true;

    /// <summary>
    /// Returns a list of all currently placed IPlaceable views.
    /// </summary>
    public List<IPlaceable> GetPlaceables()
    {
        // IInteractables holds every IPlaceable by its unique ID
        return new List<IPlaceable>(IInteractables.Values);
    }


}

// Interfész IStructureManager
// Minden épülettel foglalkozó Manager megvalósítja
public interface IStructureManager
{
    public Structure AddStructure(BuildingData Data, int ID, Vector2Int gridPosition);
    public void RemoveStructure(int ID);
    public void SetView(IPlaceable view, int ID);
}

// VIEW
// Interfész IInteractable
// Minden view amivel lehet interaktárolni, kattintás, egér fölé mozdítás stb...
public interface IInteractable
{
    public void OnHover();
    public void OnExit();
    public void OnAction();
    public void OnCancel();
}

// Interfész IPlaceable
// Minden view amit le lehet helyezni vagy mozgatni lehet, az építésnél
public interface IPlaceable
{
    public int GetID();
    public GameObject GetGameObject();
    public BuildingType GetBuildingType();
    public void Init(Structure structure);
    public Structure GetStructure();
    public BuildingData GetData();
}

// Interfész IHasInteractingPosition
// Minden view aminél van egy ajtó vagy bármilyen rész amivel az npc interaktálhat
public interface IHasInteractingPosition
{
    public Vector3 GetInteractingPosition();
}

//Interfész IStageable
//Minden, aminek változó kinézete vagy mechanizmusa van szinttől eltérően pl: Fa
//A View vagy a Model is megkaphatja, ha View megkapta akkor a Model is
public interface IStageable
{
    public float GetStage();
    public void SetStage(float stage);
}

//Interfész IUpgradeable
// Minden amit lehet fejleszteni megkapja, a View és Model egyaránt megkapja
public interface IUpgradeable
{
    public void LevelUp(int amount);
    public void LevelDown(int amount);
}