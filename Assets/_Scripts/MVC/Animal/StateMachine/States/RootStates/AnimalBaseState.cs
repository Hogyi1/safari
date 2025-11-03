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
        currentSubState?.UpdateStates();
    }
    protected void SwitchState(AnimalBaseState newState)
    {
        currentSubState?.ExitState();
        currentSubState = null;

        ExitState();

        if (newState.isRootState)
            newState.EnterState();

        if (isRootState)
            context.RootState = newState;
        else
            currentSuperState?.SetSubState(newState);
    }
    protected void SetSuperState(AnimalBaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubState(AnimalBaseState newSubState)
    {
        currentSubState?.ExitState();
        currentSubState = newSubState;
        newSubState.EnterState();
        newSubState.SetSuperState(this);
    }

    protected void ExitAllSubStates()
    {
        if (currentSubState != null)
        {
            currentSubState.ExitAllSubStates();
            currentSubState.ExitState();
            currentSubState = null;
        }
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