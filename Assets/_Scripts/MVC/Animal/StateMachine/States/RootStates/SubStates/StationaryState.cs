using UnityEngine;

/// <summary>
/// Represents a brief stationary state where the animal pauses without moving,
/// often as part of idle or natural behavior like resting or observing.
/// </summary>
public class StationaryState : AnimalBaseState
{
    private float stationaryTimer;

    public StationaryState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = false;
    }

    /// <summary>
    /// Called upon entering the state. Initializes a short pause timer and stops movement.
    /// </summary>
    public override void EnterState()
    {
        // Pause duration between 1–3 seconds
        stationaryTimer = Random.Range(1f, 3f);
        context.animal.View.StopMovement();
    }

    /// <summary>
    /// Called every frame. Decreases timer and evaluates whether to switch state.
    /// </summary>
    public override void UpdateState()
    {
        stationaryTimer -= Time.deltaTime;
        CheckSwitchStates();
    }

    /// <summary>
    /// Checks if the animal should switch to another substate, based on timer and context.
    /// </summary>
    public override void CheckSwitchStates()
    {
        if (stationaryTimer > 0f)
            return;

        float roll = Random.Range(0f, 1f);
        bool isNight = TimeManager.Instance.IsNight;
        float hunger = context.animal.Model.Hunger;

        // Chance to sleep if hungry and random roll low enough, or it's night
        if ((roll <= 0.3f && hunger >= 80f) || isNight)
            SwitchState(factory.Sleeping());
        else
            SwitchState(factory.Wandering());

    }

    /// <summary>
    /// Called when exiting the state. No cleanup necessary.
    /// </summary>
    public override void ExitState() { }

    /// <summary>
    /// No substates used in stationary behavior.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Returns a human-readable name for the current substate.
    /// </summary>
    public override string ToString() => "Chilling";
}
