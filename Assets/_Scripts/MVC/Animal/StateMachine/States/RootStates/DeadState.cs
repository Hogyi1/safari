using UnityEngine;

public class DeadState : AnimalBaseState, IRootState
{
    public DeadState(AnimalStateMachine stateMachine, AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = true;
    }

    public void CalculateModelData()
    {
        return;
    }

    public override void CheckSwitchStates()
    {
        return;
    }

    // X idő után tűnjön el a testünk
    public override void EnterState()
    {
        context.MarkAsDead();
    }

    public override void ExitState()
    {
        return;
    }

    public override void InitializeSubState()
    {
        return;
    }

    public override void UpdateState()
    {
        return;
    }

    public override string ToString()
    {
        return "Dead";
    }
}
