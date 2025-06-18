using UnityEngine;

/// <summary>
/// Root state in which the animal actively seeks food to satisfy its hunger.
/// The animal will search, approach, and consume food or prey based on its diet.
/// </summary>
public class SeekFoodState : AnimalBaseState, IRootState
{
    public SeekFoodState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = true;
    }

    /// <summary>
    /// Called when entering the state. Determines the next food source position and initializes a substate accordingly.
    /// </summary>
    public override void EnterState()
    {
        Vector3 foodPosition = context.animal.Model.GetNextFoodSourcePosition();
        context.animal.SetTarget(foodPosition);

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
    /// Evaluates whether the animal should switch to another root state
    /// such as Dead, Group, SeekWater, SeekMate, or Idle.
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
            if (model.IsThirsty && !model.IsHungry)
                SwitchState(factory.SeekWater());

            else if (!model.IsHungry)
                SwitchState(factory.Idle());
        }
    }

    /// <summary>
    /// Initializes the appropriate substate based on whether the animal has a target and if it has arrived.
    /// Carnivores also search for prey.
    /// </summary>
    public override void InitializeSubState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        ColliderTrigger[] triggers = (model.Diet == DietType.Carnivore)
            ? new ColliderTrigger[] { ColliderTrigger.Prey, ColliderTrigger.Food }
            : new ColliderTrigger[] { ColliderTrigger.Food };

        if (model.Target == Vector3.zero)
            SetSubState(factory.Searching(triggers));

        else if (!view.Arrived && model.Target != Vector3.zero)
            SetSubState(factory.OnTarget(triggers));

        else if (view.Arrived && model.Target != Vector3.zero)
            SetSubState(factory.AtTarget(triggers));


        // Actively update detection logic
        view.RefreshDetection();
    }

    /// <summary>
    /// Called every frame. Checks for transitions and updates hunger-related need calculation.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
        CalculateModelData();
    }

    /// <summary>
    /// Applies hunger and thirst degradation, and health updates while seeking food.
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
        return currentSubState?.ToString() ?? "Hungry";
    }
}
