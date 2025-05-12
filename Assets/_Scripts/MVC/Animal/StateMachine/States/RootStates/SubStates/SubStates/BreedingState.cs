using System;
using Unity.VisualScripting;
using UnityEngine;

public class BreedingState : AnimalBaseState
{
    Animal mate;
    float timer;
    const float cooldown = 2f;
    public BreedingState(AnimalStateMachine stateMachine,
                         AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = false;
    }

    public override void EnterState()
    {
        mate = context.GetAnimal(context.animal.Model.Mate.ID);

        if (mate == null)
        {
            ExitState();
            return;
        }

        timer = cooldown;
    }

    public override void UpdateState()
    {
        if (!context.animal.Model.IsBreeding || mate == null)
            return;

        timer += Time.deltaTime;
        if (timer < cooldown)
            return;

        if (context.GetAnimal(context.animal.Model.Mate.ID).IsUnityNull())
        {
            context.animal.Model.SetTarget(Vector3.zero);
            return;
        }
        else
            mate.View.StopMovement();

        timer = 0f;
        if (mate.Model.TryBreeding(context.animal.Model))
        {
            context.Breeding(mate);
            ExitState();
        }
        else
        {
            mate.View.ResetMovement();
            context.animal.Model.SetTarget(Vector3.zero);
        }
    }

    // Nincs se alstate se szomszéd state-je
    public override void CheckSwitchStates() { return; }
    public override void InitializeSubState() { return; }

    public override void ExitState()
    {
        mate.View.ResetMovement();
        context.animal.Model.StopBreeding();
        context.animal.Model.SetTarget(Vector3.zero);
    }
}
