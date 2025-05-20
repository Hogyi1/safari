using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServicesUI : MonoBehaviour
{
    /// <summary>
    /// List of UI components within the popup that contains the scripts.
    /// </summary>
    [SerializeField] private List<MonoBehaviour> UIComponents;

    /// <summary>
    /// List of popup components
    /// </summary>
    private List<IUIComponent> Components = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var mono in UIComponents)
        {
            var allBehaviours = mono.GetComponentsInChildren<MonoBehaviour>(true);

            foreach (var comp in allBehaviours.OfType<IUIComponent>())
            {
                Components.Add(comp);
            }
        }
    }

    void LateUpdate()
    {
        foreach (var comp in Components)
        {
            comp.OnPopupUpdate();
        }
    }

    /// <summary>
    /// Passes dynamic data to the popup UI components for setup.
    /// </summary>
    /// <param name="Data">Dictionary containing UI values for the structure.</param>
    public void SetUIData(Dictionary<UIKeys, object> Data)
    {
        if (Data == null) return;

        foreach (var comp in Components)
        {
            if (comp is IUIComponent component) component.TrySetup(Data);
        }
    }
}
