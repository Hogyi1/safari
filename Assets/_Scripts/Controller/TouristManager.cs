using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TouristManager : MonoBehaviour //: IRandomEventObserver
{
    // Singleton pattern
    public static TouristManager Instance { get; private set; }

    // Tourist prefabje
    [SerializeField]
    public GameObject touristPrefab;
    // Az aktív touristok
    public List<TouristView> activeTourists = new List<TouristView>();
    //Hova spawnolja
    public Vector3 Entrance = new Vector3(7.5f, 0.1f, -3.5f);
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /* Teszteléshez - lespawnol egy túristát majd elindítja egy autóhoz
    public void Start()
    {
        SpawnTourist();
        StartCoroutine(tocar());
    }

    private IEnumerator tocar()
    {
        yield return new WaitForSeconds(1);
        activeTourists[0].StartWalkingToCar(new Vector3(2.0f, 0.1f, -1.0f));
    }
    */

    public void SpawnTourist()
    {

        Tourist newTourist = new Tourist(Entrance);
        GameObject newTouristGO = Instantiate(touristPrefab, Entrance, Quaternion.identity);
        TouristView view = newTouristGO.GetComponent<TouristView>();
        view.Init(newTourist);
        activeTourists.Add(view);
        // A hely ahova lespawnolja
        // Entrance += new Vector3(-1.0f, 0.0f, 0.0f);
    }

    public void RemoveTourist(TouristView tourist)
    {
        activeTourists.Remove(tourist);
        Destroy(tourist.gameObject);
    }

    public void Update()
    {
        foreach (var view in activeTourists)
        {
            float delta = Time.deltaTime;
            if (view.GetState() == TouristState.FINISHED)
            {
                RemoveTourist(view);
            }
            else if (view.GetState() == TouristState.IN_QUEUE)
            {
                // TODO kocsiba helyezés
                view.tourist.ElapsedTime += delta;

            }
            else if (view.GetState() == TouristState.ON_TOUR)
            {
                view.tourist.ElapsedTime = delta;
            }
        }
    }

    /*public void OnNotify(RandomEvent event)
    {
        // TODO
        if (event == RandomEvent.SPAWN_TOURIST) 
        {
            SpawnTourist();
        }
    }*/
}
