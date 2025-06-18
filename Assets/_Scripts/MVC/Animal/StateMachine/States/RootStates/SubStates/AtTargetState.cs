using UnityEngine;

/// <summary>
/// Substate where the animal has arrived at the target.
/// It determines the appropriate interaction based on the type of the target (e.g., drink, eat, mate, hunt).
/// </summary>
public class AtTargetState : AnimalBaseState
{
    private ColliderTrigger[] triggers;

    public AtTargetState(AnimalStateMachine stateMachine, AnimalStateFactory factory, params ColliderTrigger[] triggers)
        : base(stateMachine, factory)
    {
        isRootState = false;
        this.triggers = triggers;
    }

    /// <summary>
    /// Called when entering the state. Stops movement and initiates an interaction substate based on the target type.
    /// </summary>
    public override void EnterState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        view.StopMovement();

        foreach (var trigger in triggers)
        {
            switch (trigger)
            {
                case ColliderTrigger.Prey:
                    if (model.PreyID > 0)
                        SetSubState(factory.Hunt());
                    break;

                case ColliderTrigger.Mate:
                    if (model.MateID > 0)
                        SetSubState(factory.Breed());
                    break;

                case ColliderTrigger.Water:
                    if (model.Target != Vector3.zero)
                        SetSubState(factory.Drink());
                    break;

                case ColliderTrigger.Food:
                    if (model.Target != Vector3.zero && model.PreyID < 0)
                        SetSubState(factory.Eat());
                    break;

                default:
                    model.SetTarget(Vector3.zero);
                    break;
            }
        }
    }

    /// <summary>
    /// Called every frame. Checks if the animal has lost the target or moved away unexpectedly.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    /// <summary>
    /// If the target is lost or the animal has moved away, switches to the appropriate state.
    /// </summary>
    public override void CheckSwitchStates()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (model.Target == Vector3.zero)
        {
            SwitchState(factory.Searching(triggers));
        }
        else if (!view.Arrived && model.Target != Vector3.zero)
        {
            SwitchState(factory.OnTarget(triggers));
        }
    }

    /// <summary>
    /// Cleans up any substates when exiting this state.
    /// </summary>
    public override void ExitState()
    {
        ExitAllSubStates();
    }

    /// <summary>
    /// This substate manages its own substates dynamically during EnterState.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Returns the name of the current substate, or a generic message if none is active.
    /// </summary>
    public override string ToString()
    {
        return currentSubState?.ToString() ?? "Interacting at target";
    }
}
