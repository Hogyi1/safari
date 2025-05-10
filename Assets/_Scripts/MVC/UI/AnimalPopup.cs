using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays UI information for a selected animal in the world. 
/// Updates data in real-time while the popup is active.
/// </summary>
public class AnimalPopup : MonoBehaviour
{
    /// <summary>
    /// The animal data model displayed by this popup.
    /// </summary>
    private Animal Data;

    /// <summary>
    /// Text component showing the animal's type or name.
    /// </summary>
    [SerializeField] private TMP_Text _name;

    /// <summary>
    /// Text component showing the animal's age.
    /// </summary>
    [SerializeField] private TMP_Text info1;

    /// <summary>
    /// Text component showing the animal's current state.
    /// </summary>
    [SerializeField] private TMP_Text info2;

    /// <summary>
    /// Text component showing the animal's diet.
    /// </summary>
    [SerializeField] private TMP_Text info3;

    /// <summary>
    /// Button to set this animal as a target (e.g., for removal).
    /// </summary>
    [SerializeField] private Button setTarget;

    /// <summary>
    /// Button to close the animal popup.
    /// </summary>
    [SerializeField] private Button close;

    /// <summary>
    /// Image component representing the animal's health bar.
    /// </summary>
    [SerializeField] private Image healthbar;

    /// <summary>
    /// Disables the popup at start to ensure it's not visible by default.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the popup UI each frame with the animal's age, state, and HP.
    /// Automatically hides if the animal is flagged for removal.
    /// </summary>
    void Update()
    {
        if (Data != null)
        {
            info1.text = Data.Model.Age.ToString();
            info2.text = Data.Brain.RootState.ToString();
            healthbar.fillAmount = Data.Model.Hp / 100f;
        }

        if (Data.CanRemove)
        {
            InputManager.Instance.DisableView();
        }
    }

    /// <summary>
    /// Initializes the popup with the given animal's data and binds button actions.
    /// </summary>
    /// <param name="animal">The animal whose data should be shown.</param>
    public void SetPopupData(Animal animal)
    {
        Data = animal;

        info3.text = animal.Model.Diet.ToString();
        info2.text = animal.Brain.RootState.ToString();
        _name.text = animal.Model.Type.ToString();

        setTarget.onClick.RemoveAllListeners();
        setTarget.onClick.AddListener(() => { AnimalManager.Instance.KillAnimal(animal); });
        close.onClick.AddListener(() => { InputManager.Instance.DisableView(); });
    }
}
