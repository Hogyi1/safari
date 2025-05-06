using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingPopup : MonoBehaviour
{
    public Transform cam; // Scene camera to adjust in world space

    private List<IStructureUIComponent> UIComponents = new(); // List of components to be actived
    private Canvas canvas; // Component to manipulate the scale of the UI
    private CanvasGroup canvasGroup; // Component to manipulate the transparency of the UI

    private Coroutine currentRoutine; // Keep track of the current coroutine

    private const float InitialDistance = 10.44f; // The distance between the camera and the UI that is comfortable to use at

    /// <summary>
    /// Initializes components and disables the popup on start.
    /// </summary>
    private void Start()
    {
        ButtonComponent bc = GetComponent<ButtonComponent>();
        UIComponents.Add(bc);
        SliderComponent sc = GetComponent<SliderComponent>();
        UIComponents.Add(sc);
        BaseComponent bsc = GetComponent<BaseComponent>();
        UIComponents.Add(bsc);

        canvas = GetComponentInChildren<Canvas>();
        canvasGroup = canvas.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Orients the popup to face the camera and updates scale based on distance. 
    /// Automatically hides it if too far or too close.
    /// </summary>
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
        float Distance = Vector3.Distance(canvas.transform.position, cam.transform.position);
        if (currentRoutine.IsUnityNull()) canvas.transform.localScale = Vector3.Lerp(canvas.transform.localScale, Vector3.one * Mathf.Max(Distance / InitialDistance, 0.75f), Time.deltaTime * 10f);

        if (Distance >= 30 || Distance <= 2)
        {
            InputManager.Instance.DisableView();
            Hide();
        }
    }

    /// <summary>
    /// Passes dynamic data to the popup UI components for setup.
    /// </summary>
    /// <param name="Data">Dictionary containing UI values for the structure.</param>
    public void SetPopupData(Dictionary<StructureUIValues, object> Data)
    {
        if (Data == null) return;
        foreach (IStructureUIComponent component in UIComponents)
        {
            component.TrySetup(Data);
        }
    }

    /// <summary>
    /// Displays the popup with a smooth fade and scale-in animation.
    /// </summary>
    public void Show()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        gameObject.SetActive(true);
        currentRoutine = StartCoroutine(FadeScaleRoutine(true));
    }

    /// <summary>
    /// Hides the popup with a smooth fade and scale-out animation.
    /// </summary>
    public void Hide()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeScaleRoutine(false));
    }

    /// <summary>
    /// Coroutine to animate fading and scaling of the popup when showing or hiding.
    /// </summary>
    /// <param name="show">True to show, false to hide.</param>
    private IEnumerator FadeScaleRoutine(bool show)
    {
        float duration = 0.25f;
        float time = 0f;

        Vector3 startScale = show ? Vector3.zero : canvas.transform.localScale;
        Vector3 endScale = show ? Vector3.one : Vector3.zero;

        float startAlpha = show ? 0f : canvasGroup.alpha;
        float endAlpha = show ? 1f : 0f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        while (time < duration)
        {
            float t = time / duration;
            canvas.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            time += Time.deltaTime;
            yield return null;
        }

        canvas.transform.localScale = endScale;
        canvasGroup.alpha = endAlpha;

        currentRoutine = null;
        if (!show)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            gameObject.SetActive(false);
        }
    }
}
