using System;
using Unity.VisualScripting;
using UnityEngine;

public class HuntingState : AnimalBaseState
{
    Animal prey;
    float eatTimer;
    const float EatingInterval = 2f;
    public HuntingState(AnimalStateMachine stateMachine,
                         AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = false;
    }

    public override void EnterState()
    {
        prey = context.GetAnimal(context.animal.Model.Prey.ID);

        if (prey == null)
        {
            ExitState();
            return;
        }

        // Kinyírjuk az állatot
        context.KillAnimal(prey);
        context.animal.Model.IsConsuming = true;
        eatTimer = EatingInterval;
    }


    public override void UpdateState()
    {
        if (!context.animal.Model.IsConsuming || prey == null)
            return;

        eatTimer += Time.deltaTime;
        if (eatTimer < EatingInterval)
            return;

        if (context.GetAnimal(context.animal.Model.Prey.ID).IsUnityNull())
        {
            FinishConsumption();
            return;
        }

        // 2 másodperc eltelt, iszunk egyszer
        eatTimer = 0f;
        context.animal.Model.Eat(2);
        context.animal.View.Animator.SetBool(AnimalView.IsEating, true);

        // ha elfogyott a vízforrás vagy már nem szomjas, befejezzük
        if (context.animal.Model.Hunger >= 100f)
            FinishConsumption();
    }

    // Nincs se alstate se szomszéd state-je
    public override void CheckSwitchStates() { return; }
    public override void InitializeSubState() { return; }

    public override void ExitState()
    {
        if (context.animal.Group == null)
        {
            // Lekérjük a következő targetet, ha vector3.zero akkor úgyis keresni indul, ha nem akkor meg OnTargetre vált
            Vector3 otherTarget = context.animal.Model.GetNextFoodSourcePosition();
            // Nincsen már semmilyen préda akit követünk
            context.animal.Model.ClearPrey();
            // Beállítjuk a targetet
            context.animal.Model.SetTarget(otherTarget);
        }

        context.animal.Model.IsConsuming = false;
        context.animal.View.Animator.SetBool(AnimalView.IsEating, false);
    }

    private void FinishConsumption()
    {
        if (context.animal.Model.IsHungry && context.animal.Group == null)
        {
            Vector3 nextTarget = context.animal.Model.GetNextFoodSourcePosition();
            context.animal.Model.ClearPrey();
            context.animal.SetTarget(nextTarget);
        }

        context.animal.Model.IsConsuming = false;
        context.animal.View.Animator.SetBool(AnimalView.IsEating, false);
    }

    public override string ToString()
    {
        return "Eating prey";
    }
}
