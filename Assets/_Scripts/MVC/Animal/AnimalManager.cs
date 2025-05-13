using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimalFactory))]
public class AnimalManager : MonoBehaviour, IRandomEventObserver, IBuyableManager, IPlaceableManager , IDataPersistence
{
    // Singleton
    public static AnimalManager Instance { get; private set; }

    // Every Animal
    private List<Animal> activeAnimals = new List<Animal>();

    // Factory
    [SerializeField] private AnimalFactory factory;
    [SerializeField] private AnimalPreviewSystem previewSystem;
    private IBuildingState BuildingState;

    public event Action OnPlaced;
    public event Action OnStopped;

    public bool Incoming;
    public int Count => activeAnimals.Count;

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

    private void Start()
    {
        factory = GetComponent<AnimalFactory>();
        StopPlacement();
        RandomEvents.Instance.AddObserver(this);
    }

    private void Update()
    {
        // Tudsz ennél biztonságosabb kódot? XDD
        try
        {
            List<Animal> deadAnimals = activeAnimals.FindAll(t => t.CanRemove);

            foreach (var dead in deadAnimals)
            {
                RemoveAnimal(dead.ID);
            }
        }
        catch { }

    }

    public void StartPlacing(int animalID)
    {
        InputManager.Instance.SetState(InputState.AnimalPlacementMode);
        InputEventChannel.OnClick += TryPlacement;
        InputManager.Instance.StopPlacement += StopPlacement;

        StopPlacement();

        AnimalType type = factory.GetAnimalTypeByID(animalID);
        BuildingState = new AnimalPlacementState(type, previewSystem);
    }

    private void TryPlacement()
    {
        if (InputManager.Instance.IsPointerOverUI()) return;

        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();

        bool placed = BuildingState.OnAction(mousePosition);
        if (placed) OnPlaced?.Invoke();
    }

    public void StopPlacement()
    {
        if (BuildingState == null) return;
        BuildingState.EndState();
        BuildingState = null;

        OnStopped?.Invoke();

        InputManager.Instance.SetState(InputState.NormalMode);
        InputEventChannel.OnClick -= TryPlacement;
        InputManager.Instance.StopPlacement -= StopPlacement;
    }

    // Létrehozzuk illetve eltávolítjuk
    public void SpawnAnimal(AnimalType animalType, Vector3 SpawningLocation, int Age)
    {
        int ID = IDGenerator.GenerateID();
        Animal newAnimal = factory.CreateAnimal(animalType, SpawningLocation, ID, Age);

        if (newAnimal != null)
            activeAnimals.Add(newAnimal);
        Incoming = true;
    }

    public void RemoveAnimal(int ID)
    {
        Animal toRemove = activeAnimals.Find(t => t.ID == ID);
        if (toRemove != null)
        {
            Incoming = false;
            activeAnimals.Remove(toRemove);
            Destroy(toRemove.View.gameObject);
        }
    }

    // Kiválaszt egy random állatot aki képes párzani
    private void SelectAnimalForBreeding()
    {
        List<AnimalModel> breedables = new List<AnimalModel>();
        foreach (var animal in activeAnimals)
        {
            if (animal.Model.CanBreed && animal.Group != null && !animal.Model.IsBreeding) breedables.Add(animal.Model);
        }

        if (breedables.Count != 0)
        {
            int Random = UnityEngine.Random.Range(0, breedables.Count);
            breedables[Random].StartBreeding();
        }
    }

    public void OnNotify(RandomEvent randomEvent)
    {
        if (randomEvent == RandomEvent.Breed_animal)
        {
            SelectAnimalForBreeding();
        }
    }

    public void Breed(AnimalModel mate1, AnimalModel mate2)
    {
        // Opció evoluciora
        Animal animal = activeAnimals.Find(t => t.ID == mate1.ID);
        SpawnAnimal(animal.Model.Type, animal.View.transform.position, 1);
    }

    public Animal GetAnimal(int iD)
    {
        Animal animal = activeAnimals.Find(t => t.ID == iD);
        if (animal == null) return null;
        return animal;
    }

    public void KillAnimal(Animal prey)
    {
        if (activeAnimals.Contains(prey))
            prey.Model.GetKilled();
    }

    public bool CanBuy() => true;

    

    public void SaveData(GameData data)
    {

        data.animalSaveDatas.Clear();
        foreach (var a in activeAnimals)
        {
            data.animalSaveDatas.Add(new AnimalSaveData(a.Model.Type,a.View.gameObject.transform.position,a.Model.Age));
        }
    }

    public void LoadData(GameData data)
    {
        StartCoroutine(SpawnAnimalWithDelay(data));
    }

    private IEnumerator SpawnAnimalWithDelay(GameData data)
    {
        yield return new WaitForEndOfFrame();

        foreach (AnimalSaveData a in data.animalSaveDatas)
        {
            SpawnAnimal(a.type, a.position, a.Age);
        }

    }

}

// Most csak ilyen állatok vannak
[Serializable]
public enum AnimalType
{
    None,
    Tiger,
    Zebra,
    Giraffe,
    Hyena
}

