using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalPopup : MonoBehaviour
{
    private Animal Data;

    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text info1;
    [SerializeField] private TMP_Text info2;
    [SerializeField] private TMP_Text info3;
    [SerializeField] private Button setTarget;
    [SerializeField] private Button close;
    [SerializeField] private Image healthbar;

    private void Start()
    {
        gameObject.SetActive(false);
    }

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

    public void SetPopupData(Animal animal)
    {
        Data = animal;

        info3.text = animal.Model.Diet.ToString();
        info2.text = animal.Brain.RootState.ToString();
        _name.text = animal.Model.Type.ToString();

        setTarget.onClick.AddListener(() => { AnimalManager.Instance.KillAnimal(animal); });
        close.onClick.AddListener(() => { InputManager.Instance.DisableView(); });
    }
}
