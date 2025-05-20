using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour 
{

    //saveslot lista
    [SerializeField] private List<SaveSlot> saveSlots = new List<SaveSlot>();
    private bool isLoadingGame = false;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private GameObject addButton;
    [SerializeField] private string gameSceneName = "Final";
    [SerializeField] private Transform saveSlotContainer;
    [SerializeField] private GameObject saveSlotPrefab;

    private void Awake()
    {
        saveSlots = new List<SaveSlot>(this.GetComponentsInChildren<SaveSlot>());
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
        this.isLoadingGame = isLoadingGame;

        if (isLoadingGame)
        {
            headerText.text = "LOAD SAVE";
            addButton.SetActive(false);
        }
        else
        {
            headerText.text = "NEW GAME";
            addButton.SetActive(true);
        }

        foreach (Transform child in saveSlotContainer)
        {
            Destroy(child.gameObject);
        }
        saveSlots.Clear();

        
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        foreach (var kvp in profilesGameData)
        {
            string profileId = kvp.Key;
            GameData profileData = kvp.Value;

            GameObject newSlotGO = Instantiate(saveSlotPrefab, saveSlotContainer);
            newSlotGO.transform.SetParent(saveSlotContainer, false);

            SaveSlot slot = newSlotGO.GetComponent<SaveSlot>();
            slot.setProfileid(profileId);
            slot.SetData(profileData);

            if (profileData == null && isLoadingGame)
                slot.SetInteractable(false);
            else
                slot.SetInteractable(true);

            Button button = newSlotGO.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnSaveSlotClicked(slot));
            }

            saveSlots.Add(slot);
        }
    }

    public void AddNewSaveSlot()
    {
        GameObject newSlot = Instantiate(saveSlotPrefab, saveSlotContainer);
        // Lekérjük a SaveSlotUI komponenst
        SaveSlot slot = newSlot.GetComponent<SaveSlot>();
        if (slot != null)
        {
            slot.setProfileid(IDGenerator.GenerateID().ToString());
        }
        Button button = newSlot.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnSaveSlotClicked(slot));
        }
        saveSlots.Add(slot);
    }

}