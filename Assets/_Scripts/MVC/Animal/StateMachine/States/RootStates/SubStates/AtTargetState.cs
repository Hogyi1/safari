using Unity.VisualScripting;
using UnityEngine;

public class AtTargetState : AnimalBaseState, IRootState
{
    ColliderTrigger[] triggers;
    public AtTargetState(AnimalStateMachine stateMachine, AnimalStateFactory factory, params ColliderTrigger[] triggers)
            : base(stateMachine, factory)
    {
        isRootState = false;
        this.triggers = triggers;
    }

    public override void EnterState()
    {
        context.animal.View.StopMovement();
        // Trigger alapján beállítjuk mit fogunk csinálni
        foreach (var trigger in triggers)
        {
            switch (trigger)
            {
                case ColliderTrigger.Prey:
                    if (context.animal.Model.Prey != null) SetSubState(factory.Hunt());
                    break;
                case ColliderTrigger.Mate:
                    if (context.animal.Model.Mate != null) SetSubState(factory.Breed());
                    break;
                case ColliderTrigger.Water:
                    if (context.animal.Model.Target != Vector3.zero) SetSubState(factory.Drink());
                    break;
                case ColliderTrigger.Food:
                    if (context.animal.Model.Target != Vector3.zero && context.animal.Model.Prey == null) SetSubState(factory.Eat());
                    break;
                default:
                    context.animal.Model.SetTarget(Vector3.zero);
                    break;
            }
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void CheckSwitchStates()
    {
        if (context.animal.Model.Target == Vector3.zero && context.animal.Group == null)
            SwitchState(factory.Searching(triggers));
        else if (!context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero)
            SwitchState(factory.OnTarget(triggers));
        else if (context.animal.Model.IsBreeding && context.animal.Group != null && context.animal.Model.Target == Vector3.zero)
            SwitchState(factory.Searching(triggers));
    }
    public override void ExitState()
    {
        currentSubState = null;
    }
    public override void InitializeSubState() { }

    public void CalculateModelData() { }

    public override string ToString()
    {
        if (currentSubState != null)
            return currentSubState?.ToString();
        return "At target";
    }
}