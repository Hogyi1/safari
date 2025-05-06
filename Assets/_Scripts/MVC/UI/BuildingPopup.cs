using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages a world-space popup UI for buildings, handling initialization, orientation,
/// scaling, and smooth fade/scale animations.
/// </summary>
public class BuildingPopup : MonoBehaviour
{
    /// <summary>
    /// Reference to the scene camera transform for orienting and scaling the popup.
    /// </summary>
    public Transform cam;

    /// <summary>
    /// List of UI components within the popup that implement IStructureUIComponent.
    /// </summary>
    private List<IStructureUIComponent> UIComponents = new();

    /// <summary>
    /// Canvas component for controlling the popup's scale.
    /// </summary>
    private Canvas canvas;

    /// <summary>
    /// CanvasGroup component for controlling the popup's transparency and interactivity.
    /// </summary>
    private CanvasGroup canvasGroup;

    /// <summary>
    /// Currently running coroutine for fade/scale animations.
    /// </summary>
    private Coroutine currentRoutine;

    /// <summary>
    /// Default comfortable distance between camera and UI before scaling.
    /// </summary>
    private const float InitialDistance = 10.44f;

    /// <summary>
    /// Initializes UI components, canvas, and canvas group, then hides the popup.
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
    /// Orients the popup to face the camera, adjusts scale based on distance,
    /// and hides the popup if outside visible range.
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
    /// Shows the popup with a smooth fade-in and scale-up animation.
    /// </summary>
    public void Show()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        gameObject.SetActive(true);
        currentRoutine = StartCoroutine(FadeScaleRoutine(true));
    }

    /// <summary>
    /// Hides the popup with a smooth fade-out and scale-down animation.
    /// </summary>
    public void Hide()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeScaleRoutine(false));
    }

    /// <summary>
    /// Coroutine for animating the popup's fade and scale transitions.
    /// </summary>
    /// <param name="show">True to animate showing; false to animate hiding.</param>
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
