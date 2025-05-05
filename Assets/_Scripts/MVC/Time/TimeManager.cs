using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    // Singleton instance
    public static TimeManager Instance;
    // A teljes játékban kezdéstől használt idő
    public GameTime GlobalGameTime;
    // A játék sebességét befolyásoló változó
    private float timeMultiplier = 1.0f;
    // Megállítva?
    private bool isPaused;
    // Minden 15. perc eltelése a játékban / másodperc
    [SerializeField] private int secondsPer15GameMinute = 5;
    // Minden év egy állatnak / másodperc
    public int SecondsPerAnimalYear = 5;
    // Két random event közti várakozási idő
    [SerializeField] private int randomEventMinDelay = 1, randomEventMaxDelay = 10;

    // Zárási idő
    [SerializeField] private int closingHour = 18;

    // Nyitási idő
    [SerializeField] private int openingHour = 8;


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
    }

    public void Start()
    {
        StartRandomEventLoop(randomEventMinDelay, randomEventMaxDelay);
        StartCoroutine(UpdateTime());
        // Így minden InGame hét egy állat év ami kb 56 perc
        if (SecondsPerAnimalYear == 0) SecondsPerAnimalYear = (60 * 24) / 15 * 5 * 7;
    }

    public IEnumerator UpdateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPer15GameMinute);

            if (!isPaused)
            {
                GlobalGameTime.AddMinutes(15);
                // Debug.Log(GlobalGameTime.ToString());
            }
        }
    }
    public void PauseTime()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResumeTime()
    {
        Time.timeScale = timeMultiplier;
        isPaused = false;
    }

    public void SpeedUpTime()
    {
        timeMultiplier = timeMultiplier >= 2f ? 1.0f : timeMultiplier + 0.5f;
        Time.timeScale = timeMultiplier;
    }


    /*Random Event választása*/

    //Delay a két randomEvent között
    public void StartRandomEventLoop(int minDelay, int maxDelay)
    {
        StartCoroutine(RandomEventCoroutine(minDelay, maxDelay));
    }

    // Delay két random event kiválasztása között
    public IEnumerator RandomEventCoroutine(int minDelay, int maxDelay)
    {
        while (true)
        {
            yield return new WaitForSeconds(new System.Random().Next(minDelay, maxDelay));

            if (!isPaused) TriggerRandomEvent();
        }
    }

    // Kiválaszt egy random eventet, amennyiben nem üres elküldi a RandomEvents observable osztálynak
    public void TriggerRandomEvent()
    {
        RandomEvent choosenRandomEvent = GetRandomEvent();
        if (choosenRandomEvent != RandomEvent.None)
        {
            RandomEvents.Instance.NotifyObservers(choosenRandomEvent);
        }
    }

    // Mindegyiknek van egy "valószínűsége", a Random 0.0 és 1.0 - között fog választani
    public RandomEvent GetRandomEvent()
    {
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll < 0.001f && GlobalGameTime.hours >= closingHour && GlobalGameTime.hours <= openingHour) return RandomEvent.Raid;
        if (roll < 0.01f) return RandomEvent.Breed_animal;
        if (roll < 0.1f) return RandomEvent.Regrow;
        if (roll < 0.95f && GlobalGameTime.hours <= closingHour && GlobalGameTime.hours >= openingHour) return RandomEvent.Spawn_tourist;
        return RandomEvent.None;
    }

    public GameTime GetCurrentTime()
    {
        return GlobalGameTime;
    }
}
