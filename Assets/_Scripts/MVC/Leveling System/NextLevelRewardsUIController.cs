using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Controller for showing what will unlock at the next level.
/// Only refreshes when the player levels up.
/// </summary>
public class NextLevelRewardsUIController : MonoBehaviour, ILevelObserver
{
    /// <summary>
    /// TMP text that displays how many challenges unlock next level.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI challengeUnlockText;

    /// <summary>
    /// Parent transform under which item card prefabs will be instantiated.
    /// </summary>
    [SerializeField]
    private Transform itemsContainer;

    /// <summary>
    /// Prefab for an item card (must contain an Image and a TextMeshProUGUI).
    /// </summary>
    [SerializeField]
    private GameObject itemCardPrefab;

    /// <summary>
    /// Called whenever this UI is enabled. Registers for LEVEL_UP events
    /// and does an immediate UI refresh based on the current LevelManager state.
    /// </summary>
    private void OnEnable()
    {
        GameEvents.Instance.AddObserver(this);
        UpdateUI();
    }

    /// <summary>
    /// Unregisters from level-up events to avoid memory leaks when disabled.
    /// </summary>
    private void OnDisable()
    {
        if (GameEvents.Instance != null)
            GameEvents.Instance.RemoveObserver(this);
    }

    /// <summary>
    /// ILevelObserver callback invoked when a game event occurs.
    /// Only updates the UI in response to a LEVEL_UP event.
    /// </summary>
    /// <param name="eventType">Type of the event received.</param>
    /// <param name="amount">Associated value (unused).</param>
    public void OnNotify(EventType eventType, int amount)
    {
        if (eventType == EventType.LEVEL_UP) UpdateUI();
    }

    /// <summary>
    /// Rebuilds the challenge count text and item cards for the next level’s rewards.
    /// </summary>
    private void UpdateUI()
    {
        var mgr = LevelManager.Instance;
        int next = mgr.CurrentLevel + 1;

        if (next > mgr.MaxLevel)
        {
            challengeUnlockText.text = "No further unlocks";
            ClearItems();
            return;
        }

        var data = mgr.GetLevelData(next);
        int count = data.unlockChallenges.Length;

        if (count == 0)
            challengeUnlockText.text = "NO NEW ACHIEVEMENTS";
        else if (count == 1)
            challengeUnlockText.text = "1 NEW ACHIEVEMENT";
        else
            challengeUnlockText.text = count + " NEW ACHIEVEMENTS";

        ClearItems();
        foreach (int id in data.unlockItemIds)
        {
            Item itemToShow = null;
            foreach (var itm in ItemManager.Instance.getItems())
                if (itm.id == id)
                {
                    itemToShow = itm;
                    break;
                }
            if (itemToShow == null) continue;

            var card = Instantiate(itemCardPrefab, itemsContainer);
            card.GetComponentInChildren<Image>().sprite = itemToShow.imagePath;
            card.GetComponentInChildren<TextMeshProUGUI>().text = itemToShow.itemName;
        }
    }

    /// <summary>
    /// Removes all instantiated item cards from the items container.
    /// </summary>
    private void ClearItems()
    {
        foreach (Transform c in itemsContainer)
            Destroy(c.gameObject);
    }
}
