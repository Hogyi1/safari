using UnityEngine;

/// <summary>
/// Visual representation for vegetation structures that supports interaction,
/// staging (growth), and data access through placement.
/// </summary>
public class VegetationView : MonoBehaviour, IInteractable, IStageable, IPlaceable
{
    /// <summary>
    /// Data describing the building type and attributes.
    /// </summary>
    [SerializeField] private BuildingData data;

    /// <summary>
    /// Associated model for this view.
    /// </summary>
    private Structure MySelectable;

    /// <summary>
    /// Whether the object is currently selected or active.
    /// </summary>
    private bool isActive = false;

    /// <summary>
    /// Handles fade-in/out effects based on interaction.
    /// </summary>
    private FadeEffect fadeEffect;

    /// <summary>
    /// Controls the visual stage (e.g. opacity) of this object.
    /// </summary>
    private StageEffect stageEffect;

    /// <summary>
    /// Tracks the last known stage value to detect changes.
    /// </summary>
    private float MyStage = 0f;

    /// <summary>
    /// Monitors the current stage and updates it if needed.
    /// </summary>
    private void Update()
    {
        if (GetStage() != MyStage)
        {
            SetStage(GetStage());
            MyStage = GetStage();
        }
    }


    /// <summary>
    /// Gets references to attached visual effect components.
    /// </summary>
    private void Awake()
    {
        fadeEffect = GetComponent<FadeEffect>();
        stageEffect = GetComponent<StageEffect>();
    }


    /// <summary>
    /// Initializes the view with its model data and sets default visual state.
    /// </summary>
    /// <param name="selectable">The model structure this view represents.</param>
    public void Init(Structure selectable)
    {
        this.MySelectable = selectable;
        isActive = false;
        fadeEffect.FadeOut();
    }


    /// <summary>
    /// Called when the mouse hovers over this object — fades in visually.
    /// </summary>
    public void OnHover() => fadeEffect.FadeIn();


    /// <summary>
    /// Called when the mouse exits this object — fades out if not active.
    /// </summary>
    public void OnExit()
    {
        if (!isActive)
            fadeEffect.FadeOut();
    }


    /// <summary>
    /// Called on click or interaction — activates popup and sets to active state.
    /// </summary>
    public void OnAction()
    {
        isActive = true;
        fadeEffect.FadeIn();
        PopupManager.Instance.ActivatePopup(((ISelectable)MySelectable).GetUIData(), GetGameObject());
    }


    /// <summary>
    /// Called when interaction is canceled — reverts fade and clears active state.
    /// </summary>
    public void OnCancel()
    {
        isActive = false;
        fadeEffect.FadeOut();
    }


    /// <summary>
    /// Returns the GameObject associated with this view.
    /// </summary>
    public GameObject GetGameObject() => gameObject;


    /// <summary>
    /// Returns the unique ID of the associated structure.
    /// </summary>
    public int GetID() => MySelectable.GetID();


    /// <summary>
    /// Returns the building type of the associated structure.
    /// </summary>
    public BuildingType GetBuildingType() => MySelectable.GetBuildingType();


    /// <summary>
    /// Returns the current stage (growth or visibility) of the structure.
    /// </summary>
    public float GetStage() =>
        MySelectable is VegetationModel vegetation ? vegetation.GetStage() : 0;


    /// <summary>
    /// Applies the given stage value to the visual material system.
    /// </summary>
    /// <param name="stage">A float value representing the current stage.</param>
    public void SetStage(float stage) => stageEffect.SetMaterialsOpacity(stage);


    /// <summary>
    /// Returns the model (Structure) associated with this view.
    /// </summary>
    public Structure GetStructure() => MySelectable;


    /// <summary>
    /// Returns the building data used to initialize this view.
    /// </summary>
    public BuildingData GetData() => data;
}
