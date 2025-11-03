using UnityEngine;

/// <summary>
/// Root state representing the behavior of an animal while being part of a group.
/// Controls substate transitions based on the current group state.
/// </summary>
public class GroupBehaviourState : AnimalBaseState, IRootState
{
    private GroupModel group;

    public GroupBehaviourState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = true;
    }

    public int GetGroupID() => group.groupID;

    /// <summary>
    /// Called when the state is entered. Subscribes to group change events and sets up listeners.
    /// </summary>
    public override void EnterState()
    {
        context.animal.OnGroupChange += GetGroup;
        GetGroup();
        HandleStateChanged(group);
    }

    /// <summary>
    /// Resolves the current group by ID and registers to its state change event.
    /// Ensures only one handler is subscribed at a time.
    /// </summary>
    private void GetGroup()
    {
        if (group != null)
            group.OnStateChanged -= HandleStateChanged;

        group = GroupManager.Instance.GetGroupByID(context.animal.GroupID);

        if (group != null)
            group.OnStateChanged += HandleStateChanged;
    }

    /// <summary>
    /// Handles changes in the group's state and switches substates accordingly.
    /// </summary>
    /// <param name="group">The group that triggered the state change.</param>
    private void HandleStateChanged(GroupModel group)
    {
        if (group.groupID != context.animal.GroupID)
            return;

        var triggers = GetTriggers(group.State);

        switch (group.State)
        {
            case GroupState.Idle:
                SetSubState(factory.GroupIdle());
                break;

            case GroupState.Hungry:
            case GroupState.Thirsty:
                SetSubState(factory.OnTarget(triggers));
                break;

            case GroupState.SearchingFood:
            case GroupState.SearchingWater:
                SetSubState(factory.Searching(triggers));
                break;
        }

        // Refresh surroundings (e.g., prey, water, food)
        context.animal.View.RefreshDetection();
    }

    /// <summary>
    /// Returns the relevant detection triggers based on the group state and the animal's diet.
    /// </summary>
    private ColliderTrigger[] GetTriggers(GroupState state)
    {
        return state switch
        {
            GroupState.Hungry or GroupState.SearchingFood =>
                context.animal.Model.Diet == DietType.Carnivore
                    ? new[] { ColliderTrigger.Prey, ColliderTrigger.Food }
                    : new[] { ColliderTrigger.Food },

            GroupState.Thirsty or GroupState.SearchingWater =>
                new[] { ColliderTrigger.Water },

            _ => null,
        };
    }

    /// <summary>
    /// Cleans up subscriptions when exiting the state.
    /// </summary>
    public override void ExitState()
    {
        context.animal.OnGroupChange -= GetGroup;

        if (group != null)
            group.OnStateChanged -= HandleStateChanged;

        ExitAllSubStates();
    }

    /// <summary>
    /// Evaluates whether the animal should exit the group state (e.g., if it left the group or died).
    /// </summary>
    public override void CheckSwitchStates()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (model.IsDead)
            SwitchState(factory.Dead());

        if (!context.animal.InGroup)
        {
            Debug.Log("Animal has left the group.");

            if (model.IsHungry)
                SwitchState(factory.SeekFood());

            else if (model.IsThirsty)
                SwitchState(factory.SeekWater());

            else
                SwitchState(factory.Idle());
        }
        else if (model.IsBreeding && group.State.Equals(GroupState.Idle))
            SwitchState(factory.SeekMate());
    }

    /// <summary>
    /// Not used – substates are controlled externally via group state changes.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Called each frame. Performs physiological updates and state checking.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
        CalculateModelData();
    }

    /// <summary>
    /// Updates the animal's hunger and thirst based on group activity intensity.
    /// </summary>
    public void CalculateModelData()
    {
        context.animal.Model.CalculateNeeds(0.7f);
    }

    /// <summary>
    /// Returns the current substate description or a default label.
    /// </summary>
    public override string ToString()
    {
        return "Group activity";
        return currentSubState?.ToString() ?? "Group activity";
    }
}
