using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GroupView : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private SphereCollider mainCollider;
    [SerializeField] private LayerMask groupLayer;

    [Header("Debug Info (Read-Only)")]
    [SerializeField] private int groupID;
    [SerializeField] private GroupState state;
    [SerializeField] private AnimalType animalType;
    [SerializeField] private DietType dietType;
    [SerializeField] private int memberCount;
    [SerializeField] private Vector3 groupPosition;
    [SerializeField] private List<Vector3> foodSources;
    [SerializeField] private List<Vector3> waterSources;

    private GroupModel model;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, GroupManager.RADIUS);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (model.IsUnityNull()) return;

        GameObject go = other.gameObject;
        int layer = go.layer;

        if ((groupLayer.value & (1 << layer)) != 0 &&
            other.TryGetComponent<GroupView>(out GroupView otherView))
        {
            GroupManager.Instance.MergeGroups(GetID(), otherView.GetID());
        }
    }

    private void LateUpdate()
    {
        if (model.IsUnityNull()) return;

        // Update collider center based on model position
        mainCollider.center = model.Position;
        foodSources = model.foodSources.ToList();
        waterSources = model.waterSources.ToList();
        // Update debug fields
        groupID = model.groupID;
        state = model.State;
        animalType = model.AnimalType;
        dietType = model.DietType;
        memberCount = model.MemberCount;
        groupPosition = model.Position;

        // Optionally also update the GameObject position for visualization
        transform.position = model.Position;
    }

    public void Init(GroupModel model)
    {
        this.model = model;
    }

    public int GetID() => model.groupID;
}
