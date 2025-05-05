using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BodySolver : MonoBehaviour
{
    public Vector3[] allHitNormals = new Vector3[4];
    public Vector3[] allHitPos = new Vector3[4];
    public float maxRotationX = 50f;
    public float maxRotationZ = 20f;
    public float maxRotationStep = 1f;
    public float smoothing = 10f;
    [SerializeField] private LayerMask RayCastLayer;
    [SerializeField] private List<Transform> RayOrigins;
    [SerializeField] private Terrain terrain;

    private void Start()
    {
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        allHitNormals = new Vector3[RayOrigins.Count];
        allHitPos = new Vector3[RayOrigins.Count];
    }

    // Quadruped IK videó: https://www.youtube.com/watch?v=iMojuj0K63s
    private void RotateCharacterBody()
    {
        Vector3 aveHitNormal = Vector3.zero;
        for (int i = 0; i < allHitNormals.Length; i++)
        {
            aveHitNormal += allHitNormals[i];
        }
        aveHitNormal = (aveHitNormal / allHitNormals.Length).normalized;

        // 2. Szögek számítása a normálhoz képest
        float angleAboutX, angleAboutZ;
        ProjectedAxisAngles(out angleAboutX, out angleAboutZ, transform, aveHitNormal);
        float threshold = 0.1f; // csak ha nagyobb, mint 0.2 fok

        if (Mathf.Abs(angleAboutX) < threshold) angleAboutX = 0f;
        if (Mathf.Abs(angleAboutZ) < threshold) angleAboutZ = 0f;

        float characterXRotation = transform.eulerAngles.x;
        float characterZRotation = transform.eulerAngles.z;

        if (characterXRotation > 180f) characterXRotation -= 360f;
        if (characterZRotation > 180f) characterZRotation -= 360f;

        if (characterXRotation + angleAboutX < -maxRotationX)
            angleAboutX = -maxRotationX - characterXRotation;
        else if (characterXRotation + angleAboutX > maxRotationX)
            angleAboutX = maxRotationX - characterXRotation;

        if (characterZRotation + angleAboutZ < -maxRotationZ)
            angleAboutZ = -maxRotationZ - characterZRotation;
        else if (characterZRotation + angleAboutZ > maxRotationZ)
            angleAboutZ = maxRotationZ - characterZRotation;

        float bodyEulerX = Mathf.MoveTowardsAngle(0, angleAboutX, maxRotationStep);
        float bodyEulerZ = Mathf.MoveTowardsAngle(0, angleAboutZ, maxRotationStep);

        Vector3 currentEuler = transform.eulerAngles;
        Vector3 targetEuler = new Vector3(
            currentEuler.x + bodyEulerX,
            currentEuler.y,
            currentEuler.z + bodyEulerZ
        );

        transform.eulerAngles = Vector3.Lerp(currentEuler, targetEuler, Time.deltaTime * smoothing);
    }

    private void ProjectedAxisAngles(out float angleAboutX, out float angleAboutZ, Transform footTargetTransform, Vector3 hitNormal)
    {
        Vector3 xAxisProjected = ProjectOnContactPlane(footTargetTransform.forward, hitNormal).normalized;
        Vector3 zAxisProjected = ProjectOnContactPlane(footTargetTransform.right, hitNormal).normalized;

        angleAboutX = Vector3.SignedAngle(footTargetTransform.forward, xAxisProjected, footTargetTransform.right);
        angleAboutZ = Vector3.SignedAngle(footTargetTransform.right, zAxisProjected, footTargetTransform.forward);
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 normal)
    {
        return vector - Vector3.Dot(vector, normal) * normal;
    }

    private void GetNormals()
    {
        int i = 0;
        foreach (var RayOrigin in RayOrigins)
        {
            if (Physics.Raycast(RayOrigin.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 10f, RayCastLayer))
            {
                allHitNormals[i] = hit.normal;
                allHitPos[i] = hit.point;
                i++;
            }
        }
    }

    private void SetHeight()
    {
        float Height = 0f;
        foreach (var pos in allHitPos)
        {
            Height += terrain.SampleHeight(pos);
        }
        float avgHeight = Height / allHitPos.Length;
        Vector3 CurrentPosition = transform.position;
        Vector3 targetPos = new Vector3(CurrentPosition.x, avgHeight, CurrentPosition.z);

        transform.position = Vector3.Lerp(CurrentPosition, targetPos, Time.deltaTime * smoothing);
    }

    private void LateUpdate()
    {
        GetNormals();
        SetHeight();
        RotateCharacterBody();
    }
}
