using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Represents the data model of an animal, including needs, state, and memory of environmental resources.
/// </summary>
public class AnimalModel
{
    // === Basic Info ===

    /// <summary> Unique identifier for the animal. </summary>
    public int ID;

    /// <summary> Dietary type (herbivore/carnivore/etc.). </summary>
    public DietType Diet;

    /// <summary> Type or species of the animal. </summary>
    public AnimalType Type;

    /// <summary> UI icon representing the animal. </summary>
    public Sprite Icon;

    // === Age, Price, and Breeding ===

    public int Age = 0;
    private int maxAge;
    private int price;

    /// <summary> True if the animal is old enough to breed. </summary>
    public bool CanBreed => Age > 3;

    /// <summary> Indicates if the animal is currently breeding. </summary>
    public bool IsBreeding = false;

    /// <summary> True if the animal is currently consuming food or water. </summary>
    public bool IsConsuming = false;

    private float lastBreedingTime = 0f;
    private float activeBreedingTime;
    private readonly float BreedingCooldown = 30f;

    // === Physiological Values ===

    private float hunger = 42f;
    private float thirst = 60f;
    private float hp = 100f;

    /// <summary> Current hunger level (0–100). </summary>
    public float Hunger => hunger;

    /// <summary> Current thirst level (0–100). </summary>
    public float Thirst => thirst;

    /// <summary> Current health points (0–100). </summary>
    public float Hp => hp;

    public bool IsHungry => hunger <= 40f;
    public bool IsThirsty => thirst <= 40f;
    public bool IsDead => hp <= 1;
    public bool IsOverstimulated => hunger <= 35f || thirst <= 35f;

    // === Behavioral Targets ===

    /// <summary> Current movement/interaction target. </summary>
    public Vector3 Target;

    /// <summary> ID of the prey currently targeted. </summary>
    public int PreyID = -1;

    /// <summary> ID of the current mating partner. </summary>
    public int MateID = -1;

    // === Environmental Memory ===

    public List<Vector3> waterSources = new();
    public List<Vector3> foodSources = new();

    // === Constructor ===

    /// <summary>
    /// Initializes the animal model with species data, ID and starting age.
    /// </summary>
    public AnimalModel(AnimalData data, int ID, int Age)
    {
        Diet = data.Diet;
        this.ID = ID;
        Type = data.Type;
        maxAge = data.MaxAge;
        this.Age = Age;
        price = data.Price;
        Icon = data.Icon;
    }

    // === Simple Setters ===

    public void SetPrey(int preyID) => PreyID = preyID;
    public void ClearPrey() => PreyID = -1;
    public void SetMate(int mateID) => MateID = mateID;
    public void ClearMate() => MateID = -1;
    public void SetTarget(Vector3 target) => Target = target;

    // === Needs & Health Calculations ===

    /// <summary> Reduces hunger and thirst over time, and updates HP accordingly. </summary>
    public void CalculateNeeds(float multiplier)
    {
        hunger = Mathf.Max(0, hunger - multiplier * Time.deltaTime);
        thirst = Mathf.Max(0, thirst - multiplier * Time.deltaTime);
        CalculateHp();
    }

    /// <summary> Adjusts HP based on hunger, thirst, and age. </summary>
    public void CalculateHp()
    {
        float change = (IsHungry || IsThirsty) && !IsConsuming
            ? -Time.deltaTime
            : Time.deltaTime * GetAgeFactor();

        hp = Mathf.Clamp(hp + change, 0, 100);
    }

    /// <summary> Returns an age-based health factor (0.1–1.1). </summary>
    public float GetAgeFactor()
    {
        float normalizedAge = (float)Age / maxAge;
        return 1.1f - Mathf.Pow((normalizedAge - 0.5f) * 2f, 2f);
    }

    /// <summary> Increases age by 1. Kills the animal if age exceeds maxAge. </summary>
    public void AdvanceAge()
    {
        if (Age <= maxAge) Age++;
        else GetKilled();
    }

    // === Breeding Logic ===

    /// <summary> Starts the breeding process and resets mating data. </summary>
    public void StartBreeding()
    {
        ClearMate();
        IsBreeding = true;
        activeBreedingTime = 0f;
    }

    /// <summary> Stops breeding and starts cooldown. </summary>
    public void StopBreeding()
    {
        ClearMate();
        IsBreeding = false;
        lastBreedingTime = BreedingCooldown;
    }

    /// <summary> Updates breeding cooldown or ends active breeding if time exceeded. </summary>
    public void DecreaseBreedingCooldown(float deltaTime)
    {
        if (IsBreeding)
        {
            activeBreedingTime += deltaTime;
            if (activeBreedingTime > BreedingCooldown)
            {
                IsBreeding = false;
                ClearMate();
            }
        }
        else if (lastBreedingTime > 0)
        {
            lastBreedingTime -= deltaTime;
        }
    }

    /// <summary> Checks if this animal and a given mate are both able to breed and mutually accept. </summary>
    public bool TryBreeding(int mateID)
    {
        var mate = AnimalManager.Instance.GetAnimalByID(mateID);
        if (mate == null || !mate.Model.CanBreed || !CanBreed) return false;

        bool accepted = mate.Model.hp > 40 && Random.value <= 0.65f;
        return accepted;
    }

    // === Eating & Drinking ===

    /// <summary> Restores hunger by a fixed amount. </summary>
    public void Eat(int amount)
    {
        hunger = Mathf.Min(100f, hunger + amount * 10f);
    }

    /// <summary> Restores thirst by a fixed amount. </summary>
    public void Drink(int amount)
    {
        thirst = Mathf.Min(100f, thirst + amount * 10f);
    }

    /// <summary> Kills the animal instantly. </summary>
    public void GetKilled()
    {
        hunger = 0;
        thirst = 0;
        hp = 0;
    }

    // === Source Memory Management ===

    /// <summary> Tries to add a new water source to memory. Keeps the list at max 10 entries. </summary>
    public bool SaveWaterSource(Vector3 position)
    {
        if (waterSources.Contains(position)) return false;
        waterSources.Insert(0, position);
        if (waterSources.Count > 10) waterSources.RemoveAt(waterSources.Count - 1);
        return true;
    }

    /// <summary> Tries to add a new food source to memory. Keeps the list at max 10 entries. </summary>
    public bool SaveFoodSource(Vector3 position)
    {
        if (foodSources.Contains(position)) return false;
        foodSources.Insert(0, position);
        if (foodSources.Count > 10) foodSources.RemoveAt(foodSources.Count - 1);
        return true;
    }

    /// <summary> Removes the given position from both food and water memory. </summary>
    public void RemoveSource(Vector3 position)
    {
        waterSources.Remove(position);
        foodSources.Remove(position);
    }

    /// <summary> Returns and cycles the next known water source (FIFO-like). </summary>
    public Vector3 GetNextWaterSourcePosition()
    {
        if (waterSources.Count == 0) return Vector3.zero;
        var source = waterSources[0];
        waterSources.RemoveAt(0);
        waterSources.Add(source);
        return source;
    }

    /// <summary> Returns and cycles the next known food source (FIFO-like). </summary>
    public Vector3 GetNextFoodSourcePosition()
    {
        if (foodSources.Count == 0) return Vector3.zero;
        var source = foodSources[0];
        foodSources.RemoveAt(0);
        foodSources.Add(source);
        return source;
    }

    // === Economic Logic ===

    /// <summary> Calculates the current market price of the animal based on age. </summary>
    public float GetPrice()
    {
        var ageFactor = GetAgeFactor();
        return Mathf.Min(price / 3f, price * ageFactor * 0.85f);
    }
}
