using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI parkName;
    [SerializeField] private TextMeshProUGUI percentageCompleted;
    public bool hasData { get; private set; } = false;
    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        // there's no data for this profileId
        if (data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        // there is data for this profileId
        else
        {
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            //itt kell beállítani a két szöveget
            parkName.text = data.parkData.ParkName;
            // percentageCompleted.text = data.exempleint.ToString();
        }
    }

    public string GetProfileId() => profileId;

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }
}