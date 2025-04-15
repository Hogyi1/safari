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
    private float secondsPer15GameMinute = 5.0f;
    // Két random event közti várakozási idő
    private int randomEventMinDelay = 1, randomEventMaxDelay = 5;

    // Zárási idő
    [SerializeField]
    private int ClosingHour = 18;

    // Nyitási idő
    [SerializeField]
    private int OpeningHour = 8;


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
        GlobalGameTime.AddHours(OpeningHour);
    }

    public void Start()
    {
        StartRandomEventLoop(randomEventMinDelay, randomEventMaxDelay);
        StartCoroutine(UpdateTime());
    }

    public IEnumerator UpdateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPer15GameMinute);

            if (!isPaused)
            {
                GlobalGameTime.AddMinutes(15);
                Debug.Log(GlobalGameTime.ToString());
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
        if (roll < 0.001f && GlobalGameTime.hours >= ClosingHour && GlobalGameTime.hours <= OpeningHour) return RandomEvent.Raid;
        if (roll < 0.01f) return RandomEvent.Breed_animal;
        if (roll < 0.1f) return RandomEvent.Regrow;
        if (roll < 0.95f && GlobalGameTime.hours <= ClosingHour && GlobalGameTime.hours >= OpeningHour) return RandomEvent.Spawn_tourist;
        return RandomEvent.None;
    }

    public GameTime GetCurrentTime()
    {
        return GlobalGameTime;
    }
}
