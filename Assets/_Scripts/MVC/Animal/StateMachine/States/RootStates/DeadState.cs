using UnityEngine;

/// <summary>
/// Root state representing a dead animal. No behavior is executed in this state,
/// and the animal will eventually be removed from the simulation.
/// </summary>
public class DeadState : AnimalBaseState, IRootState
{
    public DeadState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = true;
    }

    /// <summary>
    /// No data calculation needed for dead animals.
    /// </summary>
    public void CalculateModelData() { }

    /// <summary>
    /// No state switching logic for dead animals.
    /// </summary>
    public override void CheckSwitchStates() { }

    /// <summary>
    /// Called when entering the dead state. Initiates delayed removal of the animal.
    /// </summary>
    public override void EnterState()
    {
        context.MarkAsDead();
    }

    /// <summary>
    /// No exit logic required for the dead state.
    /// </summary>
    public override void ExitState() { }

    /// <summary>
    /// No substate initialization for the dead state.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// No update logic needed for dead animals.
    /// </summary>
    public override void UpdateState() { }

    /// <summary>
    /// Returns the name of the state for debugging or UI display.
    /// </summary>
    public override string ToString()
    {
        return "Dead";
    }
}
