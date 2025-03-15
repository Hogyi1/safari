using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TouristView : MonoBehaviour
{

    private Tourist tourist;
    private NavMeshAgent agent;
    public int AnimalsSeen = 0;
    public void Init(Tourist tourist)
    {
        this.tourist = tourist;
        agent = GetComponent<NavMeshAgent>();
        transform.position = tourist.Position;
    }

    // Update is called once per frame
    void Update()
    {
        if (tourist != null)
        {
            transform.position = agent.transform.position;
            tourist.CalculateMood(AnimalsSeen);
        }
    }

    // Elindítja a NavMesh-t az autóhoz
    public void StartWalkingToCar(Vector3 destination)
    {
        if (agent != null)
        {
            tourist.SetState(TouristState.ON_WALK);
            agent.SetDestination(destination);
            StartCoroutine(WaitForArrival());
        }
    }

    // Megvárja míg odaér a kocsihoz, majd eltunteti (beszáll a kocsiba)
    private IEnumerator WaitForArrival()
    {
        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        tourist.SetState(TouristState.IN_CAR);
        gameObject.SetActive(false);
    }
}
