using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the visual representation and navigation behavior of a Ranger entity,
/// including movement, shooting animation, and interaction with the RangerManager.
/// </summary>
[RequireComponent(typeof(NavigatorComponent))]
public class RangerView : MonoBehaviour
{
    private int iD;
    private RangerModel model;

    [SerializeField] private Animator animator;
    [SerializeField] private NavigatorComponent navigator;

    [Header("Animations")]
    /// <summary>
    /// Animator parameter for triggering the shooting animation.
    /// </summary>
    public static readonly int IsShooting = Animator.StringToHash("IsShooting");

    /// <summary>
    /// Initializes the view with the associated RangerModel.
    /// </summary>
    /// <param name="model">The data model representing this ranger.</param>
    public void Init(RangerModel model)
    {
        this.model = model;
        this.iD = model.ID;
    }

    /// <summary>
    /// Sends the ranger back to their station and switches to Resting state once arrived.
    /// </summary>
    /// <param name="destination">The destination to return to.</param>
    public void ReturnToStation(Vector3 destination)
    {
        navigator.SetTarget(destination);
        StartCoroutine(WaitForArrival(RangerState.Resting));
    }

    /// <summary>
    /// Called when the ranger has reached a target and should perform the shooting animation.
    /// </summary>
    public void AtTarget()
    {
        StartCoroutine(WaitForShooting());
    }

    /// <summary>
    /// Coroutine that plays the shooting animation and waits until it finishes.
    /// </summary>
    private IEnumerator WaitForShooting()
    {
        animator.SetBool(IsShooting, true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetBool(IsShooting, false);
    }

    /// <summary>
    /// Begins navigation towards a moving animal target.
    /// </summary>
    /// <param name="target">The target MonoBehaviour to follow (usually an animal).</param>
    public void StartWalkingTowardsAnimal(MonoBehaviour target)
    {
        navigator.Follow(target);
        StartCoroutine(WaitForArrival(RangerState.At_target));
    }

    /// <summary>
    /// Waits until the ranger reaches their destination, then updates their state.
    /// If resting, teleports them to spawn position and disables the GameObject.
    /// </summary>
    /// <param name="newState">The new RangerState to set upon arrival.</param>
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

    // === INavigatable delegations ===

    /// <summary>Sets a static destination for the ranger to walk to.</summary>
    public void SetTarget(Vector3 dest) => navigator.SetTarget(dest);

    /// <summary>Sets a series of waypoints for the ranger to follow.</summary>
    public void SetWayPoints(List<Vector3> wp) => navigator.SetWayPoints(wp);

    /// <summary>Starts following a target GameObject.</summary>
    public void Follow(MonoBehaviour t) => navigator.Follow(t);

    /// <summary>Immediately stops the ranger's movement without smooth deceleration.</summary>
    public void StopMovementInstantly() => navigator.StopMovementInstantly();

    /// <summary>Stops the ranger's movement gracefully (e.g., with deceleration).</summary>
    public void StopMovement() => navigator.StopMovement();

    /// <summary>Resets the ranger's movement state and path.</summary>
    public void ResetMovement() => navigator.ResetMovement();

    /// <summary>Returns whether the ranger has arrived at their current destination.</summary>
    public bool Arrived => navigator.Arrived;
}
