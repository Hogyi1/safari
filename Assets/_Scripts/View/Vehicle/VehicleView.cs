using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class VehicleView : MonoBehaviour
{
    private Vehicle vehicle;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
    // Inicializálás
    public void Init(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();

        agent.enabled = false;
        obstacle.enabled = true;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    // Visszaadja, hogy hol van a jármű ajtaja
    public Vector3 GetDoorPosition()
    {
        return this.gameObject.transform.GetChild(1).position;
    }

    // Beállítja az agent úticélját
    public void MoveTo(Vector3 Position)
    {
        obstacle.enabled = false;
        agent.enabled = true;
        agent.SetDestination(Position);
        StartCoroutine(WaitForArrival());
    }

    // Elindítja az útvonalkövetést
    public void MoveOnRoute(Vector3[] Waypoints)
    {
        obstacle.enabled = false;
        agent.enabled = true;
        if (!agent.hasPath)
            StartCoroutine(FollowWaypoints(Waypoints));
    }

    // Követi a megadott útvonalat
    private IEnumerator FollowWaypoints(Vector3[] waypoints)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 tp = waypoints[i];
            Debug.Log(agent.SetDestination(tp));

            // Wait until the agent reaches the waypoint
            while (agent.pathPending || agent.remainingDistance > 0.5f)
            {
                Debug.Log($"Current Distance to waypoint {i}: {agent.remainingDistance}");
                yield return null;
            }

            Debug.Log($"Reached waypoint {i}");
        }

        vehicle.State = VehicleState.FINISHED;
        agent.enabled = false;
        obstacle.enabled = true;
    }

    private IEnumerator WaitForArrival()
    {
        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        if (vehicle.State == VehicleState.BUSY)
        {
            vehicle.State = VehicleState.EMPTY;
        }

        agent.enabled = false;
        obstacle.enabled = true;
    }
}
