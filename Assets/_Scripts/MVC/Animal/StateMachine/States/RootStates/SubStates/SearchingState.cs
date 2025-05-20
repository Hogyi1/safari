using System.Data;
using System.Linq;
using UnityEngine;
using static AnimalView;

public class SearchingState : AnimalBaseState
{
    ColliderTrigger[] triggers;
    public SearchingState(AnimalStateMachine stateMachine,
                          AnimalStateFactory factory, params ColliderTrigger[] triggers) : base(stateMachine, factory)
    {
        isRootState = false;
        this.triggers = triggers;
    }

    public override void CheckSwitchStates()
    {
        if (context.animal.View.Arrived && context.animal.Model.Target == Vector3.zero && context.animal.Group == null)
            context.animal.View.GetSetRandomPosition();
        else if (!context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero)
            SwitchState(factory.OnTarget(triggers));
        else if (context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero)
            SwitchState(factory.AtTarget(triggers));
        else if (context.animal.View.Arrived && context.animal.Model.IsBreeding && context.animal.Model.Target == Vector3.zero && context.animal.Group != null)
        {
            context.animal.View.GetSetRandomPosition();
            context.animal.View.RefreshDetection();
        }
    }

    public override void EnterState()
    {
        context.animal.View.ResetMovement();

        foreach (var trigger in triggers)
        {
            switch (trigger)
            {
                case ColliderTrigger.Prey:
                    context.animal.View.OnAnimalFound += HandlePreyFound;
                    break;
                case ColliderTrigger.Mate:
                    context.animal.View.OnAnimalFound += HandleAnimalFound;
                    break;
                case ColliderTrigger.Water:
                    context.animal.View.OnWaterSourceFound += HandleWaterSource;
                    break;
                case ColliderTrigger.Food:
                    context.animal.View.OnFoodSourceFound += HandleFoodSource;
                    break;
                default:
                    break;
            }
        }

        // Keresési fázis a model-ben nincsen target
        context.animal.Model.SetTarget(Vector3.zero);
    }

    // Mindenről leiratkozunk
    public override void ExitState()
    {
        context.animal.View.OnFoodSourceFound -= HandleFoodSource;
        context.animal.View.OnWaterSourceFound -= HandleWaterSource;
        context.animal.View.OnAnimalFound -= HandleAnimalFound;
        context.animal.View.OnAnimalFound -= HandlePreyFound;
    }

    // Nincsen neki substate-e
    public override void InitializeSubState() { }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }


    // Collider event Handler
    // Ha van group szólunk neki, de nem ragálunk rá, ha pedig nincsen csoportunk akkor
    // lereagáljuk és elindulunk felé a ColliderTriggereket visszük tovább
    private void HandleWaterSource(IWaterSource water, Vector3 position)
    {
        bool IsNewSource = false;

        IsNewSource = context.animal.Model.SaveWaterSource(position);

        if (context.animal.Group != null && IsNewSource)
        {
            context.animal.Group.SourceFound(position);
            return;
        }
        else if (IsNewSource)
        {
            context.animal.SetTarget(position);
        }
    }

    private void HandleFoodSource(IFoodSource food, Vector3 position)
    {
        bool IsNewSource = false;

        IsNewSource = context.animal.Model.SaveFoodSource(position);

        if (context.animal.Group != null && IsNewSource)
        {
            context.animal.Group.SourceFound(position);
            return;
        }
        else if (IsNewSource)
        {
            context.animal.SetTarget(position);
        }
    }

    // Találunk egy csoportot vagy csoport nélkülit illetve találunk egy párt
    private void HandleAnimalFound(AnimalView mateView)
    {
        Animal animal = context.GetAnimal(mateView.ID);

        if (animal != null && animal.Model.Type == context.animal.Model.Type)
        {
            if (animal.Group != null && context.animal.Group == null)
            {
                context.JoinGroup(animal.Group);
            }
            else if (animal.Model.CanBreed && context.animal.Model.IsBreeding && context.animal.Model.Mate != animal.Model)
            {
                context.animal.Model.SetMate(animal.Model);
                context.animal.Model.SetTarget(mateView.transform.position);
                context.animal.View.Follow(mateView);
            }
        }
    }

    private void HandlePreyFound(AnimalView preyView)
    {
        Animal animal = context.GetAnimal(preyView.ID);

        if (animal.Model.Diet != context.animal.Model.Diet)
        {
            if (context.animal.Group != null)
            {
                context.animal.Group.PreyFound(preyView);
            }
            else
            {
                context.animal.Model.SetTarget(preyView.transform.position);
                context.animal.View.Follow(preyView);
            }

            context.animal.Model.SetPrey(preyView.Model);
        }
    }

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