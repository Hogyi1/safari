using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour 
{ 


    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;
    [SerializeField] private string gameSceneName = "Bemutato";

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
    
        // update the selected profile id to be used for data persistence
        DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if (!isLoadingGame)
        {
            // create a new game - which will initialize our data to a clean slate
            DataPersistenceManager.Instance.NewGame();
        }

        // load the scene - which will in turn save the game because of OnSceneUnloaded() in the DataPersistenceManager
        if (SceneHandler.Instance != null)
        {

            DataPersistenceManager.Instance.SaveGame();
            SceneHandler.Instance.LoadGameScene(gameSceneName);
        }
    }

    public void OnBackClicked()
    {
        
        this.DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        // set mode
        this.isLoadingGame = isLoadingGame;

        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        // loop through each save slot in the UI and set the content appropriately
        
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

        // set the first selected button
      //  StartCoroutine(this.SetFirstSelected(firstSelected));
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

   
}