using System.Collections.Generic;
using System;
using UnityEngine;

public class VegetationManager : MonoBehaviour, IStructureManager, IRandomEventObserver
{
    public static VegetationManager Instance;
    private List<Vegetation> ActiveVegetations = new List<Vegetation>();
    private Dictionary<int, VegetationView> ActiveViews = new Dictionary<int, VegetationView>();

    public int Count;
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
    void Start()
    {
        RandomEvents.Instance.AddObserver(this);
    }

    void Update()
    {
        Count = ActiveVegetations.Count;
    }

    // Létrehozza a megadott Model réteget és eltárolja
    // Visszaadja a Model-t, hogy a fő manager tudjon vele foglalkozni
    public Structure AddStructure(BuildingData Data, int ID, Vector2Int gridPosition)
    {
        Vegetation Vegetation = new Vegetation(Data, ID);
        if (Vegetation == null) throw new Exception("Nem sikerült léterhozni a következőt: Vegetation");

        ActiveVegetations.Add(Vegetation);

        return Vegetation;
    }

    // Törli a saját referenciáját
    public void RemoveStructure(int ID)
    {
        Vegetation Vegetation = ActiveVegetations.Find(t => t.GetID() == ID);
        if (Vegetation == null) return;

        ActiveVegetations.Remove(Vegetation);
        ActiveViews.Remove(ID);
    }

    // Random mennyiségben megnöveli a növényeket
    private void HandleRegrowEvent()
    {
        foreach (Vegetation Vegetation in ActiveVegetations)
        {
            int RandomAmount = UnityEngine.Random.Range(0, Vegetation.GetMaxCapacity() / 3);

            Vegetation.Regrow(RandomAmount);
        }
    }

    public void OnNotify(RandomEvent randomEvent)
    {
        if (randomEvent == RandomEvent.Regrow) HandleRegrowEvent();
    }

    // Beállítja a megfelelő modellhez a nézetet
    public void SetView(IPlaceable view, int ID)
    {
        ActiveViews[ID] = (VegetationView)view;
    }
}
