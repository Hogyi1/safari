using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavigatorComponent : MonoBehaviour, INavigatable
{
    private NavMeshAgent agent;
    private Vector3 currentDestination;
    private List<Vector3> currentRoute;

    // Publikus hozzáférés az Agenthez (pl. külső ellenőrzéshez)
    public NavMeshAgent Agent => agent;

    // === ÉLETCIKLUS ===
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // === MOZGÁS KEZELÉS ===

    /// <summary>
    /// Célpont beállítása, ahova az állat el akar jutni.
    /// </summary>
    public void SetTarget(Vector3 destination)
    {
        if (!agent.enabled) return;

        currentDestination = destination;
        agent.SetDestination(destination);
        agent.isStopped = false;
    }

    /// <summary>
    /// Több pont bejárása egymás után coroutine-nal.
    /// </summary>
    public void SetWayPoints(List<Vector3> waypoints)
    {
        currentRoute = new(waypoints);
        StartCoroutine(TraverseWaypoints(waypoints));
    }

    /// <summary>
    /// Egy másik objektum (pl. egy állat vagy célpont) követése pozíció alapján.
    /// </summary>
    public void Follow(MonoBehaviour targetView)
    {
        StartCoroutine(FollowRoutine(targetView));
    }

    /// <summary>
    /// Mozgás teljes leállítása azonnal, és a komponens deaktiválása.
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
    /// Mozgás leállítása (de a komponens aktív marad).
    /// </summary>
    public void StopMovement()
    {
        if (!agent.enabled) return;

        agent.ResetPath();
        agent.isStopped = true;
    }

    /// <summary>
    /// Mozgás újraindítása, ha korábban le lett állítva.
    /// </summary>
    public void ResetMovement()
    {
        if (!agent.enabled) return;
        agent.isStopped = false;
    }

    // === COROUTINE-OK ===

    /// <summary>
    /// Bejárja az összes waypoint-ot egyesével, amíg el nem ér minden pontra.
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
    /// Egy célpont folyamatos követése, amíg közel nem kerül hozzá.
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

    // === SEGÉD PROPERTY ===

    /// <summary>
    /// True, ha az ügynök elérte a célpontját vagy már nincs mit követnie.
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
