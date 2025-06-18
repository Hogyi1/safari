using UnityEngine;

/// <summary>
/// Substate triggered when the animal consumes water from a stationary source.
/// Handles periodic consumption and thirst restoration.
/// </summary>
public class DrinkingState : AnimalBaseState
{
    private IWaterSource waterSource;
    private float drinkTimer;
    private const float DrinkingInterval = 1f;

    public DrinkingState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = false;
    }

    /// <summary>
    /// Called when entering the state. Validates the water source and begins consumption.
    /// </summary>
    public override void EnterState()
    {
        var model = context.animal.Model;
        model.IsConsuming = true;

        var structure = context.GetStructure(model.Target);
        if (structure is not IWaterSource fs)
        {
            model.RemoveSource(model.Target);
            ExitState();
            return;
        }

        waterSource = fs;
        drinkTimer = DrinkingInterval;
    }

    /// <summary>
    /// Called every frame. Simulates water intake at fixed intervals and ends when full or source depleted.
    /// </summary>
    public override void UpdateState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (!model.IsConsuming || waterSource == null)
            return;

        drinkTimer += Time.deltaTime;
        if (drinkTimer < DrinkingInterval)
            return;

        var structure = context.GetStructure(model.Target);
        if (structure is not IWaterSource ws)
        {
            model.RemoveSource(model.Target);
            ExitState();
            return;
        }

        waterSource = ws;
        drinkTimer = 0f;

        int drank = waterSource.Consume(1);
        model.Drink(drank);
        view.Animator.SetBool(AnimalView.IsDrinking, true);

        if (drank == 0 || model.Thirst >= 100f)
            ExitState();
    }

    /// <summary>
    /// No transitions during drinking state.
    /// </summary>
    public override void CheckSwitchStates() { }

    /// <summary>
    /// No substates for drinking.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Called when exiting the state. Clears animation and optionally sets a new water target.
    /// </summary>
    public override void ExitState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (!context.animal.InGroup)
        {
            Vector3 nextTarget = model.GetNextWaterSourcePosition();
            model.SetTarget(nextTarget);
        }

        model.IsConsuming = false;
        view.Animator.SetBool(AnimalView.IsDrinking, false);
    }

    /// <summary>
    /// Returns a description of the state for debug or UI purposes.
    /// </summary>
    public override string ToString() => "Consuming water";

}
