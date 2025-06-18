using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateMachine : MonoBehaviour
{

    public Animal animal;
    public AnimalBaseState RootState;

    [Header("Debug")]
    public AnimalType Type;
    public string rootState;
    public string subState;
    public string subsubState;

    [Header("Physicals")]
    public float Hunger;
    public float Thirst;
    public float Age;
    public float Hp;

    [Header("States")]
    public bool IsBreeding;
    public bool Arrived;
    public bool AllFinished;
    public bool IsConsuming;

    [Header("Sources")]
    public bool HasPrey;
    public bool HasMate;
    public List<Vector3> foodSources;
    public List<Vector3> waterSources;

    [Header("Positions")]
    public Vector3 Target;
    public Vector3 Destination;

    [Header("Group debugs")]
    public int GroupID;
    public Vector3 GroupTarget;
    public GroupState gState;
    public bool GroupHasPrey;

    public void Init(Animal context)
    {
        this.animal = context;

        RootState = context.InGroup ? new GroupBehaviourState(this, new AnimalStateFactory(this)) : new IdleState(this, new AnimalStateFactory(this));
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
        IsConsuming = animal.Model.IsConsuming;
        Age = animal.Model.Age;
        Hunger = animal.Model.Hunger;
        Thirst = animal.Model.Thirst;
        Hp = animal.Model.Hp;
        IsBreeding = animal.Model.IsBreeding;

        Arrived = animal.View.Arrived;
        AllFinished = animal.View.FinishedAnimation;

        foodSources = animal.Model.foodSources;
        waterSources = animal.Model.waterSources;
        Target = animal.Model.Target;
        Type = animal.Model.Type;
        Destination = animal.View.Agent.destination;
        HasMate = animal.Model.MateID > 0;
        GroupID = animal.GroupID;

        if (animal.InGroup)
        {
            GroupModel group = GroupManager.Instance.GetGroupByID(GroupID);

            gState = group.State;
            GroupTarget = group.CurrentTarget;
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

        if (animal.InGroup) GroupManager.Instance.GetGroupByID(animal.GroupID).LeaveGroup(animal.ID);
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

    public Structure GetStructure(Vector3 target)
    {
        return StructureManager.Instance.GetStructureByPosition(target);
    }
}
