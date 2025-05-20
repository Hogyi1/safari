using System;
using UnityEngine;

public class EatingState : AnimalBaseState
{
    IFoodSource foodSource;
    float eatTimer;
    const float EatingInterval = 2f;
    public EatingState(AnimalStateMachine stateMachine,
                         AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = false;
    }

    public override void EnterState()
    {
        var structObj = context.GetStructure(context.animal.Model.Target);
        if (!(structObj is IFoodSource fs))
        {
            ExitState();
            return;
        }

        foodSource = fs;
        context.animal.Model.IsConsuming = true;
        eatTimer = EatingInterval;
    }


    public override void UpdateState()
    {
        if (!context.animal.Model.IsConsuming || foodSource == null)
            return;

        eatTimer += Time.deltaTime;
        if (eatTimer < EatingInterval)
            return;

        if (!(context.GetStructure(context.animal.Model.Target) is IFoodSource fs))
        {
            FinishConsumption();
            context.animal.Model.RemoveSource(context.animal.Model.Target);
            return;
        }

        foodSource = fs;
        // 2 másodperc eltelt, iszunk egyszer
        eatTimer = 0f;
        int eaten = foodSource.Consume(1);
        context.animal.Model.Eat(eaten);
        context.animal.View.Animator.SetBool(AnimalView.IsEating, true);

        // ha elfogyott a vízforrás vagy már nem szomjas, befejezzük
        if (eaten == 0 || context.animal.Model.Hunger >= 100f)
            FinishConsumption();
    }

    // Nincs se alstate se szomszéd state-je
    public override void CheckSwitchStates() { return; }
    public override void InitializeSubState() { return; }

    public override void ExitState()
    {
        // Nem volt itt semmilyen kaja, ezért töröljük
        context.animal.Model.RemoveSource(context.animal.Model.Target);
        if (context.animal.Group == null)
        {
            // Lekérjük a következő targetet, ha vector3.zero akkor úgyis keresni indul, ha nem akkor meg OnTargetre vált
            Vector3 otherTarget = context.animal.Model.GetNextFoodSourcePosition();
            // Beállítjuk a targetet
            context.animal.Model.SetTarget(otherTarget);
        }
        context.animal.View.Animator.SetBool(AnimalView.IsEating, false);
    }

    private void FinishConsumption()
    {
        if (context.animal.Model.IsHungry && context.animal.Group == null)
        {
            Vector3 nextTarget = context.animal.Model.GetNextFoodSourcePosition();
            if (nextTarget == context.animal.Model.Target)
            {
                context.animal.Model.RemoveSource(nextTarget);
                context.animal.Model.SetTarget(Vector3.zero);
            }
            else
            {
                // Kitörlöm ami most üres inkább keresek mást
                context.animal.Model.RemoveSource(context.animal.Model.Target);
                context.animal.SetTarget(nextTarget);
            }
        }
        else if (!context.animal.Model.IsHungry && context.animal.Group == null)
        {
            context.animal.Model.SetTarget(Vector3.zero);
        }

        context.animal.Model.IsConsuming = false;
        context.animal.View.Animator.SetBool(AnimalView.IsEating, false);
    }

    public override string ToString()
    {
        return "Eating";
    }
}


