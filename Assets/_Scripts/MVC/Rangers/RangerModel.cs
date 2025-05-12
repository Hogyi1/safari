using UnityEngine;
using static RangerState;

public class RangerModel
{
    public int ID;
    public RangerState State;
    private float energy = 100f;
    private RangerData data;
    private int preyID = -1;

    private const float fatigueThreshold = 40f;
    private const float maxEnergy = 100f;

    public RangerModel(int id, RangerData data)
    {
        ID = id;
        State = Available;
        this.data = data;
    }

    public bool IsTired => energy < fatigueThreshold;
    public void SetPrey(int preyID) => this.preyID = preyID;
    public int ClearPrey()
    {
        int toReturn = preyID;
        preyID = -1;
        return toReturn;
    }

    public void DecreaseEnergy(float deltaTime)
    {
        energy = Mathf.Max(0f, energy - 10f * deltaTime);
    }

    public void IncreaseEnergy(float deltaTime)
    {
        if (State == Resting)
        {
            energy = Mathf.Min(maxEnergy, energy + 15f * deltaTime);
        }
    }

    public void CheckState()
    {
        if (State == Available || State == Resting)
        {
            State = IsTired ? Resting : Available;
        }
    }
}