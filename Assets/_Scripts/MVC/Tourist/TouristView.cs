using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavigatorComponent))]
public class TouristView : MonoBehaviour, INavigatable
{

    private int iD;
    private TouristModel model;
    [SerializeField] private Animator animator;
    [SerializeField] private NavigatorComponent navigator;

    public Vector3 destination;
    public bool hasPath;
    public float Loco;
    public bool isWalking;
    public bool arrived;

    public float waitingmood;
    public float mood;
    public AnimalType fav;


    [Header("Animations")]
    // Animator paraméterek (hash-ek)
    public static readonly int IsWalking = Animator.StringToHash("IsWalking");
    public static readonly int IsRunning = Animator.StringToHash("IsRunning");
    public static readonly int IsInteracting = Animator.StringToHash("IsInteracting");

    // Inicializálás
    public void Init(TouristModel model)
    {
        this.model = model;
        this.iD = model.ID;
    }

    // Elindítja a NavMesh-t az autóhoz
    public void StartWalkingToCar(Vector3 destination)
    {
        navigator.SetTarget(destination);
        StartCoroutine(WaitForArrival());
    }

    // Megvárja míg odaér a kocsihoz, majd eltunteti (beszáll a kocsiba)
    private IEnumerator WaitForArrival()
    {

        while (!navigator.Arrived)
        {
            yield return null;
        }

        animator.SetBool(IsInteracting, true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        TouristManager.Instance.SetTouristState(iD, TouristState.In_car);
        gameObject.SetActive(false);
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
