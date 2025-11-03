using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual and movement component for tourists, using NavigatorComponent and animations.
/// Handles navigation, arrival, and transition to "in vehicle" state.
/// </summary>
[RequireComponent(typeof(NavigatorComponent))]
public class TouristView : MonoBehaviour, INavigatable
{
    private int iD;
    private TouristModel model;

    [SerializeField] private Animator animator;
    [SerializeField] private NavigatorComponent navigator;

    [Header("Animations")]
    // Animator parameter hashes for faster access
    public static readonly int IsWalking = Animator.StringToHash("IsWalking");
    public static readonly int IsRunning = Animator.StringToHash("IsRunning");
    public static readonly int IsInteracting = Animator.StringToHash("IsInteracting");

    /// <summary>
    /// Initializes the view with its associated model.
    /// </summary>
    public void Init(TouristModel model)
    {
        this.model = model;
        this.iD = model.ID;
    }


    /// <summary>
    /// Starts the navigation toward the vehicle's position.
    /// </summary>
    public void StartWalkingToCar(Vector3 destination)
    {
        navigator.SetTarget(destination);
        StartCoroutine(WaitForArrival());
    }


    /// <summary>
    /// Waits until the tourist arrives at the vehicle, plays interaction animation, and deactivates the GameObject.
    /// </summary>
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


    // === INavigatable interface delegation ===
    public void SetTarget(Vector3 dest) => navigator.SetTarget(dest);
    public void SetWayPoints(List<Vector3> wp) => navigator.SetWayPoints(wp);
    public void Follow(MonoBehaviour t) => navigator.Follow(t);
    public void StopMovementInstantly() => navigator.StopMovementInstantly();
    public void StopMovement() => navigator.StopMovement();
    public void ResetMovement() => navigator.ResetMovement();


    /// <summary>
    /// Whether the tourist has arrived at their destination.
    /// </summary>
    public bool Arrived => navigator.Arrived;


    /// <summary>
    /// The tourist's current navigation destination.
    /// </summary>
    public Vector3 CurrentDestination => navigator.CurrentDestination;
}
