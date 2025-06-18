using UnityEngine;

/// <summary>
/// Root state in which the animal actively seeks water to satisfy its thirst.
/// The animal will search, approach, and consume water if necessary.
/// </summary>
public class SeekWaterState : AnimalBaseState, IRootState
{
    public SeekWaterState(AnimalStateMachine stateMachine,
                          AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = true;
    }

    /// <summary>
    /// Called when entering the state. Determines the next water source position and initializes a substate accordingly.
    /// </summary>
    public override void EnterState()
    {
        Vector3 waterPosition = context.animal.Model.GetNextWaterSourcePosition();
        context.animal.SetTarget(waterPosition);

        InitializeSubState();
    }

    /// <summary>
    /// Called once when exiting the state. Clears the current target.
    /// </summary>
    public override void ExitState()
    {
        context.animal.Model.SetTarget(Vector3.zero);
    }

    /// <summary>
    /// Evaluates whether the animal should switch to another root state,
    /// such as Idle, SeekFood, SeekMate, Dead, or Group.
    /// </summary>
    public override void CheckSwitchStates()
    {
        var model = context.animal.Model;

        if (model.IsDead)
            SwitchState(factory.Dead());


        if (context.animal.InGroup)
            SwitchState(factory.Group());


        if (!model.IsConsuming)
        {
            if (!model.IsThirsty && model.IsHungry)
                SwitchState(factory.SeekFood());

            else if (!model.IsThirsty)
                SwitchState(factory.Idle());
        }
    }

    /// <summary>
    /// Initializes the appropriate substate based on whether the animal has a target and if it has arrived.
    /// </summary>
    public override void InitializeSubState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (model.Target == Vector3.zero)
            SetSubState(factory.Searching(ColliderTrigger.Water));

        else if (!view.Arrived && model.Target != Vector3.zero)
            SetSubState(factory.OnTarget(ColliderTrigger.Water));

        else if (view.Arrived && model.Target != Vector3.zero)
            SetSubState(factory.AtTarget(ColliderTrigger.Water));

    }

    /// <summary>
    /// Called every frame. Checks for transitions and updates thirst-related need calculation.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
        CalculateModelData();
    }

    /// <summary>
    /// Applies thirst-related need degradation based on water-seeking effort.
    /// </summary>
    public void CalculateModelData()
    {
        context.animal.Model.CalculateNeeds(0.65f);
    }

    /// <summary>
    /// Returns a readable name of the current substate or a default description if none is active.
    /// </summary>
    public override string ToString()
    {
        return currentSubState?.ToString() ?? "Thirsty";
    }
}
