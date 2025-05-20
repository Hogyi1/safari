using UnityEngine;
using static RangerState;

/// <summary>
/// Represents the internal model/state of a Ranger unit.
/// Handles energy, prey tracking, and state transitions (e.g., Available, Resting).
/// </summary>
public class RangerModel
{
    /// <summary>
    /// Unique identifier for this ranger.
    /// </summary>
    public int ID;

    /// <summary>
    /// The current state of the ranger (e.g., Available, Resting).
    /// </summary>
    public RangerState State;

    /// <summary>
    /// Current energy level of the ranger (0–100).
    /// </summary>
    private float energy = 100f;

    /// <summary>
    /// Data reference containing ranger configuration values.
    /// </summary>
    private RangerData data;

    /// <summary>
    /// ID of the currently tracked prey, or -1 if none.
    /// </summary>
    private int preyID = -1;

    /// <summary>
    /// The threshold below which the ranger is considered tired.
    /// </summary>
    private const float fatigueThreshold = 40f;

    /// <summary>
    /// The maximum possible energy level.
    /// </summary>
    private const float maxEnergy = 100f;

    /// <summary>
    /// Constructs a new ranger model with the specified ID and data.
    /// </summary>
    /// <param name="id">Unique ranger ID.</param>
    /// <param name="data">Reference to the ranger’s data configuration.</param>
    public RangerModel(int id, RangerData data)
    {
        ID = id;
        State = Available;
        this.data = data;
    }

    /// <summary>
    /// Returns true if the ranger is below the fatigue threshold.
    /// </summary>
    public bool IsTired => energy < fatigueThreshold;

    /// <summary>
    /// Assigns a prey target by ID.
    /// </summary>
    public void SetPrey(int preyID) => this.preyID = preyID;

    /// <summary>
    /// Clears the currently tracked prey and returns its ID.
    /// </summary>
    /// <returns>The cleared prey ID, or -1 if none.</returns>
    public int ClearPrey()
    {
        int toReturn = preyID;
        preyID = -1;
        return toReturn;
    }

    /// <summary>
    /// Reduces energy based on elapsed time.
    /// </summary>
    /// <param name="deltaTime">Time delta to apply (usually Time.deltaTime).</param>
    public void DecreaseEnergy(float deltaTime)
    {
        energy = Mathf.Max(0f, energy - 10f * deltaTime);
    }

    /// <summary>
    /// Regenerates energy if the ranger is in the Resting state.
    /// </summary>
    /// <param name="deltaTime">Time delta to apply (usually Time.deltaTime).</param>
    public void IncreaseEnergy(float deltaTime)
    {
        if (State == Resting)
        {
            energy = Mathf.Min(maxEnergy, energy + 15f * deltaTime);
        }
    }

    /// <summary>
    /// Updates the ranger’s state based on current energy.
    /// Switches between Available and Resting.
    /// </summary>
    public void CheckState()
    {
        if (State == Available || State == Resting)
        {
            State = IsTired ? Resting : Available;
        }
    }
}
