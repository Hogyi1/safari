using System;
using UnityEngine;

public class DrinkingState : AnimalBaseState
{
    IWaterSource waterSource;
    float drinkTimer;
    const float DrinkInterval = 1f;
    public DrinkingState(AnimalStateMachine stateMachine,
                         AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = false;
    }

    public override void EnterState()
    {
        var structObj = context.GetStructure(context.animal.Model.Target);
        if (!(structObj is IWaterSource ws))
        {
            ExitState();
            return;
        }

        waterSource = ws;
        context.animal.Model.IsConsuming = true;
        drinkTimer = DrinkInterval;
    }


    public override void UpdateState()
    {
        if (!context.animal.Model.IsConsuming || waterSource == null)
            return;

        drinkTimer += Time.deltaTime;
        if (drinkTimer < DrinkInterval)
            return;

        if (!(context.GetStructure(context.animal.Model.Target) is IWaterSource ws))
        {
            FinishConsumption();
            context.animal.Model.RemoveSource(context.animal.Model.Target);
            return;
        }

        // 2 másodperc eltelt, iszunk egyszer
        drinkTimer = 0f;
        int drank = waterSource.Consume(1);
        context.animal.Model.Drink(drank);
        context.animal.View.Animator.SetBool(AnimalView.IsDrinking, true);

        // ha elfogyott a vízforrás vagy már nem szomjas, befejezzük
        if (drank == 0 || context.animal.Model.Thirst >= 100f)
            FinishConsumption();
    }

    // Nincs se alstate se szomszéd state-je
    public override void CheckSwitchStates() { return; }
    public override void InitializeSubState() { return; }

    public override void ExitState()
    {
        // Nem volt itt semmilyen víz, ezért töröljük
        context.animal.Model.RemoveSource(context.animal.Model.Target);
        if (context.animal.Group == null)
        {
            // Lekérjük a következő targetet, ha vector3.zero akkor úgyis keresni indul, ha nem akkor meg OnTargetre vált
            Vector3 otherTarget = context.animal.Model.GetNextWaterSourcePosition();
            // Beállítjuk a targetet
            context.animal.Model.SetTarget(otherTarget);
        }
        context.animal.Model.IsConsuming = false;
        context.animal.View.Animator.SetBool(AnimalView.IsDrinking, false);
    }

    private void FinishConsumption()
    {
        if (context.animal.Model.IsThirsty && context.animal.Group == null)
        {
            Vector3 nextTarget = context.animal.Model.GetNextWaterSourcePosition();
            if (nextTarget == context.animal.Model.Target)
            {
                context.animal.Model.RemoveSource(nextTarget);
                context.animal.Model.SetTarget(Vector3.zero);
            }
            else
            {
                context.animal.Model.RemoveSource(context.animal.Model.Target);
                context.animal.SetTarget(nextTarget);
            }
        }
        else if (!context.animal.Model.IsThirsty && context.animal.Group == null)
        {
            context.animal.Model.SetTarget(Vector3.zero);
        }

        context.animal.Model.IsConsuming = false;
        context.animal.View.Animator.SetBool(AnimalView.IsDrinking, false);
    }
}
