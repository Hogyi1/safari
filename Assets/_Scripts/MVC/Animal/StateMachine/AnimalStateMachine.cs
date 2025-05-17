using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AnimalStateMachine : MonoBehaviour
{

    public Animal animal;
    public AnimalBaseState RootState;

    public AnimalState currentState;
    public AnimalType Type;

    public string rootState;
    public string subState;
    public string subsubState;

    public float Hunger;
    public float Thirst;
    public float Age;
    public float Hp;
    public bool IsBreeding;
    public int GroupID;

    public bool Arrived;
    public bool AllFinished;

    public bool InGroup;
    public GroupState gState;
    public bool GroupHasPrey;
    public bool HasPrey;
    public bool HasMate;
    public List<Vector3> foodSources;
    public List<Vector3> waterSources;

    public Vector3 Target;
    public Vector3 Destination;
    public Vector3 GroupTarget;
    public List<Vector3> gfoodSources;
    public List<Vector3> gwaterSources;
    public void Init(Animal context)
    {
        this.animal = context;

        RootState = new IdleState(this, new AnimalStateFactory(this));
        RootState.EnterState();

        StartCoroutine(AgeRoutine());
        StartCoroutine(BreedingCooldownRoutine());
    }

    public void ReenterState()
    {
        RootState.EnterState();
    }

    public void LateUpdate()
    {
        Age = animal.Model.Age;
        Hunger = animal.Model.Hunger;
        Thirst = animal.Model.Thirst;
        Hp = animal.Model.Hp;
        IsBreeding = animal.Model.IsBreeding;

        Arrived = animal.View.Arrived;
        AllFinished = animal.View.FinishedAnimation;

        InGroup = !animal.Group.IsUnityNull();
        HasPrey = !animal.Model.Prey.IsUnityNull();

        foodSources = animal.Model.foodSources;
        waterSources = animal.Model.waterSources;

        Target = animal.Model.Target;
        Type = animal.Model.Type;
        Destination = animal.View.Agent.destination;
        HasMate = !animal.Model.Mate.IsUnityNull();
        if (animal.Group != null)
        {
            gState = animal.Group.State;
            GroupTarget = animal.Group.GetCurrentTarget();

            gfoodSources = animal.Group.foodSources.ToList();
            gwaterSources = animal.Group.waterSources.ToList();

            GroupHasPrey = !animal.Group.Prey.IsUnityNull();

            GroupID = animal.Group.ID;
        }

        try
        {
            AnimalBaseState suBState = RootState.GetSubState();
            rootState = RootState.ToString();
            if (suBState != null) subState = suBState.ToString();
            else subState = "null";

            AnimalBaseState suBsuBState = suBState.GetSubState();
            if (suBsuBState != null) subsubState = suBsuBState.ToString();
            else subsubState = "null";
        }
        catch (Exception) { }

        RootState.UpdateStates();
    }
    public void MarkAsDead()
    {
        animal.View.StopMovementInstantly();

        gameObject.GetComponent<AgentSync>().enabled = false;

        MonoBehaviour[] allBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var behaviour in allBehaviours)
        {
            if (behaviour != this)
                behaviour.enabled = false;
        }

        if (animal.Group != null) animal.Group.LeaveGroup(animal);
        StartCoroutine(DeathDelay());
    }

    private IEnumerator AgeRoutine()
    {
        while (!animal.Model.IsDead)
        {
            yield return new WaitForSeconds(TimeManager.Instance.SecondsPerAnimalYear);
            animal.Model.AdvanceAge();
            animal.View.AdvanceAge();
        }
    }

    private IEnumerator BreedingCooldownRoutine()
    {
        while (!animal.Model.IsDead)
        {
            yield return null;
            animal.Model.DecreaseBreedingCooldown(Time.deltaTime);
        }
    }

    private IEnumerator DeathDelay()
    {
        if (animal.Model.IsDead)
        {
            yield return new WaitForSeconds(10f);
            animal.CanRemove = true;
        }
    }

    public Animal GetAnimal(int iD)
    {
        return AnimalManager.Instance.GetAnimal(iD);
    }

    public void JoinGroup(Group group)
    {
        if (GroupManager.Instance.EnterGroup(group, animal))
        {
            ReenterState();
        }
    }

    public Structure GetStructure(Vector3 target)
    {
        return StructureManager.Instance.GetStructureByPosition(target);
    }

    public void KillAnimal(Animal animal)
    {
        AnimalManager.Instance.KillAnimal(animal);
    }

    public void Breeding(Animal mate)
    {
        AnimalManager.Instance.Breed(animal.Model, mate.Model);
    }
}

public enum AnimalState
{
    Idle,
    SeekFood,
    SeekWater,
    SeekMate,
    GroupBehaviour,
    Dead,
    Wandering,
    Stationary,
    Sleeping,
    AtTarget,
    Searching,
    OnTarget,
    Eating,
    Drinking,
    Breeding
}
