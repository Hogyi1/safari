using UnityEngine;
using TMPro;

public class PopUpScripts : MonoBehaviour
{
    
    public GameObject popupPanel;
    public TMP_Text messageText;

    public void Show(string message)
    {
        messageText.text = message;
        popupPanel.SetActive(true);
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
    }
}
