using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RangerState;

[RequireComponent(typeof(RangerFactory))]
public class RangerManager : MonoBehaviour, IUpgradeable, IBuyableManager, ITimeEventObserver, IDataPersistence
{
    public static RangerManager Instance { get; private set; }

    [SerializeField] private RangerFactory factory;

    [SerializeField] private ManagerType myType = ManagerType.Ranger;
    [SerializeField] private GameObject house;

    private int maxCapacity;
    private List<Ranger> activeRangers = new();
    private HashSet<int> animalIDs = new();

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
        TimeEvents.Instance.AddObserver(this);
    }

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

    private void HandleFinished(Ranger ranger)
    {
        ranger.View.ReturnToStation(GetHouseDoorPosition());
    }

    private void HandleAtTarget(Ranger ranger)
    {
        int preyID = ranger.Model.ClearPrey();
        Animal prey = AnimalManager.Instance.GetAnimal(preyID);
        if (prey == null) return;

        EconomyManager.Instance.AddMoney(prey.Model.Price);
        AnimalManager.Instance.KillAnimal(prey);
        ranger.View.AtTarget();
        animalIDs.Remove(preyID);
        ranger.Model.State = Finished;
    }

    public bool HuntDownAnimal(int animalID)
    {
        Ranger available = activeRangers.Find(r => r.Model.State == Available);
        if (available == null) return false;

        available.Model.SetPrey(animalID);
        available.Model.State = On_target;
        AnimalView preyView = AnimalManager.Instance.GetAnimal(animalID).View;
        if (preyView == null) return false;

        animalIDs.Add(animalID);
        available.View.gameObject.SetActive(true);
        available.View.StartWalkingTowardsAnimal(preyView);
        return true;
    }

    public void SpawnRanger(int rangerID)
    {
        if (activeRangers.Count >= maxCapacity) return;

        int ID = IDGenerator.GenerateID();
        Ranger newRanger = factory.CreateRanger(ID, rangerID);

        if (newRanger != null)
            activeRangers.Add(newRanger);
        EconomyManager.Instance.PaySalary();
    }

    public void RemoveRanger(int rangerID)
    {
        Ranger toRemove = activeRangers.Find(t => t.ID == rangerID);
        if (toRemove != null)
        {
            activeRangers.Remove(toRemove);
            Destroy(toRemove.View.gameObject);
        }
    }

    public void SetRangerState(int id, RangerState newState)
    {
        Ranger ranger = activeRangers.Find(r => r.ID == id);
        if (ranger != null)
            ranger.Model.State = newState;
    }

    public void LevelUp(int amount) => maxCapacity += amount;
    public void LevelDown(int amount) => maxCapacity -= amount;
    public bool CanBuy() => Capacity < MaxCapacity;
    public int MaxCapacity => maxCapacity;
    public int Capacity => activeRangers.Count;
    public Vector3 GetHouseDoorPosition() => house.transform.Find("Door").position;
    public Vector3 GetSpawnposition() => house.transform.Find("Spawn").position;
    private void HandleMonthlySalary()
    {
        var toRemove = activeRangers.Where(t => !EconomyManager.Instance.PaySalary()).ToList();
        toRemove.ForEach(t => RemoveRanger(t.ID));
    }

    public void OnNotify(TimeEvent timeEvent)
    {
        if (timeEvent == TimeEvent.Month_passed) HandleMonthlySalary();
    }

    public bool CanHunt(int animalID)
    {
        bool availableranger = activeRangers.Any(r => r.Model.State == Available);
        bool isHunted = animalIDs.Contains(animalID);

        return availableranger && !isHunted;
    }

    public void LoadData(GameData data)
    {
        maxCapacity = data.RangerMaxCapacity;
        StartCoroutine(SpawnRangerWithDelay(data.RangerCount));
    }

    public void SaveData(GameData data)
    {
        data.RangerMaxCapacity = maxCapacity;
        data.RangerCount= activeRangers.Count;
    }
    private IEnumerator SpawnRangerWithDelay(int count)
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < count; i++)
        {
            SpawnRanger(0);
        }
    }
}
public enum RangerState
{
    Busy, // ha épp vadászik
    At_target, // ha odaért
    Resting, // ha már nem busy és tul faradt
    Available, // ha már nem busy és nem tul faradt
    Finished,
    On_target
}
