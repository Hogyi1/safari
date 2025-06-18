using UnityEngine;

/// <summary>
/// Represents an idle behavior state within a group context,
/// where the group is not currently performing any specific task.
/// </summary>
public class GroupIdleState : AnimalBaseState
{
    public GroupIdleState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = false;
    }

    /// <summary>
    /// Called when entering the group idle state.
    /// Initializes idle sub-behaviors like sleeping, wandering, or standing still.
    /// </summary>
    public override void EnterState()
    {
        InitializeSubState();
    }

    /// <summary>
    /// Called every frame. Updates physiological needs and checks for state transitions.
    /// </summary>
    public override void UpdateState() { }

    /// <summary>
    /// Handles transition logic out of group idle state.
    /// This is likely unused, as transitions should be handled by GroupBehaviourState.
    /// </summary>
    public override void CheckSwitchStates()
    {
        // GroupBehaviourState should control root switching — no logic here.
    }

    /// <summary>
    /// Called when exiting the state. Cleans up substates.
    /// </summary>
    public override void ExitState()
    {
        ExitAllSubStates();
    }

    /// <summary>
    /// Initializes a randomized idle sub-behavior like sleeping or wandering.
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
    /// Returns the name of the active substate or a fallback if idle.
    /// </summary>
    public override string ToString()
    {
        return currentSubState?.ToString() ?? "In group chilling";
    }
}
