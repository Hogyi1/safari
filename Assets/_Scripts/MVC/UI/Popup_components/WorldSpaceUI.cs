using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a world-space popup UI for buildings, handling initialization, orientation,
/// scaling, and smooth fade/scale animations.
/// </summary>
public class WorldSpaceUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the scene camera transform for orienting and scaling the popup.
    /// </summary>
    [SerializeField] private Transform cam;

    /// <summary>
    /// List of UI components within the popup that contains the scripts.
    /// </summary>
    [SerializeField] private List<MonoBehaviour> PopupComponents;

    /// <summary>
    /// List of popup components
    /// </summary>
    private List<IPopupComponent> Components = new();

    /// <summary>
    /// Max distance when the popup closes automatically
    /// </summary>
    [SerializeField] private float maxDistance;

    /// <summary>
    /// Min distance when the popup closes automatically
    /// </summary>
    [SerializeField] private float minDistance;

    /// <summary>
    /// Canvas component for controlling the popup's scale.
    /// </summary>
    [SerializeField] private Canvas canvas;

    /// <summary>
    /// CanvasGroup component for controlling the popup's transparency and interactivity.
    /// </summary>
    [SerializeField] private CanvasGroup canvasGroup;

    /// <summary>
    /// Currently running coroutine for fade/scale animations.
    /// </summary>
    private Coroutine currentRoutine;

    /// <summary>
    /// Default comfortable distance between camera and UI before scaling.
    /// </summary>
    private const float InitialDistance = 10.44f;

    /// <summary>
    /// The Transform to follow
    /// </summary>
    private Transform target;

    /// <summary>
    /// Initializes UI components, canvas, and canvas group, then hides the popup.
    /// </summary>
    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvasGroup = canvas.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();

        foreach (var mono in PopupComponents)
        {
            var allBehaviours = mono.GetComponentsInChildren<MonoBehaviour>(true);

            foreach (var comp in allBehaviours.OfType<IPopupComponent>())
            {
                Components.Add(comp);
            }
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Orients the popup to face the camera, adjusts scale based on distance, adjusts position based on target's position,
    /// and hides the popup if outside visible range.
    /// </summary>
    void LateUpdate()
    {
        if (target != null) transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 10f);
        transform.LookAt(transform.position + cam.forward);
        float Distance = Vector3.Distance(canvas.transform.position, cam.transform.position);
        if (currentRoutine.IsUnityNull()) canvas.transform.localScale = Vector3.Lerp(canvas.transform.localScale, Vector3.one * Mathf.Max(Distance / InitialDistance, 0.75f), Time.deltaTime * 10f);

        if (Distance >= maxDistance || Distance <= minDistance)
        {
            InputManager.Instance.DisableView();
        }

        foreach (var comp in Components)
        {
            comp.OnPopupUpdate();
        }
    }

    /// <summary>
    /// Passes dynamic data to the popup UI components for setup. With strict transform.
    /// </summary>
    /// <param name="Data">Dictionary containing UI values for the structure.</param>
    public void SetPopupData(Dictionary<UIKeys, object> Data, Transform target)
    {
        if (Data == null) return;
        this.target = target;

        foreach (var comp in Components)
        {
            if (comp is IPopupComponent component) component.TrySetup(Data);
        }
    }

    /// <summary>
    /// Passes dynamic data to the popup UI components for setup. With strict position.
    /// </summary>
    /// <param name="Data">Dictionary containing UI values for the structure.</param>
    public void SetPopupData(Dictionary<UIKeys, object> Data, Vector3 position)
    {
        if (Data == null) return;
        target = null;
        transform.position = position;

        foreach (var comp in Components)
        {
            if (comp is IPopupComponent component) component.TrySetup(Data);
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

        canvas.GetComponentsInChildren<RectTransform>().ToList().ForEach(t => LayoutRebuilder.ForceRebuildLayoutImmediate(t));

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
        yield return new WaitForEndOfFrame();
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

    public bool CheckDistance(Vector3 goPos)
    {
        float Distance = Vector3.Distance(goPos, cam.transform.position);
        return (Distance >= maxDistance || Distance <= minDistance);
    }
}
