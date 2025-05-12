using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavigatorComponent))]
public class RangerView : MonoBehaviour
{
    private int iD;
    private RangerModel model;
    [SerializeField] private Animator animator;
    [SerializeField] private NavigatorComponent navigator;

    [Header("Animations")]
    public static readonly int IsShooting = Animator.StringToHash("IsShooting");

    // Inicializálás
    public void Init(RangerModel model)
    {
        this.model = model;
        this.iD = model.ID;
    }

    public void ReturnToStation(Vector3 destination)
    {
        navigator.SetTarget(destination);
        StartCoroutine(WaitForArrival(RangerState.Resting));
    }

    public void AtTarget()
    {
        StartCoroutine(WaitForShooting());
    }

    private IEnumerator WaitForShooting()
    {
        animator.SetBool(IsShooting, true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetBool(IsShooting, false);
    }

    // Elindítja a NavMesh-t az autóhoz
    public void StartWalkingTowardsAnimal(MonoBehaviour target)
    {
        navigator.Follow(target);
        StartCoroutine(WaitForArrival(RangerState.At_target));
    }

    // Megvárja míg odaér a kocsihoz, majd eltunteti (beszáll a kocsiba)
    private IEnumerator WaitForArrival(RangerState newState)
    {

        while (!navigator.Arrived)
        {
            yield return null;
        }

        RangerManager.Instance.SetRangerState(iD, newState);
        if (newState == RangerState.Resting)
        {
            transform.position = RangerManager.Instance.GetSpawnposition();
            gameObject.SetActive(false);
        }
    }

    // === INavigatable metódusok delegálása ===
    public void SetTarget(Vector3 dest) => navigator.SetTarget(dest);
    public void SetWayPoints(List<Vector3> wp) => navigator.SetWayPoints(wp);
    public void Follow(MonoBehaviour t) => navigator.Follow(t);
    public void StopMovementInstantly() => navigator.StopMovementInstantly();
    public void StopMovement() => navigator.StopMovement();
    public void ResetMovement() => navigator.ResetMovement();

    // === Érkezés logika ===
    public bool Arrived => navigator.Arrived;
}
