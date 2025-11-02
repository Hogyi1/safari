using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GroupState;

/// <summary>
/// Represents a group of animals of the same type that coordinate movement and needs.
/// </summary>
public class GroupModel
{
    // === State and type ===
    public int groupID;
    public GroupState State = Idle;
    public AnimalType AnimalType;
    public DietType DietType;

    // === Members and targets ===
    private List<int> members = new();
    private Vector3 center = Vector3.zero;
    private Vector3 Target = Vector3.zero;
    private int preyID = -1;
    public HashSet<int> ToMove = new();

    // === Sources and events ===
    public HashSet<Vector3> waterSources = new();
    public HashSet<Vector3> foodSources = new();
    private volatile bool sourceFound = false;
    public event Action<GroupModel> OnStateChanged;

    private static readonly float OFFSET = 4f;

    /// <summary>
    /// Initializes a new instance of the Group class.
    /// </summary>
    public GroupModel(GroupState State, List<int> members, AnimalType AnimalType, DietType DietType, int groupID)
    {
        this.members = members;
        this.AnimalType = AnimalType;
        this.State = State;
        this.groupID = groupID;
        this.DietType = DietType;

        UpdateCircle();
        InitializeSources();
        members.ForEach(t => GetAnimal(t).SetGroup(groupID));
    }

    /// <summary>
    /// Returns the Animal instance with the given ID.
    /// </summary>
    private Animal GetAnimal(int animalID) => AnimalManager.Instance.GetAnimalByID(animalID);

    /// <summary>
    /// Checks if all members have reached their target position.
    /// </summary>
    public bool AllArrived() => members.TrueForAll(t => GetAnimal(t).View.Arrived);

    /// <summary>
    /// Checks if all members have finished consuming.
    /// </summary>
    public bool AllFinished() => members.TrueForAll(t => !GetAnimal(t).Model.IsConsuming);

    /// <summary>
    /// The calculated central position of the group.
    /// </summary>
    public Vector3 Position => center;

    /// <summary>
    /// The current target.
    /// </summary>
    public Vector3 CurrentTarget => Target;

    /// <summary>
    /// Returns the current number of members.
    /// </summary>
    public int MemberCount => members.Count;

    /// <summary>
    /// Returns the current members.
    /// </summary>
    public List<int> Members => members;

    /// <summary>
    /// Attempts to add an animal to the group. Children always succeed.
    /// </summary>
    public bool TryJoin(int animalID, bool isChild)
    {
        var animal = GetAnimal(animalID);
        if (isChild)
        {
            members.Add(animalID);
            animal.SetGroup(groupID);
            return true;
        }

        if (!animal.Model.Type.Equals(AnimalType) || MemberCount >= 5 || members.Contains(animalID) || State != Idle)
            return false;

        members.Add(animalID);
        animal.SetGroup(groupID);
        return true;
    }

    /// <summary>
    /// Removes the specified animal from the group.
    /// </summary>
    public void LeaveGroup(int animalID)
    {
        if (!members.Contains(animalID))
            return;

        members.Remove(animalID);
        GetAnimal(animalID).SetGroup(-1);
    }

    /// <summary>
    /// Updates the center position of the group by averaging member positions.
    /// </summary>
    public void UpdateCircle()
    {
        Vector3 averageCenter = Vector3.zero;
        members.ForEach(t => averageCenter += GetAnimal(t)?.GetPosition() ?? Vector3.zero);
        center = averageCenter / MemberCount;
    }

