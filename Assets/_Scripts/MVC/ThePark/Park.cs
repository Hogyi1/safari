using UnityEngine;

[System.Serializable]
public class Park : MonoBehaviour, IDataPersistence 
{
    
    public int ID;
    public static Park Instance;
    public string ParkName;


    public DifficultyEnum difficulty;


    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }


    public void LoadData(GameData data)
    {
      this.ParkName = data.parkData.parkName;
      this.difficulty = data.parkData.difficulty;
            
    }

    public void SaveData(GameData data)
    {
        data.parkData.parkName = this.ParkName;
        data.parkData.difficulty = this.difficulty;
    }

}
