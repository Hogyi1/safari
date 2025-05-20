using System.Collections.Generic;
using UnityEngine;

public class SaveSlotsMenu : MonoBehaviour
{

    //saveslot lista
    [SerializeField] private SaveSlot[] saveSlots;
    private bool isLoadingGame = true;
    [SerializeField] private string gameSceneName = "Final";

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // update the selected profile id to be used for data persistence
        DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        //ha a createGamebol jon akkor inicializál
        if (!isLoadingGame)
        {
            DataPersistenceManager.Instance.NewGame();
        }
        else
        {
            DataPersistenceManager.Instance.LoadGame();
        }

        SceneLoadManager.LoadScene(gameSceneName);

    }

    public void ActivateMenu(bool isLoadingGame)
    {
        // set mode
        this.isLoadingGame = isLoadingGame;

        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        // loop through each save slot in the UI and set the content appropriately

        // Implement new saveslots that dinamically generate
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }

    }

}