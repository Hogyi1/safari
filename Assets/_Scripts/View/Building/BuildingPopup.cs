using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPopup : MonoBehaviour
{
    public Transform cam;

    private List<IUIComponent> UIComponents = new();

    private Canvas canvas;

    float InitialDistance = 10.44f;
    private void Start()
    {
        ButtonComponent bc = GetComponent<ButtonComponent>();
        UIComponents.Add(bc);
        SliderComponent sc = GetComponent<SliderComponent>();
        UIComponents.Add(sc);
        BaseComponent bsc = GetComponent<BaseComponent>();
        UIComponents.Add(bsc);

        gameObject.SetActive(false);

        canvas = GetComponentInChildren<Canvas>();
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
        float Distance = Vector3.Distance(canvas.transform.position, cam.transform.position);
        canvas.transform.localScale = Vector3.one * Mathf.Max(Distance / InitialDistance, 0.75f);
    }

    public void SetPopupData(Dictionary<UIComponent, object> Data)
    {
        if (Data == null) return;
        foreach (IUIComponent component in UIComponents)
        {
            component.TrySetup(Data);
        }
    }
}
