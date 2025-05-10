using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : AnimalBaseState, IRootState
{
    public IdleState(AnimalStateMachine stateMachine,
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
        if (context.animal.Group != null && context.animal.Group.State != GroupState.Idle)
        {
            SwitchState(factory.Group());
            return;
        }
        // Nincsen groupban
        // Ha éhes lett és nem szomjas már
        else if (context.animal.Model.IsHungry)
        {
            SwitchState(factory.SeekFood());
        }
        // Ha groupban vagyok ha nem az Idle maga intézi
        // Ha nincsen semmi bajom akkor Idle
        else if (context.animal.Model.IsThirsty)
        {
            SwitchState(factory.SeekWater());
        }
        // Ha már nincsen más bajom és breedingelhetek
        else if (context.animal.Model.IsBreeding && !context.animal.Model.IsHungry && !context.animal.Model.IsThirsty)
        {
            SwitchState(factory.SeekMate());
        }
    }

    public override void EnterState()
    {
        context.animal.View.RefreshDetection();
        context.animal.View.OnAnimalFound += HandleAnimalFound;
        InitializeSubState();
    }

    public override void ExitState()
    {
        context.animal.View.OnAnimalFound -= HandleAnimalFound;
    }

    public override void InitializeSubState()
    {
        // Random választunk egy SubStatet
        // Azt, hogy mennyi ideig tartson azt, maga a State fogja eldönteni
        float roll = UnityEngine.Random.Range(0f, 1f);

        if (roll <= 0.1f || TimeManager.Instance.GetCurrentTime().hours > 22) { SetSubState(factory.Sleeping()); return; }
        if (roll <= 0.5f) { SetSubState(factory.Stationary()); return; }
        else SetSubState(factory.Wandering());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        CalculateModelData();
    }

    // A jelenlegi aktivítástól függően beállítjuk az éhség és szomjúság mértékét
    public void CalculateModelData()
    {
        context.animal.Model.CalculateHp();

        switch (currentSubState)
        {
            case SleepingState sleepingState:
                context.animal.Model.CalculateHunger(0.2f);
                context.animal.Model.CalculateThirst(0.2f);
                break;
            case StationaryState stationaryState:
                context.animal.Model.CalculateHunger(0.5f);
                context.animal.Model.CalculateThirst(0.5f);
                break;
            case WanderingState wanderingState:
                context.animal.Model.CalculateHunger(0.8f);
                context.animal.Model.CalculateThirst(0.8f);
                break;
        }
    }

    // Ha a View talál egy másik állatot, csoport kezelés
    private void HandleAnimalFound(AnimalView view)
    {
        if (view.ID == context.animal.Model.ID) return;
        Animal animal = context.GetAnimal(view.ID);
        if (animal.Group == null && context.animal.Group == null && animal.Model.Type == context.animal.Model.Type && !animal.Model.IsDead)
        {
            GroupManager.Instance.CreateNewGroup(new List<Animal> { animal, context.animal });
        }
        else if (animal.Group != null && context.animal.Group == null && animal.Model.Type == context.animal.Model.Type && !animal.Model.IsDead)
        {
            GroupManager.Instance.EnterGroup(animal.Group, context.animal);
        }
    }
}
