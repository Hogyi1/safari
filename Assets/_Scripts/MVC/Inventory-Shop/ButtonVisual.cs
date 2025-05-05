using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ButtonVisual : MonoBehaviour
{
    public List<Button> buttons;
    public Color normalColor ;
    public Color selectedColor;
    
    public void HighlightSelectedButton(GameObject btn) {
        
        Button button = btn.GetComponent<Button>();
        Clear();
        ColorBlock cb = button.colors;
        cb.normalColor = selectedColor;
        button.colors = cb;
        Debug.Log("asd");
    }


    private void Clear() {

        foreach (var button in buttons) {
            ColorBlock cb = button.colors;
            cb.normalColor = normalColor;
            button.colors = cb;
        }
    }





}
