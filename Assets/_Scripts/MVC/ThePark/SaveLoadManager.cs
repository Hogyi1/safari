using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{

    public bool isSaved;
    public string saveFile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public bool Save() { 
        return false;
    }
    public bool Load(string saveFile) {
        return false;
    }
}
