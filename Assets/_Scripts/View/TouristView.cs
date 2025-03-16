using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TouristView : MonoBehaviour
{

    public Tourist tourist;
    private NavMeshAgent agent;
    private Animator animator;
    public int AnimalsSeen = 0;
    public void Init(Tourist tourist)
    {
        this.tourist = tourist;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        transform.position = tourist.Position;
    }

    // Update is called once per frame
    void Update()
    {
        if (tourist != null)
        {
            transform.position = agent.transform.position;
            tourist.CalculateMood(AnimalsSeen);

            // Teszteléshez 
            // Debug.Log(tourist.WaitingMood);
            // Debug.Log(tourist.TotalMood);
        }
    }


    //Visszaadja a state-jét a touristnak
    public TouristState GetState()
    {
        return tourist.state;
    }

    // Elindítja a NavMesh-t az autóhoz
    public void StartWalkingToCar(Vector3 destination)
    {
        if (agent != null && tourist != null)
        {
            tourist.SetState(TouristState.ON_WALK);
            agent.SetDestination(destination);
            animator.SetBool("isWalking", true);
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

        animator.SetBool("isWalking", false);
        animator.SetBool("isInteracting", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        tourist.SetState(TouristState.IN_CAR);
        gameObject.SetActive(false);
    }
}
