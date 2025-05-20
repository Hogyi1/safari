using UnityEngine;

/// <summary>
/// Fetches tourist-related data from the TouristManager and updates the TouristUI view accordingly.
/// </summary>
public class TouristUIController : MonoBehaviour
{
    [SerializeField] private TouristUI ui;

    /// <summary>
    /// Called once per frame, after all Updates.
    /// Gathers current tourist statistics and updates the view with the latest values.
    /// </summary>
    private void LateUpdate()
    {
        var touristMgr = TouristManager.Instance;

        ui.SetFavoriteAnimal(touristMgr.FavouriteAnimal.ToString());
        ui.SetTouristCount(touristMgr.AllTimeVisitors);
        ui.SetAllMoods(touristMgr.OverallMood, touristMgr.OverallWaitingMood, touristMgr.OverallFeeMood);
    }
}
