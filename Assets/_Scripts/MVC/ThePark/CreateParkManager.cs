using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateParkManager : MonoBehaviour
{
    public static CreateParkManager Instance;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }



    public MainMenuController MainMenu;
    public SaveSlotsMenu SaveSlotsMenu;
    public GameObject LoadAndNew;
    public TMP_InputField inputField;   
    public TMP_Dropdown dropdown;
    public Button button;
    public PopUpScripts popup;
    private string ParkName;
    private DifficultyEnum difficulty; 


    public void ValidateInput()
    {
        string inputText = inputField.text.Trim();
        difficulty = (DifficultyEnum)dropdown.value;
        if (string.IsNullOrEmpty(inputText))
        {
            popup.Show("Nem lehet üres a park neve");
            return;
        }
        ParkName = inputText;
        SetParkPropertys();
        MainMenu.mm_NavigationBarClick(LoadAndNew);
        SaveSlotsMenu.ActivateMenu(false);
    }

    public void SetParkPropertys() { 
        Park.Instance.ParkName = ParkName;
        Park.Instance.difficulty = difficulty;
    }

  

}
