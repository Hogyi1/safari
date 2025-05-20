using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using static RangerState;

/// <summary>
/// Manages ranger units: spawning, task assignment, energy management, and economic interactions.
/// Implements IUpgradeable, IBuyableManager, and observes time-based events for salary handling.
/// </summary>
[RequireComponent(typeof(RangerFactory))]
public class RangerManager : MonoBehaviour, IUpgradeable, IBuyableManager, ITimeEventObserver, IDataPersistence
{
    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static RangerManager Instance { get; private set; }

    [SerializeField] private RangerFactory factory;
    [SerializeField] private ManagerType myType = ManagerType.Ranger;

    private int maxCapacity;
    private List<Ranger> activeRangers = new();
    private HashSet<int> animalIDs = new();

    /// <summary>Maximum number of rangers that can be active.</summary>
    public int MaxCapacity => maxCapacity;

    /// <summary>Current number of active rangers.</summary>
    public int Capacity => activeRangers.Count;
    public void SetCapacity(int amount) => maxCapacity = amount;

    public float Priority => 750f;


    /// <summary>
    /// Initializes the singleton instance.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// Subscribes to time-based events.
    /// </summary>
    private void Start()
    {
        TimeEvents.Instance.AddObserver(this);
    }


    /// <summary>
    /// Updates all rangers every frame based on their current state.
    /// Handles energy regeneration/consumption and state transitions.
    /// </summary>
    private void Update()
    {
        foreach (var ranger in activeRangers)
        {
            switch (ranger.Model.State)
            {
                case Busy:
                case On_target:
                    ranger.Model.DecreaseEnergy(Time.deltaTime);
                    break;

                case Resting:
                    ranger.Model.IncreaseEnergy(Time.deltaTime);
                    ranger.Model.CheckState();
                    break;

                case Available:
                    ranger.Model.CheckState();
                    break;

                case At_target:
                    HandleAtTarget(ranger);
                    break;

                case Finished:
                    HandleFinished(ranger);
                    break;
            }
        }
    }


    /// <summary>
    /// Spawns a new ranger if there is room in capacity.
    /// </summary>
    /// <param name="rangerID">Prefab ID or type of ranger to spawn.</param>
    public void SpawnRanger(int rangerID)
    {
        if (activeRangers.Count >= maxCapacity)
            return;

        int id = IDGenerator.GenerateID();
        Ranger newRanger = factory.CreateRanger(id, rangerID);

        if (newRanger.IsUnityNull())
            return;

        activeRangers.Add(newRanger);
        EconomyManager.Instance.PaySalary();
    }


    /// <summary>
    /// Removes a ranger by ID and destroys its GameObject.
    /// </summary>
    public void RemoveRanger(int rangerID)
    {
        Ranger toRemove = activeRangers.Find(r => r.ID == rangerID);
        if (toRemove == null) return;

        activeRangers.Remove(toRemove);
        Destroy(toRemove.View.gameObject);
    }


    /// <summary>
    /// Attempts to assign an available ranger to hunt a specific animal.
    /// </summary>
    /// <param name="animalID">Target animal ID.</param>
    /// <returns>True if a ranger was assigned, false if none were available.</returns>
    public bool HuntDownAnimal(int animalID)
    {
        Ranger available = activeRangers.Find(r => r.Model.State == Available);
        if (available == null) return false;

        AnimalView preyView = AnimalManager.Instance.GetAnimal(animalID)?.View;
        if (preyView == null) return false;

        available.Model.SetPrey(animalID);
        available.Model.State = On_target;

        animalIDs.Add(animalID);
        available.View.gameObject.SetActive(true);
        available.View.StartWalkingTowardsAnimal(preyView);

        return true;
    }


    /// <summary>
    /// Updates a ranger's state by their unique ID.
    /// </summary>
    public void SetRangerState(int id, RangerState newState)
    {
        Ranger ranger = activeRangers.Find(r => r.ID == id);
        if (ranger != null)
            ranger.Model.State = newState;
    }


    /// <summary>
    /// Handles behavior when a ranger reaches their prey target.
    /// </summary>
    private void HandleAtTarget(Ranger ranger)
    {
        int preyID = ranger.Model.ClearPrey();
        Animal prey = AnimalManager.Instance.GetAnimal(preyID);
        if (prey != null)
        {
            EconomyManager.Instance.AddMoney(prey.Model.Price);
            AnimalManager.Instance.KillAnimal(prey);
            ranger.View.AtTarget();
            animalIDs.Remove(preyID);
        }

        ranger.Model.State = Finished;
    }


    /// <summary>
    /// Handles behavior after the ranger finishes hunting.
    /// Sends them back to the house and marks them resting.
    /// </summary>
    private void HandleFinished(Ranger ranger)
    {
        ranger.View.ReturnToStation(GetHouseDoorPosition());
    }


    /// <summary>
    /// Called when a month passes. Removes rangers if their salary couldn't be paid.
    /// </summary>
    private void HandleMonthlySalary()
    {
        var toRemove = activeRangers.Where(r => !EconomyManager.Instance.PaySalary()).ToList();
        toRemove.ForEach(r => RemoveRanger(r.ID));
    }


    /// <summary>
    /// Handles time-based notifications (e.g. monthly salary events).
    /// </summary>
    public void OnNotify(TimeEvent timeEvent)
    {
        if (timeEvent == TimeEvent.Month_passed)
            HandleMonthlySalary();
    }


    /// <summary>
    /// Returns whether a specific animal can currently be hunted.
    /// </summary>
    public bool CanHunt(int animalID)
    {
        bool available = activeRangers.Any(r => r.Model.State == Available);
        bool alreadyTargeted = animalIDs.Contains(animalID);
        return available && !alreadyTargeted;
    }



    /*REFACTOR*/
    public IEnumerator LoadData(GameData data)
    {
        yield return SpawnRangerWithDelay(data.rangerManagerSaveData.RangerCount);
    }

    public void SaveData(GameData data)
    {
        data.rangerManagerSaveData.RangerCount = activeRangers.Count;
    }
    private IEnumerator SpawnRangerWithDelay(int count)
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < count; i++)
        {
            SpawnRanger(0);
        }
    }


    /// <summary>
    /// Indicates whether a new ranger can be purchased (i.e., capacity is not full).
    /// </summary>
    public bool CanBuy() => Capacity < MaxCapacity;

    /// <summary>
    /// Increases maximum ranger capacity.
    /// </summary>
    public void LevelUp(int amount)
    {
        maxCapacity += amount;
    }

    /// <summary>
    /// Decreases maximum ranger capacity.
    /// </summary>
    public void LevelDown(int amount)
    {
        maxCapacity -= amount;
    }

    /// <summary>
    /// Gets the position of the ranger house's door in world space.
    /// </summary>
    public Vector3 GetHouseDoorPosition() => FacilityManager.Instance.GetInteractingPosition(myType);

    /// <summary>
    /// Gets the position where new rangers should spawn in world space.
    /// </summary>
    public Vector3 GetSpawnPosition() => FacilityManager.Instance.GetSpawnPosition(myType);

}

/// <summary>
/// Possible states a ranger can be in during the simulation.
/// </summary>
public enum RangerState
{
    Busy,       // Actively hunting
    At_target,  // Reached target, about to perform action
    Resting,    // Recovering energy
    Available,  // Ready for assignment
    Finished,   // Done with task, returning to base
    On_target   // Walking toward the animal
}
