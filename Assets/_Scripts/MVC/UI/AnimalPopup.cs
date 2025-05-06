using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays UI information for a selected animal in the world. 
/// Updates data in real-time while the popup is active.
/// </summary>
public class AnimalPopup : MonoBehaviour
{
    private Animal Data; // UI Data

    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text info1;
    [SerializeField] private TMP_Text info2;
    [SerializeField] private TMP_Text info3;
    [SerializeField] private Button setTarget;
    [SerializeField] private Button close;
    [SerializeField] private Image healthbar;

    /// <summary>
    /// Disables the popup at start.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Continuously updates the UI with the animal's state, age, and HP.
    /// Automatically disables view if the animal is flagged for removal.
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
    /// Initializes the popup with animal-specific data and binds button actions.
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
