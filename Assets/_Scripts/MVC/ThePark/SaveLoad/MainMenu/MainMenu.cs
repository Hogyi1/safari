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
        DataPersistenceManager.Instance.NewGame();
    }

    private void Start()
    {
        if (!DataPersistenceManager.Instance.HasGameData())
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
