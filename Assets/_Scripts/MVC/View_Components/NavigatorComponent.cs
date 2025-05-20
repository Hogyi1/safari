using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles movement logic using NavMeshAgent, providing functionality for direct navigation,
/// path following, and target tracking.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavigatorComponent : MonoBehaviour, INavigatable
{
    private NavMeshAgent agent;
    private Vector3 currentDestination;
    private List<Vector3> currentRoute;

    /// <summary>
    /// Public access to the NavMeshAgent (e.g., for external checks).
    /// </summary>
    public NavMeshAgent Agent => agent;

    // === LIFECYCLE ===
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // === MOVEMENT HANDLING ===

    /// <summary>
    /// Sets a destination for the agent to move toward.
    /// </summary>
    public void SetTarget(Vector3 destination)
    {
        if (!agent.enabled) return;

        currentDestination = destination;
        agent.SetDestination(destination);
        agent.isStopped = false;
    }

    /// <summary>
    /// Moves through multiple waypoints sequentially using a coroutine.
    /// </summary>
    public void SetWayPoints(List<Vector3> waypoints)
    {
        currentRoute = new(waypoints);
        StartCoroutine(TraverseWaypoints(waypoints));
    }

    /// <summary>
    /// Follows another object (e.g., animal or target) by tracking its position.
    /// </summary>
    public void Follow(MonoBehaviour targetView)
    {
        StartCoroutine(FollowRoutine(targetView));
    }

    /// <summary>
    /// Instantly stops all movement and disables this component.
    /// </summary>
    public void StopMovementInstantly()
    {
        if (!agent.enabled) return;

        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.enabled = false;
        enabled = false;
    }

    /// <summary>
    /// Stops movement but keeps this component active.
    /// </summary>
    public void StopMovement()
    {
        if (!agent.enabled) return;

        agent.ResetPath();
        agent.isStopped = true;
    }

    /// <summary>
    /// Resumes movement if it was previously stopped.
    /// </summary>
    public void ResetMovement()
    {
        if (!agent.enabled) return;
        agent.isStopped = false;
    }

    // === COROUTINES ===

    /// <summary>
    /// Visits each waypoint one by one until all are reached.
    /// </summary>
    private IEnumerator TraverseWaypoints(List<Vector3> waypoints)
    {
        foreach (var point in waypoints)
        {
            SetTarget(point);
            while (!Arrived || Vector3.Distance(transform.position, point) <= agent.stoppingDistance) yield return null;
            currentRoute.Remove(point);
        }

        currentRoute.Clear();
        StopMovement();
    }

    /// <summary>
    /// Continuously follows a moving target until close enough.
    /// </summary>
    private IEnumerator FollowRoutine(MonoBehaviour target)
    {
        while (target != null && Vector3.Distance(target.transform.position, transform.position) >= 1f)
        {
            SetTarget(target.transform.position);
            yield return new WaitForSeconds(0.5f);
        }

        agent.ResetPath();
        yield return null;
    }

    // === HELPER PROPERTY ===

    /// <summary>
    /// True if the agent has arrived at its destination or has no more path to follow.
    /// </summary>
    public bool Arrived
    {
        get
        {
            if (!agent.enabled) return true;

            bool navArrived = !agent.pathPending
                              && agent.remainingDistance <= agent.stoppingDistance
                              && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);

            bool closeEnough = Vector3.Distance(transform.position, currentDestination)
                               <= agent.stoppingDistance + 0.5f;

            return navArrived || closeEnough;
        }
    }

    public Vector3 CurrentDestination => currentDestination;
    public List<Vector3> CurrentRoute => currentRoute;
}
