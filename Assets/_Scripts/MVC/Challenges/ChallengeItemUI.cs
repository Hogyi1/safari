using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChallengeItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text prizeText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private GameObject collectedGo;
    [SerializeField] private Image progressBar;
    [SerializeField] private Button collectButton;

    public void Setup(Challenge challenge)
    {
        descriptionText.text = challenge.description;
        prizeText.text = "$ " + challenge.prize.ToString();
        progressText.text = $"{challenge.progress}/{challenge.goal}";
        progressBar.fillAmount = challenge.goal > 0 ? challenge.progress / challenge.goal : 0;

        collectButton.gameObject.SetActive(challenge.state == ChallengeState.COMPLETED);
        collectedGo.SetActive(challenge.state == ChallengeState.COLLECTED);

        collectButton.onClick.RemoveAllListeners();
        collectButton.onClick.AddListener(() =>
        {
            bool success = ChallengeManager.Instance.CollectChallenge(challenge.description);
            if (success)
            {
                Setup(challenge);

                ChallengeView view = FindFirstObjectByType<ChallengeView>();
                if (view != null)
                {
                    view.RefreshChallengeList();
                }
            }
        });
    }
}
