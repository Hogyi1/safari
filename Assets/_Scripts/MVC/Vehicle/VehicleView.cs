using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VehicleState;

/// <summary>
/// View component for rendering and moving a vehicle using NavigatorComponent.
/// Handles navigation to positions and along routes without direct NavMeshAgent or animations.
/// </summary>
[RequireComponent(typeof(NavigatorComponent))]
public class VehicleView : MonoBehaviour, INavigatable, IInteractable
{
    private int iD;
    private VehicleModel model;
    private bool isActive = false;
    private VehicleState nextState;
    public List<AnimalType> AnimalsInView = new();

    [SerializeField] private NavigatorComponent navigator;
    [SerializeField] private FadeEffect fadeEffect;

    [SerializeField] LayerMask animalLayermask;

    /// <summary>
    /// Initializes this view with its corresponding model and sets up the navigator.
    /// </summary>
    /// <param name="vehicle">The VehicleModel instance to bind to.</param>
    // Inicializálás
    public void Init(VehicleModel model)
    {
        this.model = model;
        this.iD = model.ID;
    }


    /// <summary>
    /// Moves the vehicle along a sequence of waypoints in order.
    /// </summary>
    /// <param name="waypoints">Array of world positions defining the route.</param>
    public void MoveOnRoute(List<Vector3> waypoints, VehicleState newState)
    {
        // Delegate setting waypoints to navigator
        navigator.SetWayPoints(waypoints);
        nextState = newState;
        StartCoroutine(WaitForArrival(newState));
    }


    /// <summary>
    /// Waits until the navigator reports arrival, then updates state if necessary.
    /// </summary>
    private IEnumerator WaitForArrival(VehicleState newState)
    {
        yield return new WaitUntil(() => navigator.Arrived);

        VehicleManager.Instance.SetVehicleState(iD, newState);
        if (newState == Empty) ResetVehicle();
    }


    /// <summary>
    /// TriggerEnter handler: adds animal types to the in-view list.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if ((animalLayermask & (1 << other.gameObject.layer)) == 0)
            return;

        var av = other.GetComponent<AnimalView>();
        if (av != null && av.enabled)
        {
            AnimalType type = av.Model.Type;
            AnimalsInView.Add(type);
        }
    }


    /// <summary>
    /// TriggerExit handler: removes animal types from the in-view list.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if ((animalLayermask & (1 << other.gameObject.layer)) == 0)
            return;

        var av = other.GetComponent<AnimalView>();
        if (av != null && av.enabled)
        {
            AnimalType type = av.Model.Type;
            AnimalsInView.Remove(type);
        }
    }

    // === Delegation for INavigatable interface ===
    public void SetTarget(Vector3 dest) => navigator.SetTarget(dest);
    public void SetWayPoints(List<Vector3> wp) => navigator.SetWayPoints(wp);
    public void Follow(MonoBehaviour t) => navigator.Follow(t);
    public void StopMovementInstantly() => navigator.StopMovementInstantly();
    public void StopMovement() => navigator.StopMovement();
    public void ResetMovement() => navigator.ResetMovement();

    // === Getters ===
    public bool Arrived => navigator.Arrived;
    public List<Vector3> CurrentRoute => navigator.CurrentRoute;
    public VehicleState NextState => nextState;

    /// <summary>
    /// Resets the vehicle's position and state.
    /// </summary>
    public void ResetVehicle()
    {
        transform.position = VehicleManager.Instance.GetParkingSpot();
        VehicleManager.Instance.SetVehicleState(iD, Empty);
        if (isActive) InputManager.Instance.DisableView();
    }


    /// <summary>
    /// Sets the agents speed
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed)
    {
        navigator.Agent.speed = speed;
    }


    private void OnDestroy()
    {
        if (isActive) PopupManager.Instance.HidePopup();
    }


    /// <summary>
    /// Hover event – fades in the vehicle highlight.
    /// </summary>
    public void OnHover()
    {
        fadeEffect.FadeIn();
    }


    /// <summary>
    /// Mouse exit event – fades out if not active.
    /// </summary>
    public void OnExit()
    {
        if (!isActive)
        {
            fadeEffect.FadeOut();
        }
    }


    /// <summary>
    /// On interaction – activates UI popup and highlight.
    /// </summary>
    public void OnAction()
    {
        isActive = true;
        fadeEffect.FadeIn();
        PopupManager.Instance.ActivatePopup(VehicleManager.Instance.GetVehicle(iD).GetUIData(), gameObject);
    }


    /// <summary>
    /// Cancels interaction – fades out and marks inactive.
    /// </summary>
    public void OnCancel()
    {
        isActive = false;
        fadeEffect.FadeOut();
    }
}