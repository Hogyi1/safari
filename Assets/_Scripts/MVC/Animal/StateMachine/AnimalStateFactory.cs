
// Egyszerű factory, hogy ne kelljen mindig new State-t kiírni és megkapjon minden adatot
public class AnimalStateFactory
{
    AnimalStateMachine context;

    public AnimalStateFactory(AnimalStateMachine context)
    {
        this.context = context;
    }

    // Root
    public AnimalBaseState Idle() => new IdleState(context, this);
    public AnimalBaseState SeekFood() => new SeekFoodState(context, this);
    public AnimalBaseState SeekWater() => new SeekWaterState(context, this);
    public AnimalBaseState SeekMate() => new SeekMateState(context, this);
    public AnimalBaseState Dead() => new DeadState(context, this);
    public AnimalBaseState Group() => new GroupBehaviourState(context, this);

    // Sub
    public AnimalBaseState OnTarget(params ColliderTrigger[] triggers) => new OnTargetState(context, this, triggers);
    public AnimalBaseState AtTarget(params ColliderTrigger[] triggers) => new AtTargetState(context, this, triggers);
    public AnimalBaseState Searching(params ColliderTrigger[] triggers) => new SearchingState(context, this, triggers);
    public AnimalBaseState Wandering() => new WanderingState(context, this);
    public AnimalBaseState Sleeping() => new SleepingState(context, this);
    public AnimalBaseState Stationary() => new StationaryState(context, this);
    public AnimalBaseState GroupIdle() => new GroupIdleState(context, this);

    // Sub -> Sub
    public AnimalBaseState Breed() => new BreedingState(context, this);
    public AnimalBaseState Drink() => new DrinkingState(context, this);
    public AnimalBaseState Eat() => new EatingState(context, this);
    public AnimalBaseState Hunt() => new HuntingState(context, this);
}
