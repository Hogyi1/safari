using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateParkManager : MonoBehaviour
{

    public MainMenuController MainMenu;
    public SaveSlotsMenu SaveSlotsMenu;
    public GameObject LoadAndSave;
    public TMP_InputField inputField;   
    public TMP_Dropdown dropdown;
    public Button button;
    public PopUpScripts popup;


    public void ValidateInput()
    {
        string inputText = inputField.text.Trim();
        int selectedDropdownIndex = dropdown.value;
        string selectedDropdownText = dropdown.options[selectedDropdownIndex].text;

      
        if (string.IsNullOrEmpty(inputText))
        {
            popup.Show("Nem lehet üres a park neve");
            return;
        }

        MainMenu.mm_NavigationBarClick(LoadAndSave);
        SaveSlotsMenu.ActivateMenu(false);




    }


}
