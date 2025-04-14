using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StructureManager : MonoBehaviour
{
    public static StructureManager Instance;

    [SerializeField] private VegetationManager vegetationManager;
    [SerializeField] private FeederManager feederManager;
    [SerializeField] private WaterManager waterManager;

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
    // A **ROADMANAGER** és a **FACILITYMANAGEREK** külön managerek
    public IStructureManager GetManager(BuildingType type)
    {
        return type switch
        {
            BuildingType.Vegetation => vegetationManager,
            BuildingType.Water => waterManager,
            BuildingType.Feeder => feederManager,
            _ => null
        };
    }


    // Szól a megfelelő managernek, hogy készítsen el egy ISelectable-t
    // Szól a PlacementManagernek, hogy rakja le a megadott építményt a megfelelő pozícióra.
    // Visszatérési értéke a generált ID amit, később a MapData tárol el
    public int CreateStructure(BuildingData Data, Vector3 position)
    {
        IStructureManager manager = GetManager(Data.type);
        if (manager == null) throw new Exception("Nem található a következő manager: " + Data.type + "Manager");

        int ID = IDGenerator.GenerateID();

        Structure newStructure = manager.AddStructure(Data, ID);
        activeSelectables.Add(newStructure);

        IPlaceable view = PlacementManager.Instance.PlaceStructure(Data, position, newStructure);

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
        }
    }

    // Visszaadja a megfelelő view-t a másik managernek, így csak egy helyen kell tárolni
    public IPlaceable GetCorrespondingView(int ID)
    {
        return IInteractables[ID];
    }
}

// Interfész IStructureManager
// Minden épülettel foglalkozó Manager megvalósítja
public interface IStructureManager
{
    public Structure AddStructure(BuildingData Data, int ID);
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
}

//Interfész IStageable
//Minden, aminek változó kinézete vagy mechanizmusa van szinttől eltérően pl: Fa, Parkoló
//A View vagy a Model is megkaphatja, ha View megkapta akkor a Model is
public interface IStageable
{
    public float GetStage();
    public void SetStage(float stage);
}