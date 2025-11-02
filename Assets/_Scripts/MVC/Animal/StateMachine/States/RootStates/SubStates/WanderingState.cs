using System;
using UnityEngine;

/// <summary>
/// Represents an idle wandering substate where the animal moves randomly,
/// while remaining alert for food or water sources.
/// </summary>
public class WanderingState : AnimalBaseState
{
    public WanderingState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
    {
        isRootState = false;
    }

    /// <summary>
    /// Called when the wandering state is entered.
    /// Subscribes to detection events and sets a random movement target.
    /// </summary>
    public override void EnterState()
    {
        var view = context.animal.View;
        context.animal.Model.SetTarget(Vector3.zero);
        view.OnFoodSourceFound += HandleFoodSource;
        view.OnWaterSourceFound += HandleWaterSource;
        view.ResetMovement();

        GetNextWanderingTarget();
    }

    /// <summary>
    /// Called once per frame. Checks if conditions are met to transition out of wandering.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    /// <summary>
    /// Evaluates whether the animal should switch to another substate,
    /// such as sleeping or standing still, or continue wandering.
    /// </summary>
    public override void CheckSwitchStates()
    {
        if (!context.animal.View.Arrived)
            return;

        float roll = UnityEngine.Random.Range(0f, 1f);
        float hunger = context.animal.Model.Hunger;
        bool isNight = TimeManager.Instance.IsNight;

        if ((roll <= 0.3f && hunger >= 80f) || isNight)
            SwitchState(factory.Sleeping());
        else if (roll <= 0.5f)
            SwitchState(factory.Stationary());
        else
            GetNextWanderingTarget();

    }

    /// <summary>
    /// Selects and sets a new random destination for the animal to wander toward.
    /// Group members move within their group's territory.
    /// </summary>
    private void GetNextWanderingTarget()
    {
        Vector3 target = GroupManager.Instance
            .GetGroupByID(context.animal.GroupID)?
            .GetRandomPointInCircle()
            ?? context.animal.View.GetRandomTarget();

        context.animal.SetTarget(target);
    }

    /// <summary>
    /// Called when exiting the wandering state.
    /// Unsubscribes from detection events and clears movement data.
    /// </summary>
    public override void ExitState()
    {
        var view = context.animal.View;

        view.OnFoodSourceFound -= HandleFoodSource;
        view.OnWaterSourceFound -= HandleWaterSource;
    }

    /// <summary>
    /// Wandering has no internal substates.
    /// </summary>
    public override void InitializeSubState() { }

    /// <summary>
    /// Handles detection of new water sources and forwards data to the group if applicable.
    /// </summary>
    private void HandleWaterSource(IWaterSource water, Vector3 position)
        => TryRegisterSource(() => context.animal.Model.SaveWaterSource(position));

    /// <summary>
    /// Handles detection of new food sources and forwards data to the group if applicable.
    /// </summary>
    private void HandleFoodSource(IFoodSource food, Vector3 position)
        => TryRegisterSource(() => context.animal.Model.SaveFoodSource(position));

    /// <summary>
    /// Attempts to register a new resource source in the group, if applicable.
    /// </summary>
    private void TryRegisterSource(Func<bool> saveSourceFunc)
    {
        if (!context.animal.InGroup || !saveSourceFunc())
            return;

        GroupManager.Instance
            .GetGroupByID(context.animal.GroupID)?
            .UpdateSources();
    }

    /// <summary>
    /// Returns the name of the current substate.
    /// </summary>
    public override string ToString() => "Wandering";
}
