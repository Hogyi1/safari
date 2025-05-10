using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Represents the visual component of a Vehicle, handling navigation and animation.
/// </summary>
public class VehicleView : MonoBehaviour
{
    /// <summary>
    /// Reference to the vehicle model driving this view.
    /// </summary>
    private Vehicle vehicle;

    /// <summary>
    /// NavMeshAgent used for pathfinding and movement control.
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Animator component controlling vehicle animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// NavMeshObstacle used when the agent is disabled to block navigation.
    /// </summary>
    private NavMeshObstacle obstacle;

    /// <summary>
    /// Initializes the VehicleView with its model and component references,
    /// disabling the NavMeshAgent until movement begins.
    /// </summary>
    /// <param name="vehicle">The Vehicle model associated with this view.</param>
    public void Init(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();

        agent.enabled = false;
        obstacle.enabled = true;
    }

    /// <summary>
    /// Called once per frame. Reserved for future animation updates.
    /// </summary>
    private void Update()
    {
        // Intentionally left blank; animations can be driven here.
    }

    /// <summary>
    /// Returns the world position of the vehicle's door, used for passenger boarding.
    /// </summary>
    /// <returns>World-space position of the door.</returns>
    public Vector3 GetDoorPosition()
    {
        return this.gameObject.transform.GetChild(1).position;
    }

    /// <summary>
    /// Moves the vehicle to a specified destination by enabling the agent.
    /// </summary>
    /// <param name="Position">Target position in world space.</param>
    public void MoveTo(Vector3 Position)
    {
        obstacle.enabled = false;
        agent.enabled = true;
        agent.SetDestination(Position);
        StartCoroutine(WaitForArrival());
    }

    /// <summary>
    /// Begins following a series of waypoints sequentially.
    /// </summary>
    /// <param name="Waypoints">Array of world-space positions to traverse.</param>
    public void MoveOnRoute(Vector3[] Waypoints)
    {
        obstacle.enabled = false;
        agent.enabled = true;
        if (!agent.hasPath)
            StartCoroutine(FollowWaypoints(Waypoints));
    }

    /// <summary>
    /// Coroutine that navigates through each waypoint in order,
    /// and marks the vehicle as Finished upon completion.
    /// </summary>
    /// <param name="waypoints">List of positions to visit.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator FollowWaypoints(Vector3[] waypoints)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 tp = waypoints[i];

            // Wait until the agent reaches the waypoint
            while (agent.pathPending || agent.remainingDistance > 0.5f)
            {
                yield return null;
            }
        }

        vehicle.State = VehicleState.Finished;
        agent.enabled = false;
        obstacle.enabled = true;
    }

    /// <summary>
    /// Coroutine that waits until the vehicle has arrived at its destination,
    /// then updates its state to Empty if it was Busy.
    /// </summary>
    private IEnumerator WaitForArrival()
    {
        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        if (vehicle.State == VehicleState.Busy)
        {
            vehicle.State = VehicleState.Empty;
        }

        agent.enabled = false;
        obstacle.enabled = true;
    }
}
