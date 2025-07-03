using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Substate triggered when the animal hunts and consumes a prey.
/// Handles the prey elimination and hunger restoration over time.
/// </summary>
public class HuntingState : AnimalBaseState
{
    private float eatTimer;
    private const float EatingInterval = 2f;

    public HuntingState(AnimalStateMachine stateMachine,
                        AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = false;
    }

    /// <summary>
    /// Called when entering the state. Kills the prey and starts the eating process.
    /// </summary>
    public override void EnterState()
    {
        var model = context.animal.Model;
        model.IsConsuming = true;

        var preyAnimal = AnimalManager.Instance.GetAnimalByID(model.PreyID);
        if (preyAnimal.IsUnityNull())
        {
            ExitState();
            return;
        }

        AnimalManager.Instance.KillAnimal(model.PreyID);
        eatTimer = EatingInterval;
    }

    /// <summary>
    /// Called every frame. Simulates periodic consumption of prey and ends the state if full.
    /// </summary>
    public override void UpdateState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (!model.IsConsuming || model.PreyID <= 0)
            return;

        eatTimer += Time.deltaTime;
        if (eatTimer < EatingInterval)
            return;

        if (AnimalManager.Instance.GetAnimalByID(model.PreyID).IsUnityNull())
        {
            ExitState();
            return;
        }

        eatTimer = 0f;
        model.Eat(2);
        view.Animator.SetBool(AnimalView.IsEating, true);

        if (model.Hunger >= 100f)
            ExitState();
    }

    /// <summary>
    /// No transitions required while consuming.
    /// </summary>
    public override void CheckSwitchStates() { }

    /// <summary>
    /// This substate does not contain any substates.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Called when exiting the state. Resets flags and determines the next target.
    /// </summary>
    public override void ExitState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        model.ClearPrey();
        model.IsConsuming = false;
        view.Animator.SetBool(AnimalView.IsEating, false);

        // If not in group and still hungry, try setting a new target
        if (!context.animal.InGroup && model.IsHungry)
        {
            Vector3 newTarget = model.GetNextFoodSourcePosition();
            context.animal.SetTarget(newTarget);
        }
    }

    /// <summary>
    /// Returns a description of the state for debug or UI purposes.
    /// </summary>
    public override string ToString() => "Consuming prey";

}
