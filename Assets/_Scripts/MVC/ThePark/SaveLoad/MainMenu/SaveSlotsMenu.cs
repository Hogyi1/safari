using UnityEngine;
using System.Collections.Generic;

public class SaveSlotsMenu : MonoBehaviour
{

    private SaveSlot[] saveSlots;
    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void ActivateMenu() { 
        
        Dictionary<string, GameData> profilesGameData = new Dictionary<string, GameData>();
        foreach (SaveSlot saveSlot in saveSlots )
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
        }
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot) {
        DataPersistanceManager.Instance.ChangeSelectedProdileId(saveSlot.GetProfileId());
        DataPersistanceManager.Instance.NewGane();
            //load the scene
     
    }



}
