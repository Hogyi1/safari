using UnityEngine;

public class SeekFoodState : AnimalBaseState, IRootState
{
    public SeekFoodState(AnimalStateMachine stateMachine,
                         AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = true;

    }

    public override void CheckSwitchStates()
    {
        // Priority #1
        // Ha halott akkor vége van
        if (context.animal.Model.IsDead) { SwitchState(factory.Dead()); return; }

        // Priority #2
        // Ha csoportban és van valamilyen activity
        if (context.animal.Group != null)
        {
            if (context.animal.Group.State != GroupState.Idle)
            {
                SwitchState(factory.Group());
            }
        }
        // Nincsen context.animal.Groupban
        // Ha szomjas lett és nem éhes már
        else if (context.animal.Model.IsThirsty && !context.animal.Model.IsHungry)
        {
            SwitchState(factory.SeekWater());
        }
        // Ha már nincsen más bajom és breedingelhetek
        else if (context.animal.Model.IsBreeding && !context.animal.Model.IsHungry && !context.animal.Model.IsThirsty && !context.animal.Model.IsConsuming)
        {
            SwitchState(factory.SeekMate());
        }
        // Ha context.animal.Groupban vagyok ha nem az Idle maga intézi
        // Ha nincsen semmi bajom akkor Idle
        else if (!context.animal.Model.IsHungry && !context.animal.Model.IsConsuming)
        {
            SwitchState(factory.Idle());
        }
    }

    public override void EnterState()
    {
        Vector3 foodPos = context.animal.Model.GetNextFoodSourcePosition();
        context.animal.SetTarget(foodPos);

        InitializeSubState();
    }

    public override void ExitState()
    {
        context.animal.Model.SetTarget(Vector3.zero);
    }

    public override void InitializeSubState()
    {
        ColliderTrigger[] triggers;
        if (context.animal.Model.Diet == DietType.Carnivore) triggers = new ColliderTrigger[] { ColliderTrigger.Prey, ColliderTrigger.Food };
        else triggers = new ColliderTrigger[] { ColliderTrigger.Food };

        if (context.animal.Model.Target == Vector3.zero)
            SetSubState(factory.Searching(triggers));
        else if (!context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero)
            SetSubState(factory.OnTarget(triggers));
        else if (context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero)
            SetSubState(factory.AtTarget(triggers));

        // Minden keresésnél nézzen újra körbe
        context.animal.View.RefreshDetection();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        CalculateModelData();
    }

    public void CalculateModelData()
    {
        context.animal.Model.CalculateHp();
        context.animal.Model.CalculateHunger(1.2f);
        context.animal.Model.CalculateThirst(1f);
    }

    public override string ToString()
    {
        return "Hungry";
    }
}
