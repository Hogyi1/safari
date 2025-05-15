using UnityEngine;

public class OnTargetState : AnimalBaseState
{
    ColliderTrigger[] triggers;

    public OnTargetState(AnimalStateMachine stateMachine, AnimalStateFactory factory, params ColliderTrigger[] triggers)
        : base(stateMachine, factory)
    {
        isRootState = false;
        this.triggers = triggers;
    }

    public override void EnterState()
    {
        context.animal.View.ResetMovement();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    // Haladunk a cél felé
    public override void CheckSwitchStates()
    {
        if (context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero)
            SwitchState(factory.AtTarget(triggers));
        else if (context.animal.Model.Target == Vector3.zero)
            SwitchState(factory.Searching(triggers));
    }

    public override void ExitState() { }
    public override void InitializeSubState() { }

    public override string ToString()
    {
        return "On target";
    }
}