using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AnimalFactory))]
[RequireComponent(typeof(AnimalPreviewSystem))]
public class AnimalManager : MonoBehaviour, IRandomEventObserver, IBuyableManager, IPlaceableManager, IDataPersistence
{
    public static AnimalManager Instance;

    private List<Animal> activeAnimals = new List<Animal>();

    // Factory, preview
    [SerializeField] private AnimalFactory factory;
    [SerializeField] private AnimalPreviewSystem previewSystem;

    public event Action OnPlaced;
    public event Action OnStopped;

    public bool Incoming;
    public int Count => activeAnimals.Count;
    public int HerbivoreCount => activeAnimals.Where(t => t.Model.Diet == DietType.Herbivore).Count();
    public int CarnivoreCount => activeAnimals.Where(t => t.Model.Diet == DietType.Carnivore).Count();
    public List<Animal> AllAnimals => activeAnimals;

    public float Priority => 1000f;
    private Action OnHandlerResponse;
    private IBuildingState BuildingState;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        StopPlacement();

        // Events
        RandomEvents.Instance.AddObserver(this);
        OnHandlerResponse = () => gameObject.SetActive(true);
        DataPersistenceManager.Instance.OnAllLoaded += OnHandlerResponse;

        gameObject.SetActive(false);
    }

    private void OnDestroy() => DataPersistenceManager.Instance.OnAllLoaded -= OnHandlerResponse;

    private void Update()
    {
        List<Animal> deadAnimals = activeAnimals.FindAll(t => t.CanRemove);

        foreach (var dead in deadAnimals)
        {
            RemoveAnimal(dead.ID);
        }
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
        if (placed)
        {
            OnPlaced?.Invoke();
            GameEvents.Instance.NotifyObservers(EventType.ANIMAL_PLACE, 1);
        }
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
    public Animal SpawnAnimal(AnimalType animalType, Vector3 SpawningLocation, int Age)
    {
        int ID = IDGenerator.GenerateID();
        Animal newAnimal = factory.CreateAnimal(animalType, SpawningLocation, ID, Age);

        if (newAnimal.IsUnityNull()) return null;

        activeAnimals.Add(newAnimal);
        Incoming = true;

        GameEvents.Instance.NotifyObservers(EventType.EXP_ADD, 10);
        GameEvents.Instance.NotifyObservers(EventType.ANIMAL_PLACE, 1);
        GameEvents.Instance.NotifyObservers(EventType.ANIMALS_OWNED, 1);
        return newAnimal;
    }

    public void RemoveAnimal(int ID)
    {
        Animal toRemove = activeAnimals.Find(t => t.ID == ID);
        if (toRemove != null)
        {
            Incoming = false;
            activeAnimals.Remove(toRemove);
            Destroy(toRemove.View.gameObject);

            GameEvents.Instance.NotifyObservers(EventType.ANIMALS_OWNED, -1);
            GameEvents.Instance.NotifyObservers(EventType.EXP_ADD, 20);
        }
    }

    // Kiválaszt egy random állatot aki képes párzani
    private void SelectAnimalForBreeding()
    {
        var breedables = activeAnimals.Where(animal => animal.Model.CanBreed && animal.InGroup && !animal.Model.IsBreeding).ToList();

        if (breedables.Count != 0)
            breedables[UnityEngine.Random.Range(0, breedables.Count)].Model.StartBreeding();
    }

    public void OnNotify(RandomEvent randomEvent)
    {
        if (randomEvent == RandomEvent.Breed_animal)
            SelectAnimalForBreeding();
    }

    public void Breed(int mate1ID, int mate2ID)
    {
        // Opció evoluciora
        Animal animal = GetAnimalByID(mate1ID);

        Animal babyAnimal = SpawnAnimal(animal.Model.Type, animal.View.transform.position, 1);

        GroupManager.Instance.EnterGroup(animal.GroupID, babyAnimal.ID, true);

        GameEvents.Instance.NotifyObservers(EventType.EXP_ADD, 50);
        GameEvents.Instance.NotifyObservers(EventType.ANIMAL_BORN, 1);
    }

    public Animal GetAnimalByID(int iD)
    {
        Animal animal = activeAnimals.Find(t => t.ID == iD);
        if (animal == null) return null;
        return animal;
    }

    public void KillAnimal(int preyID)
    {
        GetAnimalByID(preyID)?.Model.GetKilled();
    }

    public bool CanBuy() => true; // Nincs kapacitás jelenleg





    // Rework mentés
    // pls hogyi doit
    public void SaveData(GameData data)
    {

        data.animalSaveDatas.Clear();
        foreach (var a in activeAnimals)
        {
            data.animalSaveDatas.Add(new AnimalSaveData(a.Model.Type, a.View.gameObject.transform.position, a.Model.Age));
        }
    }
    public IEnumerator LoadData(GameData data)
    {
        yield return SpawnAnimalWithDelay(data);
    }

    private IEnumerator SpawnAnimalWithDelay(GameData data)
    {
        yield return new WaitForEndOfFrame();

        foreach (AnimalSaveData a in data.animalSaveDatas)
        {
            SpawnAnimal(a.type, a.position, a.Age);
        }
    }

    /// <summary>
    /// Suggests the formation of a new group between two animals,
    /// if neither is currently part of a group and they are compatible.
    /// Only the animal with the smaller ID will initiate the group to avoid duplicates.
    /// </summary>
    /// <param name="iD1">The ID of the first animal.</param>
    /// <param name="iD2">The ID of the second animal.</param>
    public void SuggestGroupFormation(int iD1, int iD2)
    {
        if (iD1 == iD2) return;

        Animal a1 = GetAnimalByID(iD1);
        Animal a2 = GetAnimalByID(iD2);

        if (a1 == null || a2 == null) return;
        if (a1.InGroup || a2.InGroup) return;

        // Only the animal with the smaller ID initiates the group creation
        if (a1.ID < a2.ID)
            GroupManager.Instance.CreateGroup(new List<int> { a1.ID, a2.ID }, GroupState.Idle);
    }

    /// <summary>
    /// Attempts to join the specified animal to an existing group, by their IDs.
    /// </summary>
    /// <param name="animalID">The ID of the animal trying to join.</param>
    /// <param name="groupID">The ID of the target group.</param>
    public void TryJoinGroup(int animalID, int groupID)
    {
        GroupManager.Instance.EnterGroup(groupID, animalID, false);
    }

}

// Most csak ilyen állatok vannak
[System.Serializable]
public enum AnimalType
{
    None,
    Tiger,
    Zebra,
    Giraffe,
    Hyena
}