    /// <summary>
    /// Returns a random point within the group's circle.
    /// </summary>
    public Vector3 GetRandomPointInCircle()
    {
        float Random01 = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));
        float r = GroupManager.RADIUS * Random01;
        float theta = Random01 * 2 * Mathf.PI;

        float x = center.x + r * Mathf.Cos(theta);
        float z = center.z + r * Mathf.Sin(theta);

        return new Vector3(x, 1, z);
    }

    /// <summary>
    /// Returns a random positional offset inside a square of the given max size.
    /// </summary>
    public Vector3 GetRandomOffset()
    {
        return new Vector3(UnityEngine.Random.Range(1, OFFSET), 0, UnityEngine.Random.Range(1, OFFSET));
    }

    /// <summary>
    /// Initializes shared resource knowledge from group members' memory.
    /// </summary>
    private void InitializeSources()
    {
        waterSources.Clear();
        foodSources.Clear();

        members.ForEach(t =>
        {
            waterSources.UnionWith(GetAnimal(t)?.Model.waterSources);
            foodSources.UnionWith(GetAnimal(t)?.Model.foodSources);
        });
    }

    /// <summary>
    /// Recalculates group state based on member needs.
    /// Called in every update cycle.
    /// </summary>
    public void CalculateGroupState()
    {
        if (MemberCount == 1)
        {
            LeaveGroup(members[0]);
            return;
        }

        Debug.Log("My group state is: " + State.ToString());

        List<int> hungryAnimals = members
            .Where(t => GetAnimal(t).Model.IsHungry && !GetAnimal(t).Model.IsConsuming)
            .ToList();

        List<int> thirstyAnimals = members
            .Where(t => !GetAnimal(t).Model.IsHungry && GetAnimal(t).Model.IsThirsty && !GetAnimal(t).Model.IsConsuming)
            .ToList();

        bool anyHungry = hungryAnimals.Any();
        bool anyThirsty = thirstyAnimals.Any();
        bool allDone = AllFinished();

        // Ha kevert a szükséglet (éhes + szomjas), különválasztjuk
        if (anyHungry && anyThirsty && allDone)
        {
            var overStimulated = thirstyAnimals.Where(t => GetAnimal(t).Model.IsOverstimulated).Any();

            if (overStimulated)
            {
                thirstyAnimals.ForEach(LeaveGroup);
                if (thirstyAnimals.Count >= 2)
                {
                    GroupManager.Instance.CreateGroup(thirstyAnimals, Thirsty);
                }
            }
            return;
        }

        // Ha senki nem éhes/szomjas ÉS mindenkinek kész az állapota
        if (!anyHungry && !anyThirsty && allDone)
        {
            SetState(Idle);
            return;
        }

        // Ha mindenki kész, de van még éhség/szomjúság
        if (allDone)
        {
            if (anyHungry)
            {
                SetState(foodSources.Any() || preyID > 0 ? Hungry : SearchingFood);
            }
            else if (anyThirsty)
            {
                SetState(waterSources.Any() ? Thirsty : SearchingWater);
            }

            return;
        }
    }

    /// <summary>
    /// Updates the group state and notifies listeners if the state changed.
    /// </summary>
    private void SetState(GroupState newState)
    {
        if (!newState.Equals(State))
        {
            State = newState;
            OnStateChanged?.Invoke(this);
        }
    }

    /// <summary>
    /// Coroutine that makes the group search for a resource (food, water, prey) on the map.
    /// It runs until a source is found or the search is cancelled.
    /// If no known sources are saved or all have been visited, this exploration is triggered.
    /// </summary>
    public IEnumerator StartGroupSearching()
    {
        sourceFound = false;
        preyID = -1;
        Target = Vector3.zero;

        // Reset all member movement and clear their current targets
        members.ForEach(t =>
        {
            var animal = GetAnimal(t);
            animal?.View.ResetMovement();
            animal?.Model.SetTarget(Vector3.zero);
        });

        Debug.Log("Started searching: " + State.ToString());

        // Continue until some resource (food, water, prey) is discovered
        while (!sourceFound)
        {
            // Generate a random position somewhere on the map
            Target = GroupManager.Instance.GetRandomPointOnMap(Position);

            // Assign slightly randomized target offsets to each member's View
            members.ForEach(t => GetAnimal(t)?.View.SetTarget(Target + GetRandomOffset()));

            Debug.Log("Next search target: " + Target);

            // Wait until either all group members have arrived, or a source was found meanwhile
            yield return new WaitUntil(() => AllArrived() || sourceFound);
        }

        Debug.Log("Search stopped – a source was found.");

        // At this point, the source should have already been added to shared memory by the discoverer
    }

    /// <summary>
    /// Coroutine for setting the group's movement target based on known food or water sources.
    /// This is only used when the group has remembered positions — hunting behavior is handled separately.
    /// If the group members encounter an empty source (e.g., a dry feeder or removed tree), they forget it.
    /// </summary>
    public IEnumerator SetGroupTargeting()
    {
        // Choose the active sources based on current group state (hungry or thirsty).
        HashSet<Vector3> activeSources = (State == Hungry ? foodSources : waterSources);

        // Reset movement for all group members before assigning new targets.
        members.ForEach(t =>
        {
            var animal = GetAnimal(t);
            animal?.View.ResetMovement();
            animal?.Model.SetTarget(Vector3.zero);
        });

        GroupState startingState = State;

        while (startingState == State)
        {
            if (preyID < 0)
            {
                // Not hunting: target the closest known source to the group's center.
                Target = activeSources.OrderBy(t => Vector3.Distance(t, center)).First();

                // Send all members to the selected target.
                members.ForEach(t =>
                {
                    GetAnimal(t).SetTarget(Target);
                    Debug.Log("Moving to source: " + Target);
                });
            }
            else
            {
                // Hunting: use prey's current position as the target.
                Target = GetAnimal(preyID).View.transform.position;

                members.ForEach(t =>
                {
                    var animal = GetAnimal(t);
                    animal.Model.SetPrey(preyID);
                    animal.Model.SetTarget(Target);
                    animal.View.Follow(GetAnimal(preyID).View);
                });
            }

            // Wait until all group members have arrived and finished their task (e.g., eating).
            // Currently, no animation is handled, and feeding is considered instant.
            yield return new WaitUntil(() => AllArrived() && AllFinished());

            // Remove the visited source from the list to avoid revisiting it.
            // If all sources are exhausted, CalculateGroupState will change the state,
            // which automatically stops this coroutine.
            activeSources.Remove(Target);
            Debug.Log("Proceeding to next target...");
        }

        // Clear prey info once targeting is done or interrupted.
        activeSources.Add(Target);
        members.ForEach(t => GetAnimal(t).Model.ClearPrey());
    }

    /// <summary>
    /// Called when a group member discovers a valid food or water source during the search phase.
    /// Updates the appropriate source list and switches the group's state accordingly.
    /// </summary>
    /// <param name="targetPosition">The world position of the found source (must not be Vector3.zero).</param>
    public void SourceFound(Vector3 targetPosition)
    {
        // Ignore invalid (zero) positions.
        if (!targetPosition.Equals(Vector3.zero))
        {
            if (State.Equals(SearchingFood))
            {
                foodSources.Add(targetPosition); // Add current group Target to foodSources.
                SetState(Hungry);        // Switch to Hungry state.
            }
            else if (State.Equals(SearchingWater))
            {
                waterSources.Add(targetPosition); // Add current group Target to waterSources.
                SetState(Thirsty);        // Switch to Thirsty state.
            }

            sourceFound = true;
            Debug.Log("Source found");
        }
    }

    /// <summary>
    /// Called when a group member detects prey during the search phase.
    /// Sets the group's prey ID and flags the source as found (used to interrupt search).
    /// </summary>
    /// <param name="preyID">The unique ID of the discovered prey.</param>
    public void PreyFound(int preyID)
    {
        // Only carnivores can target prey, and only if they don't already have one.
        if (DietType.Equals(DietType.Carnivore) && this.preyID < 0)
        {
            this.preyID = preyID;
            sourceFound = true;
            Debug.Log("Prey found");
        }
    }

    /// <summary>
    /// Reinitializes the group's known food and water sources (usually after state reset or regrouping).
    /// </summary>
    public void UpdateSources() => InitializeSources();

}

/// <summary>
/// Represents the possible behavioral states of an animal group.
/// State transitions typically depend on internal needs or external discoveries.
/// </summary>
public enum GroupState
{
    /// <summary> The group is idle and not currently searching or moving toward any target. /// </summary>
    Idle,

    /// <summary>  The group is actively searching for a food source, but has not found one yet. /// </summary>
    SearchingFood,

    /// <summary> The group is actively searching for a water source, but has not found one yet. /// </summary>
    SearchingWater,

    /// <summary> The group has located a food source and is currently moving toward or interacting with it. /// </summary>
    Hungry,

    /// <summary> The group has located a water source and is currently moving toward or interacting with it. /// </summary>
    Thirsty
}
