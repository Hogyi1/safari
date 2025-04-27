using UnityEngine;

[System.Serializable]

//abstract
public class GameData 
{
    public int exempleint;
    public SerializableDictionary<int, int> exemple;
    public TutorialFile TutorialFile;

    public GameData()
    {
        //This is where you need to add the data, according to how you want your class to be initialized
        this.exempleint = 0;
        this.exemple = new SerializableDictionary<int, int>();
        this.TutorialFile = new TutorialFile();

    }
}
