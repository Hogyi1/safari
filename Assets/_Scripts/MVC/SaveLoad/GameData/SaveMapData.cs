using UnityEngine;

[System.Serializable]
public class SaveMapData
{
    public int uniqid;
    public int buildid;
    public Vector3 pos;


    public SaveMapData(int uid , int buildid, Vector3 v)
    {
        uniqid = uid;
        this.buildid = buildid;
        pos = v;
    }


}
