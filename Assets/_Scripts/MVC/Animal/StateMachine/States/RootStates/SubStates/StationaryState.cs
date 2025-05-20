using UnityEngine;

public class StationaryState : AnimalBaseState
{
    private float stationaryTimer;

    public StationaryState(AnimalStateMachine stateMachine, AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = false;
    }

    public override void EnterState()
    {
        // legrövidebb pihi: 1-3 mp
        stationaryTimer = Random.Range(1f, 3f);
        context.animal.View.StopMovement();
    }

    public override void UpdateState()
    {
        stationaryTimer -= Time.deltaTime;
        CheckSwitchStates();
    }

    public override void CheckSwitchStates()
    {
        if (stationaryTimer <= 0f)
        {
            float roll = UnityEngine.Random.Range(0f, 1f);

            if ((roll >= 1.5f && context.animal.Model.Hunger >= 80) || TimeManager.Instance.GetCurrentTime().Hours > 22) { SwitchState(factory.Sleeping()); return; }
            else { SwitchState(factory.Wandering()); return; }
        }
    }
    public override void ExitState() { return; }
    public override void InitializeSubState() { return; }

    public override string ToString()
    {
        return "Chilling";
    }
}
