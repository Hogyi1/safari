using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PopupKeys;

[System.Serializable]
public class AnimalModel
{
    // Csak olvasható property, privát setterrel
    public int ID { get; private set; }
    public DietType Diet { get; private set; }
    public AnimalType Type { get; private set; }
    public float Speed { get; private set; } = 5f;
    public Sprite Icon;

    // Élettani jellemzők - csak olvashatók kívülről
    private float hunger = 100;
    private float thirst = 100;
    private float hp = 100;
    private int maxAge;

    public int Price;
    public int Age = 0;
    public bool IsHungry => hunger <= 40f;
    public bool IsThirsty => thirst <= 40f;
    public bool IsDead => hp <= 1;
    public float Hunger => hunger;
    public float Thirst => thirst;
    public float Hp => hp;
    public bool CanBreed => Age > 1;

    public bool IsConsuming = false;
    public bool IsBreeding = false;
    private float lastBreedingTime = 0f;
    private float activeBreedingTime;

    private readonly float breedingCooldown = 30f;

    // Források – csak belső logika használja
    public List<Vector3> waterSources = new List<Vector3>();
    public List<Vector3> foodSources = new List<Vector3>();

    // Pozíciók – belső értékek
    public AnimalModel Prey { get; private set; }
    public AnimalModel Mate { get; private set; }
    public Vector3 Target { get; private set; }

    public AnimalModel(AnimalData data, int ID, int Age)
    {
        this.Diet = data.Diet;
        this.ID = ID;
        this.Type = data.Type;
        this.Speed = data.Speed;
        this.maxAge = data.MaxAge;
        this.Age = Age;
        this.Price = data.Price / 2;
        this.Icon = data.Icon;
    }
    // Egyszerű számítások az igényekhez
    public void CalculateHunger(float multiplier)
    {
        hunger = Mathf.Max(0, hunger - (multiplier * Time.deltaTime));
        if (hunger <= 0)
        {
            hp = 1;
        }
    }

    public void CalculateThirst(float multiplier)
    {
        thirst = Mathf.Max(0, thirst - (multiplier * Time.deltaTime));
        if (thirst <= 0)
        {
            hp = 1;
        }
    }
    public void CalculateHp()
    {
        float normalizedAge = (float)Age / (float)maxAge;
        float ageFactor = 1f - Mathf.Pow((normalizedAge - 0.5f) * 2f, 2f); // 0.0..1.0

        float change = 0f;

        // Ha van valami baja akkor szépen csökken az életereje, ha nem és még van "ereje" akkor regenerálódik
        if (IsHungry || IsThirsty)
        {
            change = -10f * Time.deltaTime * 2;
        }
        else
        {
            change = 3f * Time.deltaTime * ageFactor;
        }

        hp = Mathf.Clamp(Mathf.RoundToInt(hp + change), 0, 100);
    }

    public void AdvanceAge()
    {
        if (Age <= maxAge)
        {
            Age++;
            return;
        }
        else
        {
            GetKilled();
        }
    }
    // Csökkentjük, hogy ne tudjon örökké párzani
    public void DecreaseBreedingCooldown(float deltaTime)
    {
        if (IsBreeding)
        {
            activeBreedingTime += deltaTime;
            if (activeBreedingTime >= breedingCooldown)
            {
                IsBreeding = false;
                ClearMate();
            }
        }
        if (lastBreedingTime > 0) lastBreedingTime -= deltaTime;
    }


    // Amennyiben sikertelen a keresés csak Remove. Így tudunk mindig végigpörgetni a listánkon.
    // IPlaceable, mert csak a View-t látja és csak az általunk lerakott "épületek" ehetőek.

    // Hozzáadjuk az új helyet ha megtaláltuk
    public bool SaveWaterSource(Vector3 position)
    {
        if (waterSources.Contains(position))
            return false;

        waterSources.Insert(0, position);

        if (waterSources.Count > 10)
            waterSources.RemoveAt(waterSources.Count - 1);

        return true;
    }

    public bool SaveFoodSource(Vector3 position)
    {
        if (foodSources.Contains(position))
            return false;

        foodSources.Insert(0, position);

        if (foodSources.Count > 10)
            foodSources.RemoveAt(foodSources.Count - 1);

        return true;
    }

    // Kivesszük azt a helyet ahol épp vagyunk
    public void RemoveSource(Vector3 position)
    {
        waterSources.Remove(position);

        foodSources.Remove(position);
    }

    // Ami az elején van az a végére kerül, ha nem találunk ott akkor a Removeval úgyis ki lesz lőve
    public Vector3 GetNextWaterSourcePosition()
    {
        if (waterSources.Count == 0) { return Vector3.zero; }
        Vector3 source = waterSources[0];
        waterSources.Remove(source);
        waterSources.Add(source);
        return source;
    }
    public Vector3 GetNextFoodSourcePosition()
    {
        if (foodSources.Count == 0) { return Vector3.zero; }
        Vector3 source = foodSources[0];
        foodSources.Remove(source);
        foodSources.Add(source);
        return source;
    }
    public void SetTarget(Vector3 vec3) => Target = vec3;

    // A paraméterben kapott állat a kezdeményező, neked van lehetőséged visszautasítani, ha pl kevés az élete stb.
    public bool TryBreeding(AnimalModel mate)
    {
        // Check to see if the dugopartner is dughato
        if (mate == null || !mate.CanBreed || !CanBreed || lastBreedingTime > 1) return false;

        // Random mood + ha már halálhoz közelít akkor is
        if (mate.hp > 40 && UnityEngine.Random.Range(0f, 1f) <= 0.5f)
        {
            // Smash
            Debug.Log("Smash");
            lastBreedingTime = breedingCooldown;
            IsBreeding = false;
            return true;
        }

        // Pass
        Debug.Log("Pass");
        IsBreeding = false;
        return false;
    }


    // Egyszerű beállítások, hogy metódust kelljen hívni
    public void StartBreeding()
    {
        IsBreeding = true;
        activeBreedingTime = 0f;
        ClearMate();
    }

    // Az állat egy egységnyi kaját 10 hungernek tekinti
    public void Eat(int amount)
    {
        hunger = Mathf.Min(100f, hunger + amount * 10f);
    }
    public void Drink(int amount)
    {
        thirst = Mathf.Min(100f, thirst + amount * 10f);
    }
    public void SetPrey(AnimalModel prey)
    {
        Prey = prey;
    }
    public void ClearPrey()
    {
        Prey = null;
    }
    public void GetKilled()
    {
        hunger = 0;
        thirst = 0;
        hp = 0;
    }
    public void SetMate(AnimalModel model)
    {
        Mate = model;
    }
    public void ClearMate()
    {
        Mate = null;
    }
    public void StopBreeding()
    {
        IsBreeding = false;
        lastBreedingTime = breedingCooldown;
    }
}
