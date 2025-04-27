using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    public void onNewGameClicked()
    {
        DisableMenuButtons();
        DataPersistanceManager.Instance.NewGane();
    }

    private void Start()
    {
        if (!DataPersistanceManager.Instance.HasGameData())
        {
            loadGameButton.interactable = false;
        }
    }

    public void onContinueGameClicled() {
    
    
    
    }


    private void DisableMenuButtons() { 
        newGameButton.interactable = false;
        loadGameButton.interactable = false;
    }




}
