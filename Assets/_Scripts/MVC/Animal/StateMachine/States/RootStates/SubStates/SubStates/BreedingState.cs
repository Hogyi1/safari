using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Substate triggered when the animal attempts reproduction with a valid mate.
/// Handles partner validation, breeding logic, and cooldown.
/// </summary>
public class BreedingState : AnimalBaseState
{
    private Animal mate;
    private float timer;
    private const float Cooldown = 2f;

    public BreedingState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = false;
    }

    /// <summary>
    /// Called when entering the state. Attempts to locate the mating partner.
    /// </summary>
    public override void EnterState()
    {
        var model = context.animal.Model;
        model.IsConsuming = true;

        mate = AnimalManager.Instance.GetAnimalByID(model.MateID);
        if (mate.IsUnityNull())
        {
            ExitState();
            return;
        }

        timer = Cooldown;
    }

    /// <summary>
    /// Called every frame. Waits for cooldown then triggers the breeding logic.
    /// </summary>
    public override void UpdateState()
    {
        var model = context.animal.Model;

        if (!model.IsBreeding || model.MateID < 0)
            return;

        timer += Time.deltaTime;
        if (timer < Cooldown)
            return;

        if (AnimalManager.Instance.GetAnimalByID(model.MateID).IsUnityNull())
        {
            ExitState();
            return;
        }

        mate.View.StopMovement();
        timer = 0f;

        if (mate.Model.TryBreeding(context.animal.ID))
        {
            AnimalManager.Instance.Breed(mate.ID, context.animal.ID);
            model.StopBreeding();
        }

        ExitState();
    }

    /// <summary>
    /// No transitions during the breeding process.
    /// </summary>
    public override void CheckSwitchStates() { }

    /// <summary>
    /// This substate does not contain any substates.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Called when exiting the state. Resets movement and mating flags.
    /// </summary>
    public override void ExitState()
    {
        var model = context.animal.Model;

        mate?.View.ResetMovement();
        model.IsConsuming = false;
        model.SetTarget(Vector3.zero);
    }

    /// <summary>
    /// Returns a description of the state for debug or UI purposes.
    /// </summary>
    public override string ToString() => "Ultimate smash bros";
}
