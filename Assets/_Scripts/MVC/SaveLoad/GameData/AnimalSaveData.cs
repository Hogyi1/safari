using UnityEngine;

[System.Serializable]
public class AnimalSaveData 
{
    public Vector3 position;
    public AnimalType type;
    public int Age;
    public AnimalSaveData(AnimalType type, Vector3 pos, int age)
    {
        this.position = pos;
        this.type = type;
        this.Age = age;

    }
}
