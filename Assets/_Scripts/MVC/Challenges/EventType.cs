/// <summary>
/// Enumerates the types of events that can trigger events.
/// </summary>
public enum EventType
{
    /// <summary>An animal has been purchased.</summary>
    ANIMAL_BUY,
    /// <summary>An animal has been placed.</summary>
    ANIMAL_PLACE,
    /// <summary>An animal has been killed.</summary>
    ANIMAL_KILL,
    /// <summary>An animal ate something.</summary>
    ANIMAL_FED,
    /// <summary>An animal gave birth.</summary>
    ANIMAL_BORN,
    /// <summary>The current animal count.</summary>
    ANIMALS_OWNED,
    /// <summary>A tree has been placed.</summary>
    TREE_PLACE,
    /// <summary>A building has been placed.</summary>
    BUILDING_PLACE,
    /// <summary>A building has been upgraded.</summary>
    BUILDING_UPGRADE,
    /// <summary>A feeder/water dispenser has been placed.</summary>
    FEEDER_PLACE,
    /// <summary>A jeep has been purchased.</summary>
    JEEP_BUY,
    /// <summary>Money has been gained.</summary>
    MONEY_GAIN,
    /// <summary>Experience points have been added.</summary>
    EXP_ADD,
    /// <summary>Visitor has been transported.</summary>
    VISITOR_TRANSPORTED,
    /// <summary>Visitor happiness is high.</summary>
    VISITOR_HAPPINESS_HIGH,
    /// <summary>Player leveled up.</summary>
    LEVEL_UP,
    /// <summary>All challenges have been completed.</summary>
    ALL_CHALLENGES_COMPLETED,
    /// <summary>Experience points have been gained.</summary>
    EXP_GAINED
    // Add more if we need to
}
