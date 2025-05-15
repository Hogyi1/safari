using UnityEngine;

public class WanderingState : AnimalBaseState
{
    public WanderingState(AnimalStateMachine stateMachine, AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = false;
    }

    public override void EnterState()
    {
        context.animal.View.OnFoodSourceFound += HandleFoodSource;
        context.animal.View.OnWaterSourceFound += HandleWaterSource;
        context.animal.View.ResetMovement();

        GetRandomTarget();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void CheckSwitchStates()
    {
        if (context.animal.View.Arrived)
        {
            float roll = Random.Range(0f, 1f);

            if ((roll <= 0.3f && context.animal.Model.Hunger >= 80) || TimeManager.Instance.GetCurrentTime().hours > 22) { SwitchState(factory.Sleeping()); return; }
            if (roll <= 0.5f) { SwitchState(factory.Stationary()); return; }
            else GetRandomTarget();
        }
    }

    private void GetRandomTarget()
    {
        // Random célpont beállítása
        Vector3 targetPos;
        if (context.animal.Group != null)
        {
            targetPos = context.animal.Group.GetRandomPointInCircle();
            context.animal.View.SetTarget(targetPos);
        }
        else
            targetPos = context.animal.View.GetSetRandomPosition();

        context.animal.Model.SetTarget(targetPos);
    }

    public override void ExitState()
    {
        context.animal.View.OnFoodSourceFound -= HandleFoodSource;
        context.animal.View.OnWaterSourceFound -= HandleWaterSource;
        context.animal.Model.SetTarget(Vector3.zero);
        context.animal.View.StopMovement();
    }

    public override void InitializeSubState() { return; }

    private void HandleWaterSource(IWaterSource water, Vector3 position)
    {
        bool IsNewSource = context.animal.Model.SaveWaterSource(position);

        if (context.animal.Group != null && IsNewSource)
        {
            context.animal.Group.UpdateSources();
            return;
        }
    }

    private void HandleFoodSource(IFoodSource food, Vector3 position)
    {
        bool IsNewSource = context.animal.Model.SaveFoodSource(position);

        if (context.animal.Group != null && IsNewSource)
        {
            context.animal.Group.UpdateSources();
            return;
        }
    }

    public override string ToString()
    {
        return "Wandering";
    }
}