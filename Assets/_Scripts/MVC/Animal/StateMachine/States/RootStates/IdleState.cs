using UnityEngine;

/// <summary>
/// Represents the idle root state of an animal when it is not engaged in any urgent behavior
/// (e.g., hunger, thirst, reproduction) and not part of a group.
/// </summary>
public class IdleState : AnimalBaseState, IRootState
{
    public IdleState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = true;
    }

    /// <summary>
    /// Checks for transitions to other root states based on the animal's condition.
    /// Prioritizes death, group membership, hunger, thirst, and mating.
    /// </summary>
    public override void CheckSwitchStates()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (model.IsDead)
            SwitchState(factory.Dead());

        else if (context.animal.InGroup)
            SwitchState(factory.Group());

        else if (model.IsHungry)
            SwitchState(factory.SeekFood());

        else if (model.IsThirsty)
            SwitchState(factory.SeekWater());

        else if (model.IsBreeding)
            SwitchState(factory.SeekMate());
    }

    /// <summary>
    /// Called when entering the idle state. Subscribes to animal detection events and sets an initial substate.
    /// </summary>
    public override void EnterState()
    {
        context.animal.View.OnAnimalFound += HandleAnimalFound;
        InitializeSubState();
    }

    /// <summary>
    /// Called when exiting the idle state. Unsubscribes from events and exits substates.
    /// </summary>
    public override void ExitState()
    {
        context.animal.View.OnAnimalFound -= HandleAnimalFound;
        ExitAllSubStates();
    }

    /// <summary>
    /// Randomly initializes a substate such as sleeping, stationary or wandering behavior
    /// to simulate idle activity during downtime.
    /// </summary>
    public override void InitializeSubState()
    {
        float roll = Random.Range(0f, 1f);

        if (roll <= 0.1f || TimeManager.Instance.IsNight)
            SetSubState(factory.Sleeping());

        else if (roll <= 0.5f)
            SetSubState(factory.Stationary());

        else
            SetSubState(factory.Wandering());
    }

    /// <summary>
    /// Called every frame. Handles state transitions and updates physiological data.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
        CalculateModelData();
    }

    /// <summary>
    /// Updates the animal's hunger and thirst levels based on its current idle substate.
    /// </summary>
    public void CalculateModelData()
    {
        switch (currentSubState)
        {
            case SleepingState:
                context.animal.Model.CalculateNeeds(0.1f);
                break;
            case StationaryState:
                context.animal.Model.CalculateNeeds(0.3f);
                break;
            case WanderingState:
                context.animal.Model.CalculateNeeds(0.5f);
                break;
        }
    }

    /// <summary>
    /// Handles detection of nearby animals.
    /// If both animals are ungrouped and share the same species, a group formation is suggested.
    /// If the other animal is in a group and this one is not, it tries to join that group.
    /// </summary>
    /// <param name="viewID">The ID of the detected animal.</param>
    private void HandleAnimalFound(int viewID)
    {
        Animal self = context.animal;
        Animal other = AnimalManager.Instance.GetAnimalByID(viewID);

        if (other == null || other.Model.IsDead || self.Model.IsDead)
            return;

        if (other.Model.Type != self.Model.Type)
            return;

        if (!self.InGroup && !other.InGroup)
            AnimalManager.Instance.SuggestGroupFormation(self.ID, other.ID);

        else if (other.InGroup && !self.InGroup)
            AnimalManager.Instance.TryJoinGroup(self.ID, other.GroupID);
    }

    /// <summary>
    /// Returns the name of the active substate or "Idle" if none is set.
    /// </summary>
    public override string ToString()
    {
        return currentSubState?.ToString() ?? "Idle";
    }
}
