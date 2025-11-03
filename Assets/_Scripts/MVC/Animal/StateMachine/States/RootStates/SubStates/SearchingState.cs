using System.Linq;
using UnityEngine;

/// <summary>
/// State in which the animal searches for a specific type of resource or entity (e.g., food, water, mate, or prey).
/// It listens for environmental triggers and transitions accordingly based on detection results.
/// </summary>
public class SearchingState : AnimalBaseState
{
    private readonly ColliderTrigger[] triggers;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchingState"/> class with the desired triggers to detect.
    /// </summary>
    public SearchingState(AnimalStateMachine stateMachine, AnimalStateFactory factory, params ColliderTrigger[] triggers)
        : base(stateMachine, factory)
    {
        isRootState = false;
        this.triggers = triggers;
    }

    /// <summary>
    /// Called once when the state is entered. Subscribes to relevant detection events
    /// and resets movement to begin searching.
    /// </summary>
    public override void EnterState()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        view.ResetMovement();

        // Subscribe to relevant detection events
        foreach (var trigger in triggers)
        {
            switch (trigger)
            {
                case ColliderTrigger.Prey:
                    view.OnAnimalFound += HandlePreyFound;
                    break;
                case ColliderTrigger.Mate:
                    view.OnAnimalFound += HandleMateFound;
                    break;
                case ColliderTrigger.Water:
                    view.OnWaterSourceFound += HandleWaterSource;
                    break;
                case ColliderTrigger.Food:
                    view.OnFoodSourceFound += HandleFoodSource;
                    break;
            }
        }

        view.RefreshDetection();
    }

    /// <summary>
    /// Called every frame to evaluate transitions to new states
    /// based on current search conditions and progress.
    /// </summary>
    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    /// <summary>
    /// Evaluates if the animal should transition from searching to another state
    /// such as OnTarget, AtTarget, or continue random movement.
    /// </summary>
    public override void CheckSwitchStates()
    {
        var model = context.animal.Model;
        var view = context.animal.View;

        if (view.Arrived && model.Target == Vector3.zero && (!context.animal.InGroup || model.IsBreeding))
            view.GetSetRandomTarget();

        else if (!view.Arrived && model.Target != Vector3.zero)
            SwitchState(factory.OnTarget(triggers));

        else if (view.Arrived && model.Target != Vector3.zero)
            SwitchState(factory.AtTarget(triggers));

    }

    /// <summary>
    /// Called once when exiting the state. Unsubscribes from all detection events.
    /// </summary>
    public override void ExitState()
    {
        var view = context.animal.View;

        view.OnFoodSourceFound -= HandleFoodSource;
        view.OnWaterSourceFound -= HandleWaterSource;
        view.OnAnimalFound -= HandleMateFound;
        view.OnAnimalFound -= HandlePreyFound;
    }

    public override void InitializeSubState() { }

#warning Lehet egyszerusiteni
    /// <summary>
    /// Handles detection of a new water source. If the animal is in a group,
    /// the group is notified. Otherwise, the animal sets the water as a new target.
    /// </summary>
    private void HandleWaterSource(IWaterSource water, Vector3 position)
    {
        var model = context.animal.Model;

        if (!model.SaveWaterSource(position))
            return;

        if (context.animal.InGroup)
            GroupManager.Instance.GetGroupByID(context.animal.GroupID)?.SourceFound(position);

        else
            context.animal.SetTarget(position);
    }

    /// <summary>
    /// Handles detection of a new food source. If the animal is in a group,
    /// the group is notified. Otherwise, the animal sets the food as a new target.
    /// </summary>
    private void HandleFoodSource(IFoodSource food, Vector3 position)
    {
        var model = context.animal.Model;

        if (!model.SaveFoodSource(position))
            return;

        if (context.animal.InGroup)
            GroupManager.Instance.GetGroupByID(context.animal.GroupID)?.SourceFound(position);

        else
            context.animal.SetTarget(position);

    }

    /// <summary>
    /// Handles detection of a potential mate. If conditions are favorable (e.g., same type, same group),
    /// the animal begins to follow the mate.
    /// </summary>
    private void HandleMateFound(int mateID)
    {
        Animal self = context.animal;
        Animal other = AnimalManager.Instance.GetAnimalByID(mateID);

        if (other == null || other.Model.IsDead || self.Model.IsDead)
            return;

        if (other.Model.Type != self.Model.Type || !other.Model.CanBreed)
            return;

        if (other.GroupID == self.GroupID)
        {
            self.Model.SetMate(other.ID);
            self.Model.SetTarget(other.View.transform.position);
            self.View.Follow(other.View);
        }
    }

    /// <summary>
    /// Handles detection of a prey animal. If in a group, notifies the group.
    /// Otherwise, sets the prey as a new target and starts following.
    /// </summary>
    private void HandlePreyFound(int preyID)
    {
        Animal other = AnimalManager.Instance.GetAnimalByID(preyID);
        if (other == null || other.Model.Diet == context.animal.Model.Diet)
            return;

        Debug.Log("Other animal found");

        if (context.animal.InGroup)
            GroupManager.Instance.GetGroupByID(context.animal.GroupID)?.PreyFound(preyID);

        else
        {
            context.animal.Model.SetTarget(other.View.transform.position);
            context.animal.Model.SetPrey(preyID);
            context.animal.View.Follow(other.View);
        }
    }

    /// <summary>
    /// Returns a readable description of the current search intent.
    /// </summary>
    public override string ToString()
    {
        if (triggers.Contains(ColliderTrigger.Water))
            return "Searching for water";
        if (triggers.Contains(ColliderTrigger.Mate))
            return "Searching for mate";
        if (triggers.Contains(ColliderTrigger.Prey))
            return "Searching for prey";
        if (triggers.Contains(ColliderTrigger.Food))
            return "Searching for food";

        return "Searching for something";
    }
}
