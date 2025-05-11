using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimalFactory))]
public class AnimalManager : MonoBehaviour, IRandomEventObserver
{
    // Singleton
    public static AnimalManager Instance { get; private set; }

    // Every Animal
    private List<Animal> ActiveAnimals = new List<Animal>();

    // Factory
    private AnimalFactory factory;

    // Csak a lerakáshoz kell
    private AnimalType animalType;

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
        RandomEvents.Instance.AddObserver(this);
    }

    private void Update()
    {
        // Tudsz ennél biztonságosabb kódot? XDD
        try
        {
            List<Animal> deadAnimals = ActiveAnimals.FindAll(t => t.CanRemove);

            foreach (var dead in deadAnimals)
            {
                RemoveAnimal(dead.ID);
            }
        }
        catch { }

    }

    public void StartPlacingAnimal(int animalType)
    {
        this.animalType = (AnimalType)animalType;
        InputEventChannel.OnClick += HandleClick;
    }

    private void HandleClick()
    {
        Vector3 pos = InputManager.Instance.GetSelectedMapPosition();

        if (animalType != AnimalType.None)
            SpawnAnimal(animalType, pos, 5);

        InputEventChannel.OnClick -= HandleClick;
    }

    // Létrehozzuk illetve eltávolítjuk
    public void SpawnAnimal(AnimalType type, Vector3 SpawningLocation, int Age)
    {
        int ID = IDGenerator.GenerateID();
        Animal newAnimal = factory.CreateAnimal(type, SpawningLocation, ID, Age);

        if (newAnimal != null)
            ActiveAnimals.Add(newAnimal);
    }

    public void RemoveAnimal(int ID)
    {
        Animal toRemove = ActiveAnimals.Find(t => t.ID == ID);
        if (toRemove != null)
        {
            ActiveAnimals.Remove(toRemove);
            Destroy(toRemove.View.gameObject);
        }
    }

    // Kiválaszt egy random állatot aki képes párzani
    private void SelectAnimalForBreeding()
    {
        List<AnimalModel> breedables = new List<AnimalModel>();
        foreach (var animal in ActiveAnimals)
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
        Animal animal = ActiveAnimals.Find(t => t.ID == mate1.ID);
        SpawnAnimal(animal.Model.Type, animal.View.transform.position, 1);
    }

    public Animal GetAnimal(int iD)
    {
        Animal animal = ActiveAnimals.Find(t => t.ID == iD);
        if (animal == null) return null;
        return animal;
    }

    public void KillAnimal(Animal prey)
    {
        if (ActiveAnimals.Contains(prey))
            prey.Model.GetKilled();
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

