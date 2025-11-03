using System.Linq;
using UnityEngine;

/// <summary>
/// Substate where the animal has a known target and is navigating toward it.
/// If the target is reached, transitions to AtTarget; if lost, returns to Searching.
/// </summary>
public class OnTargetState : AnimalBaseState
{
    private ColliderTrigger[] triggers;

    public OnTargetState(AnimalStateMachine stateMachine, AnimalStateFactory factory, params ColliderTrigger[] triggers)
        : base(stateMachine, factory)
    {
        isRootState = false;
        this.triggers = triggers;
    }

    /// <summary>
    /// Called when entering the state. Resets movement to avoid overshooting.
    /// </summary>
    public override void EnterState()
    {
        context.animal.View.ResetMovement();
    }

    /// <summary>
    /// Called every frame. Checks if the animal has arrived or needs to re-enter search.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    /// <summary>
    /// Switches to AtTarget if arrived, or to Searching if the target is lost.
    /// </summary>
    public override void CheckSwitchStates()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (view.Arrived && model.Target != Vector3.zero)
            SwitchState(factory.AtTarget(triggers));
        else if (model.Target == Vector3.zero)
            SwitchState(factory.Searching(triggers));
    }

    /// <summary>
    /// No additional cleanup required on exit.
    /// </summary>
    public override void ExitState() { }

    /// <summary>
    /// This substate has no substates.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Returns a descriptive name of the current target being approached.
    /// </summary>
    public override string ToString()
    {
        var model = context.animal.Model;

        if (triggers.Contains(ColliderTrigger.Water))
            return "Approaching water source";

        if (triggers.Contains(ColliderTrigger.Mate) && model.MateID > 0)
            return "Approaching potential mate";

        if (triggers.Contains(ColliderTrigger.Prey) && model.PreyID > 0)
            return "Chasing prey";

        if (triggers.Contains(ColliderTrigger.Food))
            return "Heading towards food source";

        return "Approaching target";
    }
}
