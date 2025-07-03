using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// View component for the speed-up button, showing different icon combinations based on the current time multiplier.
/// Requires an <see cref="Image"/> component on the same GameObject.
/// </summary>
[RequireComponent(typeof(Image))]
public class SpeedupButtonView : MonoBehaviour
{
    /// <summary>
    /// Array of icon <see cref="GameObject"/>s representing speed levels:
    /// icons[0] = normal speed,
    /// icons[1] = 1.5× speed,
    /// icons[2] = 2× speed.
    /// </summary>
    [SerializeField]
    private GameObject[] icons;

    /// <summary>
    /// Updates which icons are active based on the given multiplier.
    /// </summary>
    /// <param name="mult">The time multiplier (e.g. 1f, 1.5f, 2f).</param>
    public void CycleImages(float mult)
    {
        switch (mult)
        {
            case 1.5f:
                HideAllIcons();
                icons[0].SetActive(true);
                icons[1].SetActive(true);
                break;

            case 2f:
                ShowAllIcons();
                break;

            default:
                HideAllIcons();
                icons[0].SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Hides all speed-up icons.
    /// </summary>
    private void HideAllIcons()
    {
        foreach (GameObject icon in icons)
        {
            icon.SetActive(false);
        }
    }

    /// <summary>
    /// Shows all speed-up icons.
    /// </summary>
    private void ShowAllIcons()
    {
        foreach (GameObject icon in icons)
        {
            icon.SetActive(true);
        }
    }
}
