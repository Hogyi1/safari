using UnityEngine;
using UnityEngine.AI;

public class SeekWaterState : AnimalBaseState, IRootState
{
    public SeekWaterState(AnimalStateMachine stateMachine,
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
        // Ha éhes lett és nem szomjas már
        else if (!context.animal.Model.IsThirsty && context.animal.Model.IsHungry)
        {
            SwitchState(factory.SeekFood());
        }
        // Ha már nincsen más bajom és breedingelhetek
        else if (context.animal.Model.IsBreeding && !context.animal.Model.IsHungry && !context.animal.Model.IsThirsty && !context.animal.Model.IsConsuming)
        {
            SwitchState(factory.SeekMate());
        }
        // Ha context.animal.Groupban vagyok ha nem az Idle maga intézi
        // Ha nincsen semmi bajom akkor Idle
        else if (!context.animal.Model.IsThirsty && !context.animal.Model.IsConsuming)
        {
            SwitchState(factory.Idle());
        }
    }

    public override void EnterState()
    {
        Vector3 waterPosition = context.animal.Model.GetNextWaterSourcePosition();
        context.animal.SetTarget(waterPosition);

        InitializeSubState();
    }

    public override void ExitState()
    {
        context.animal.Model.SetTarget(Vector3.zero);
    }

    public override void InitializeSubState()
    {
        if (context.animal.Model.Target == Vector3.zero)
            SetSubState(factory.Searching(ColliderTrigger.Water));
        else if (!context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero)
            SetSubState(factory.OnTarget(ColliderTrigger.Water));
        else if (context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero)
            SetSubState(factory.AtTarget(ColliderTrigger.Water));

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
        context.animal.Model.CalculateHunger(1f);
        context.animal.Model.CalculateThirst(1.2f);
    }
}
