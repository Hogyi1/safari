using UnityEngine;

public class GroupBehaviourState : AnimalBaseState, IRootState
{
    public GroupBehaviourState(AnimalStateMachine stateMachine,
                              AnimalStateFactory factory) : base(stateMachine, factory)
    {
        isRootState = true;
    }

    public override void EnterState()
    {
        HandleStateChanged(context.animal.Group);
    }

    // Ha a csoportnak van valami gondja itt kezeljük
    private void HandleStateChanged(Group g)
    {
        ColliderTrigger[] triggers = context.animal.Model.Diet == DietType.Carnivore ? new ColliderTrigger[] { ColliderTrigger.Food, ColliderTrigger.Prey } : new ColliderTrigger[] { ColliderTrigger.Food };

        switch (g.State)
        {
            case GroupState.Idle:
                SwitchState(factory.Idle());
                break;
            case GroupState.Hungry:
                SetSubState(factory.OnTarget(triggers));
                break;
            case GroupState.Thirsty:
                SetSubState(factory.OnTarget(ColliderTrigger.Water));
                break;
            case GroupState.SearchingFood:
                SetSubState(factory.Searching(triggers));
                break;
            case GroupState.SearchingWater:
                SetSubState(factory.Searching(ColliderTrigger.Water));
                break;
        }

        // Minden keresésnél nézzen újra körbe
        context.animal.View.RefreshDetection();
    }

    public override void ExitState()
    {
        if (context.animal.Group != null)
            context.animal.Group.OnStateChanged -= HandleStateChanged;
        context.animal.Model.SetTarget(Vector3.zero);
    }

    public override void CheckSwitchStates()
    {
        // Priority #1
        if (context.animal.Model.IsDead) { SwitchState(factory.Dead()); return; }

        // Ha kiléptünk akkor visszaváltunk Idle-be onnan, majd kezeli saját magát
        if (context.animal.Group == null) { SwitchState(factory.Idle()); return; }
    }

    public override void InitializeSubState()
    {
        return; // HandleStage intézi
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        CalculateModelData();
    }

    public void CalculateModelData()
    {
        context.animal.Model.CalculateHp();
        context.animal.Model.CalculateHunger(0.8f);
        context.animal.Model.CalculateThirst(0.8f);
    }

    public override string ToString()
    {
        if (currentSubState != null)
            return currentSubState?.ToString();
        else return "In group";
    }
}
