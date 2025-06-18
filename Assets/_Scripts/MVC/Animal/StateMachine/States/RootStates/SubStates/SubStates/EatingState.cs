using UnityEngine;

/// <summary>
/// Substate triggered when the animal consumes food from a stationary source.
/// Handles periodic consumption and hunger restoration.
/// </summary>
public class EatingState : AnimalBaseState
{
    private IFoodSource foodSource;
    private float eatTimer;
    private const float EatingInterval = 2f;

    public EatingState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = false;
    }

    /// <summary>
    /// Called when entering the state. Validates the food source and begins consumption.
    /// </summary>
    public override void EnterState()
    {
        var model = context.animal.Model;
        model.IsConsuming = true;

        var structure = context.GetStructure(model.Target);
        if (structure is not IFoodSource fs)
        {
            model.RemoveSource(model.Target);
            ExitState();
            return;
        }

        foodSource = fs;
        eatTimer = EatingInterval;
    }

    /// <summary>
    /// Called every frame. Simulates food intake at fixed intervals and ends when full or source depleted.
    /// </summary>
    public override void UpdateState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (!model.IsConsuming || foodSource == null)
            return;

        eatTimer += Time.deltaTime;
        if (eatTimer < EatingInterval)
            return;

        if (context.GetStructure(model.Target) is not IFoodSource fs)
        {
            model.RemoveSource(model.Target);
            ExitState();
            return;
        }

        foodSource = fs;
        eatTimer = 0f;

        int eaten = foodSource.Consume(1);
        model.Eat(eaten);
        view.Animator.SetBool(AnimalView.IsEating, true);

        if (eaten == 0 || model.Hunger >= 99f)
            ExitState();
    }

    /// <summary>
    /// No transitions during eating state.
    /// </summary>
    public override void CheckSwitchStates() { }

    /// <summary>
    /// No substates for eating.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Called when exiting the state. Resets animation and optionally sets a new food target.
    /// </summary>
    public override void ExitState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (!context.animal.InGroup && model.IsHungry)
        {
            Vector3 nextTarget = model.GetNextFoodSourcePosition();
            context.animal.SetTarget(nextTarget);
        }

        model.IsConsuming = false;
        view.Animator.SetBool(AnimalView.IsEating, false);
    }

    /// <summary>
    /// Returns a description of the state for debug or UI purposes.
    /// </summary>
    public override string ToString() => "Consuming food";

}
