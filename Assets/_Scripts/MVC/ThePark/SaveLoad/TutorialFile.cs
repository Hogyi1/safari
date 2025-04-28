using System;
using UnityEngine;


//First, we inherit from the class into an IDataPersistence
[Serializable]
public class TutorialFile : IDataPersistence
{
    public int exempleInt;
    public SerializableDictionary<int,int> exemple = new SerializableDictionary<int,int>();


    public TutorialFile()
    {
        
    }
    //You go to the GameData file(Assets/_Scripts/MVC/ThePark/SaveLoad/GameData.cs)and add fields for the data you want to save — for me, it will be an int and a dictionary for now.

    //then implement the SaveData and LoadData funcions
    public void LoadData(GameData data)
    {
        //Reference it the same way as it is in the GameData field.
        this.exempleInt = data.exempleint;
        this.exemple = data.exemple;
    }

    public void SaveData(GameData data)
    {
        //Assign the fields of the data object with the values you want to persist from your class.
         data.exempleint = this.exempleInt;
         data.exemple = this.exemple;
    }
    //and thats it

}

