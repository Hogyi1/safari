using Unity.VisualScripting;
using UnityEngine;

public class SeekMateState : AnimalBaseState, IRootState
{
    public SeekMateState(AnimalStateMachine stateMachine, AnimalStateFactory factory)
        : base(stateMachine, factory)
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
        if (context.animal.Group != null && context.animal.Group.State != GroupState.Idle)
        {
            SwitchState(factory.Group());
        }
        // Nincsen context.animal.Groupban
        // Ha éhes lett és nem szomjas már
        else if (context.animal.Model.IsHungry)
        {
            SwitchState(factory.SeekFood());
            return;
        }
        // Ha már nincsen más bajom és breedingelhetek
        else if (context.animal.Model.IsThirsty)
        {
            SwitchState(factory.SeekWater());
            return;
        }
        // Ha context.animal.Groupban vagyok ha nem az Idle maga intézi
        // Ha nincsen semmi bajom akkor Idle
        else if (!context.animal.Model.IsBreeding)
        {
            SwitchState(factory.Idle());
        }
    }

    public override void EnterState()
    {
        context.animal.Model.SetTarget(Vector3.zero);
        InitializeSubState();
    }

    public override void ExitState()
    {
        context.animal.Model.SetTarget(Vector3.zero);
    }

    public override void InitializeSubState()
    {
        if (context.animal.Model.Target == Vector3.zero && context.animal.Model.Mate == null)
            SetSubState(factory.Searching(ColliderTrigger.Mate));
        else if (!context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero && context.animal.Model.Mate != null)
            SetSubState(factory.OnTarget(ColliderTrigger.Mate));
        else if (context.animal.View.Arrived && context.animal.Model.Target != Vector3.zero && context.animal.Model.Mate != null)
            SetSubState(factory.AtTarget(ColliderTrigger.Mate));

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
        context.animal.Model.CalculateHunger(0.5f);
        context.animal.Model.CalculateThirst(0.5f);
    }

    public override string ToString()
    {
        return "Searching for mate";
    }
}
