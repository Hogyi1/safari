using UnityEngine;

/// <summary>
/// Represents a passive sleeping substate where the animal is resting and not moving.
/// Used mainly during night or after long activity.
/// </summary>
public class SleepingState : AnimalBaseState
{
    private float sleepTimer;

    public SleepingState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = false;
    }

    /// <summary>
    /// Called when entering the sleeping state.
    /// Triggers the sleeping animation, stops movement, and sets a sleep duration.
    /// </summary>
    public override void EnterState()
    {
        context.animal.View.Animator.SetBool(AnimalView.IsSleeping, true);
        sleepTimer = Random.Range(10f, 30f);
        context.animal.View.StopMovement();
    }

    /// <summary>
    /// Called every frame to update the sleep timer and evaluate transitions.
    /// </summary>
    public override void UpdateState()
    {
        sleepTimer -= Time.deltaTime;
        CheckSwitchStates();
    }

    /// <summary>
    /// Checks whether the sleep has finished.
    /// If it is no longer night, transition to a groggy stationary state.
    /// </summary>
    public override void CheckSwitchStates()
    {
        if (sleepTimer <= 0f && !TimeManager.Instance.IsNight)
        {
            SwitchState(factory.Stationary());
        }
    }

    /// <summary>
    /// Called when exiting sleep.
    /// Resets sleeping animation parameter.
    /// </summary>
    public override void ExitState()
    {
        context.animal.View.Animator.SetBool(AnimalView.IsSleeping, false);
    }

    /// <summary>
    /// Sleeping has no substates.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Returns a string representation of this state.
    /// </summary>
    public override string ToString() => "Sleeping";
}
