using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonComp : MonoBehaviour, IUIComponent
{
    [SerializeField] private Button myButton;

    /// <summary>
    /// Keys for handling the interactions with the button
    /// </summary>
    [SerializeField] private UIKeys eventKey;
    [SerializeField] private UIKeys interactKey;

    private Func<bool> predicate;

    public void TrySetup(Dictionary<UIKeys, object> data)
    {
        if (data.TryGetValue(eventKey, out var myFunc) && myFunc is Action action)
        {
            myButton.gameObject.SetActive(true);
            myButton.onClick.RemoveAllListeners();
            myButton.onClick.AddListener(() => action());
        }
        else myButton.gameObject.SetActive(false);

        if (data.TryGetValue(interactKey, out var myPredicate) && myPredicate is Func<bool> func) predicate = func;
        else predicate = null;

        myButton.interactable = true;
    }

    public void OnPopupUpdate()
    {
        if (predicate != null)
            myButton.interactable = predicate();
    }
}
