using Unity.VisualScripting;
using UnityEngine;

public abstract class AnimalBaseState
{
    protected AnimalStateMachine context;
    protected AnimalStateFactory factory;
    protected AnimalBaseState currentSubState;
    protected AnimalBaseState currentSuperState;
    protected bool isRootState = false;

    public AnimalBaseState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
    {
        this.context = stateMachine;
        this.factory = factory;
    }

    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract void EnterState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();
    public void UpdateStates()
    {
        UpdateState();
        if (currentSubState != null) currentSubState.UpdateStates();
    }
    protected void SwitchState(AnimalBaseState newState)
    {
        ExitState();

        newState.EnterState();

        if (isRootState)
            context.RootState = newState;
        else if (currentSuperState != null)
            currentSuperState.SetSubState(newState);
    }
    protected void SetSuperState(AnimalBaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubState(AnimalBaseState newSubState)
    {
        currentSubState = newSubState;
        newSubState.EnterState();
        newSubState.SetSuperState(this);
    }

    public AnimalBaseState GetSubState()
    {
        return currentSubState;
    }
}

public interface IRootState
{
    void CalculateModelData();
}