using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton MonoBehaviour controlling game time progression, pausing, speed, and random event scheduling.
/// </summary>
public class TimeManager : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// Singleton instance of TimeManager.
    /// </summary>
    public static TimeManager Instance;

    /// <summary>
    /// Global GameTime tracking in-game temporal state.
    /// </summary>
    public GameTime GlobalGameTime;

    /// <summary>
    /// Multiplier applied to Unity's time scale for speed controls.
    /// </summary>
    private float timeMultiplier = 1.0f;

    /// <summary>
    /// Flag indicating whether time is currently paused.
    /// </summary>
    private bool isPaused;

    /// <summary>
    /// Real seconds per 15 in-game minutes.
    /// </summary>
    [SerializeField] private int secondsPer15GameMinute = 5;

    /// <summary>
    /// Seconds per animal year calculation.
    /// </summary>
    public int SecondsPerAnimalYear = 5;

    /// <summary>
    /// Minimum and maximum delay in seconds between random events.
    /// </summary>
    [SerializeField] private int randomEventMinDelay = 1, randomEventMaxDelay = 10;

    /// <summary>
    /// Hour of day when the park closes (inclusive).
    /// </summary>
    [SerializeField] private int closingHour = 18;

    /// <summary>
    /// Hour of day when the park opens.
    /// </summary>
    [SerializeField] private int openingHour = 8;

    public float Priority => 5000f;
    private Action OnHandlerResponse;

    /// <summary>
    /// Initializes the singleton, global time, and sets opening hour.
    /// </summary>
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        GlobalGameTime = new GameTime();
        GlobalGameTime.AddHours(openingHour);

        OnHandlerResponse = () => gameObject.SetActive(true);
        DataPersistenceManager.Instance.OnAllLoaded += OnHandlerResponse;

        gameObject.SetActive(false);
    }

    private void OnDestroy() => DataPersistenceManager.Instance.OnAllLoaded -= OnHandlerResponse;

    /// <summary>
    /// Starts the time update and random event loops.
    /// </summary>
    public void Start()
    {
        StartRandomEventLoop(randomEventMinDelay, randomEventMaxDelay);
        StartCoroutine(UpdateTime());

        // This makes each in-game week correspond to one animal year, which is about 56 minutes
        if (SecondsPerAnimalYear == 0) SecondsPerAnimalYear = (60 * 24) / 15 * 5 * 7;
        Debug.Log("Gametime loaded in start");
    }

    /// <summary>
    /// Coroutine advancing in-game time at fixed real-time intervals.
    /// </summary>
    /// <returns>IEnumerator for coroutine management.</returns>
    public IEnumerator UpdateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPer15GameMinute);

            if (!isPaused)
            {
                GlobalGameTime.AddMinutes(15);
            }
        }
    }

    /// <summary>
    /// Pauses the in-game time progression.
    /// </summary>
    public void PauseTime()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    /// <summary>
    /// Resumes the in-game time progression at the current multiplier.
    /// </summary>
    public void ResumeTime()
    {
        Time.timeScale = timeMultiplier;
        isPaused = false;
    }

    /// <summary>
    /// Toggles time speed among 1x, 1.5x, 2x, then back to 1x.
    /// </summary>
    public void SpeedUpTime()
    {
        timeMultiplier = timeMultiplier >= 2f ? 1.0f : timeMultiplier + 0.5f;
        Time.timeScale = timeMultiplier;
    }

    /// <summary>
    /// Begins the loop that triggers random events at random intervals.
    /// </summary>
    /// <param name="minDelay">Minimum delay in seconds.</param>
    /// <param name="maxDelay">Maximum delay in seconds.</param>
    public void StartRandomEventLoop(int minDelay, int maxDelay)
    {
        StartCoroutine(RandomEventCoroutine(minDelay, maxDelay));
    }

    /// <summary>
    /// Coroutine selecting and notifying random events periodically.
    /// </summary>
    public IEnumerator RandomEventCoroutine(int minDelay, int maxDelay)
    {
        while (true)
        {
            yield return new WaitForSeconds(new System.Random().Next(minDelay, maxDelay));

            if (!isPaused) TriggerRandomEvent();
        }
    }

    /// <summary>
    /// Chooses a random event and notifies observers if applicable.
    /// </summary>
    public void TriggerRandomEvent()
    {
        RandomEvent choosenRandomEvent = GetRandomEvent();
        if (choosenRandomEvent != RandomEvent.None)
        {
            RandomEvents.Instance.NotifyObservers(choosenRandomEvent);
        }
    }

    /// <summary>
    /// Determines which random event occurs based on weighted probability and time of day.
    /// </summary>
    /// <returns>Selected RandomEvent enum value.</returns>
    public RandomEvent GetRandomEvent()
    {
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll < 0.001f && GlobalGameTime.hours >= closingHour && GlobalGameTime.hours <= openingHour) return RandomEvent.Raid;
        if (roll < 0.01f) return RandomEvent.Breed_animal;
        if (roll < 0.1f) return RandomEvent.Regrow;
        if (roll < 0.95f && GlobalGameTime.hours <= closingHour && GlobalGameTime.hours >= openingHour) return RandomEvent.Spawn_tourist;
        return RandomEvent.None;
    }

    /// <summary>
    /// Gets the current global GameTime instance.
    /// </summary>
    /// <returns>Current GameTime.</returns>
    public GameTime GetCurrentTime() => GlobalGameTime;

    public IEnumerator LoadData(GameData data)
    {
        GlobalGameTime = new GameTime(data.GameTime);
        yield return null;
    }

    public void SaveData(GameData data)
    {
        data.GameTime = new GameTime(GlobalGameTime);
    }
}
