using UnityEngine;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId;

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;

    [SerializeField] private TMP_Text parkName;
    [SerializeField] private TMP_Text percentageComplateText;

    public void SetData(GameData data) {
        
        if (data != null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            //percentageComplateText.text = Challengecount;
            //// parkName.text =  Park neve;

        }

    }
    public string GetProfileId() { 
    return this.profileId;
    }


}
