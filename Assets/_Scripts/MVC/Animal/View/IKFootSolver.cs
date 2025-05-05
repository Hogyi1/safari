using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint Constraint;
    [SerializeField] private Animator Animator;
    [SerializeField] private string Code;
    [SerializeField] private Transform RayOrigin;
    [SerializeField] private LayerMask RayCastLayer;
    [SerializeField] private Vector3 footOffset;
    [SerializeField] private float smoothing = 10f;
    [SerializeField] private Vector3 rotationOffsetEuler = new Vector3(0f, 0f, 0f);

    private Quaternion baseLocalRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Awake()
    {
        baseLocalRotation = transform.localRotation;
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void LateUpdate()
    {
        float footWeight = Animator.GetFloat(Code);
        Constraint.weight = footWeight;

        if (Physics.Raycast(RayOrigin.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 10f, RayCastLayer))
        {
            Quaternion rotToNormal = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Vector3 fwd = transform.parent.forward;
            Vector3 projFwd = Vector3.ProjectOnPlane(fwd, hit.normal).normalized;
            Quaternion rotLook = Quaternion.LookRotation(projFwd, hit.normal);

            Quaternion rotOffset = Quaternion.Euler(rotationOffsetEuler);

            targetRotation = rotLook * baseLocalRotation * rotOffset;

            targetPosition = hit.point + footOffset;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothing);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothing);
    }
}
