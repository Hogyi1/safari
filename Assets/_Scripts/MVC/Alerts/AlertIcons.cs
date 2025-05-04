using UnityEngine;

/// <summary>
/// Singleton to provide Check and Cross icons for alerts.
/// </summary>
public class AlertIcons : MonoBehaviour
{
    public static AlertIcons Instance { get; private set; }

    [Header("Alert Icon Sprites")]
    public Sprite checkSprite;
    public Sprite crossSprite;

    /// <summary>
    /// Ensures only one instance of AlertIcons exists and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
