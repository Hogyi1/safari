using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AgentSync : MonoBehaviour
{
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private Animator Animator;

    private Vector2 smoothDeltaPosition;
    public float smoothingTime = 0.1f;
    public float moving = 0.5f;

    [Header("Animations")]
    // Animator paraméterek (hash-ek)
    public static readonly int IsWalking = Animator.StringToHash("IsWalking");
    public static readonly int Loco = Animator.StringToHash("Loco");

    /// <summary>
    /// Initializes the NavMeshAgent and Animator references if not manually assigned.
    /// Configures agent behavior to use root motion and custom position updates.
    /// </summary>
    private void Awake()
    {
        if (Agent.IsUnityNull())
            Agent = GetComponent<NavMeshAgent>();
        if (Animator.IsUnityNull())
            Animator = GetComponentInChildren<Animator>();

        Animator.applyRootMotion = true;
        Agent.updatePosition = false;
        Agent.updateRotation = true;
    }

    // <summary>
    /// Applies root motion from the animator to control character position,
    /// synchronizing it with the NavMeshAgent's next position, and aligning with terrain height.
    /// </summary>
    private void OnAnimatorMove()
    {
        if (!Agent.enabled || !Agent.hasPath) return;
        Vector3 rootPos = Animator.rootPosition;
        rootPos.y = Mathf.Max(Agent.nextPosition.y, Terrain.activeTerrain.SampleHeight(transform.position));
        transform.position = rootPos;
        Agent.nextPosition = rootPos;
    }

    /// <summary>
    /// Updates the character's movement state and animation parameters based on current navigation progress.
    /// Handles smoothing of velocity, stopping behavior, and position interpolation for natural movement.
    /// </summary>
    private void UpdateAnimatorParameters()
    {
        if (!Agent.enabled) return;
        Vector3 worldDelta = Agent.nextPosition - transform.position;
        worldDelta.y = 0f;

        float dx = Vector3.Dot(transform.right, worldDelta);
        float dz = Vector3.Dot(transform.forward, worldDelta);
        Vector2 deltaPos = new Vector2(dx, dz);

        float t = Mathf.Min(1f, Time.deltaTime / smoothingTime);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPos, t);

        Vector2 velocity = smoothDeltaPosition / Time.deltaTime;

        if (Agent.remainingDistance <= Agent.stoppingDistance)
            velocity = Vector2.Lerp(Vector2.zero, velocity,
                                    Agent.remainingDistance / Agent.stoppingDistance);

        bool shouldMove = Agent.remainingDistance > Agent.stoppingDistance
                       && Agent.velocity.sqrMagnitude > moving;

        float deltaMagnitude = worldDelta.magnitude;
        if (deltaMagnitude > Agent.radius / 2f)
        {
            transform.position = Vector3.Lerp(Animator.rootPosition, Agent.nextPosition, smoothingTime);
        }

        Animator.SetBool(IsWalking, shouldMove);
        Animator.SetFloat(Loco, velocity.magnitude);
    }

    /// <summary>
    /// Called every physics frame to evaluate movement and update animator state if the agent is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        if (Agent.enabled) UpdateAnimatorParameters();
    }

    /// <summary>
    /// Automatically resets animation parameters when this component is disabled,
    /// ensuring the character returns to idle state visually.
    /// </summary>
    private void OnDisable()
    {
        Animator.SetBool(IsWalking, false);
        Animator.SetFloat(Loco, 0);
    }
}
