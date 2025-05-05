using System.Collections.Generic;
using UnityEngine;

public class BuildingPopup : MonoBehaviour
{
    public Transform cam;

    private List<IStructureUIComponent> UIComponents = new();

    private Canvas canvas;

    private const float InitialDistance = 10.44f;
    private void Start()
    {
        ButtonComponent bc = GetComponent<ButtonComponent>();
        UIComponents.Add(bc);
        SliderComponent sc = GetComponent<SliderComponent>();
        UIComponents.Add(sc);
        BaseComponent bsc = GetComponent<BaseComponent>();
        UIComponents.Add(bsc);

        canvas = GetComponentInChildren<Canvas>();

        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
        float Distance = Vector3.Distance(canvas.transform.position, cam.transform.position);
        canvas.transform.localScale = Vector3.one * Mathf.Max(Distance / InitialDistance, 0.75f);

        if (Distance >= 30 || Distance <= 2)
        {
            InputManager.Instance.DisableView();
            gameObject.SetActive(false);
        }
    }

    public void SetPopupData(Dictionary<StructureUIValues, object> Data)
    {
        if (Data == null) return;
        foreach (IStructureUIComponent component in UIComponents)
        {
            component.TrySetup(Data);
        }
    }
}
