using UnityEngine;

public class SleepingState : AnimalBaseState
{
    private float sleepTimer;

    public SleepingState(AnimalStateMachine stateMachine, AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = false;
    }

    public override void EnterState()
    {
        context.animal.View.Animator.SetBool(AnimalView.IsSleeping, true);

        sleepTimer = Random.Range(10f, 30f);
        context.animal.View.StopMovement();
    }

    public override void UpdateState()
    {
        sleepTimer -= Time.deltaTime;
        CheckSwitchStates();
    }

    public override void CheckSwitchStates()
    {
        // Ha felkelt akkor egy kicsit kómás úgyhogy stationary
        if (sleepTimer <= 0f && TimeManager.Instance.GetCurrentTime().hours < 22 && TimeManager.Instance.GetCurrentTime().hours > 5)
        {
            SwitchState(factory.Stationary());
        }
    }
    public override void ExitState() { context.animal.View.Animator.SetBool(AnimalView.IsSleeping, false); }
    public override void InitializeSubState() { return; }
}
