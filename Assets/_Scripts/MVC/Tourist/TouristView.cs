using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TouristView : MonoBehaviour
{

    private Tourist tourist;
    private NavMeshAgent agent;
    private Animator animator;

    // Inicializálás
    public void Init(Tourist tourist)
    {
        this.tourist = tourist;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Elindítja a NavMesh-t az autóhoz
    public void StartWalkingToCar(Vector3 destination)
    {
        if (agent != null && tourist != null)
        {
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
        tourist.SetState(TouristState.In_car);
        gameObject.SetActive(false);
    }

    // Elindítja az agentet
    public void MoveTo(Vector3 Position)
    {
        agent.SetDestination(Position);
        animator.SetBool("isWalking", true);
    }
}
