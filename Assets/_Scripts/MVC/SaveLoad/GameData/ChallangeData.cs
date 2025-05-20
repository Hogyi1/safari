using UnityEngine;

[System.Serializable]
public class ChallangeData
{
    public int id;
    public ChallengeState state;
    public float progress;

    public ChallangeData(int id, ChallengeState State, float prog) { 
        this.id = id;
        this.state = State; 
        this.progress = prog;
    }
}
