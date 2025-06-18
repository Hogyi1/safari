using UnityEngine;

/// <summary>
/// Root state in which the animal actively seeks a mate for reproduction.
/// Handles searching, approaching, and interacting with potential mates.
/// </summary>
public class SeekMateState : AnimalBaseState, IRootState
{
    public SeekMateState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = true;
    }

    /// <summary>
    /// Called when entering the state. Resets the target and initializes the mating substate logic.
    /// </summary>
    public override void EnterState()
    {
        context.animal.Model.SetTarget(Vector3.zero);
        InitializeSubState();
    }

    /// <summary>
    /// Called once when exiting the state. Clears the current target.
    /// </summary>
    public override void ExitState()
    {
        context.animal.Model.StopBreeding();
        context.animal.Model.SetTarget(Vector3.zero);
    }

    /// <summary>
    /// Evaluates whether the animal should switch to another root state,
    /// such as Dead, Group, SeekFood, SeekWater, or Idle.
    /// </summary>
    public override void CheckSwitchStates()
    {
        var model = context.animal.Model;

        if (model.IsDead)
            SwitchState(factory.Dead());


        if (!model.IsBreeding)
            SwitchState(factory.Group());


        if (!context.animal.InGroup)
        {
            if (model.IsHungry)
                SwitchState(factory.SeekFood());

            else if (model.IsThirsty)
                SwitchState(factory.SeekWater());

            else if (!model.IsBreeding)
                SwitchState(factory.Idle());
        }
    }

    /// <summary>
    /// Initializes the appropriate substate for mating behavior based on presence and location of a potential mate.
    /// </summary>
    public override void InitializeSubState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (model.Target == Vector3.zero && model.MateID < 0)
            SetSubState(factory.Searching(ColliderTrigger.Mate));

        else if (!view.Arrived && model.Target != Vector3.zero && model.MateID > 0)
            SetSubState(factory.OnTarget(ColliderTrigger.Mate));

        else if (view.Arrived && model.Target != Vector3.zero && model.MateID > 0)
            SetSubState(factory.AtTarget(ColliderTrigger.Mate));


        view.RefreshDetection();
    }

    /// <summary>
    /// Called every frame. Checks for transitions and updates needs related to mating behavior.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
        CalculateModelData();
    }

    /// <summary>
    /// Applies moderate degradation to hunger and thirst while searching for a mate.
    /// </summary>
    public void CalculateModelData()
    {
        context.animal.Model.CalculateNeeds(0.5f);
    }

    /// <summary>
    /// Returns a readable name of the current substate or a default description if none is active.
    /// </summary>
    public override string ToString()
    {
        return currentSubState?.ToString() ?? "Searching for mate";
    }
}
