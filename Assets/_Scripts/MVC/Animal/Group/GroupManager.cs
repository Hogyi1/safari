using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GroupManager : MonoBehaviour
{
    public static GroupManager Instance;
    public static readonly float RADIUS = 10f;
    public static readonly float SEPARATION_TIME = 2f;

    private List<GroupModel> activeGroups = new();
    private Dictionary<int, GroupView> activeViews = new();
    private Dictionary<GroupModel, Coroutine> runningCoroutines = new();

    [SerializeField] private int Count;
    [SerializeField] private GameObject groupPrefab;
    [SerializeField] private GameObject groupParent;

    /// <summary>
    /// Sets up the singleton instance. Destroys this instance if another one already exists.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Starts the coroutine responsible for separating animals with different needs.
    /// </summary>
    private void Start()
    {
        //StartCoroutine(HandleDifferentNeeds());
    }

    /// <summary>
    /// Updates group states and removes empty groups each frame.
    /// </summary>
    private void LateUpdate()
    {
        Count = activeGroups.Count;

        //activeGroups
        //    .Select(g => { g.CalculateGroupState(); g.UpdateCircle(); return g; })
        //    .Where(g => g.MemberCount == 0)
        //    .ToList()
        //    .ForEach(RemoveGroup);

        foreach (var group in activeGroups.ToList())
        {
            group.UpdateCircle();
            group.CalculateGroupState();

            if (group.MemberCount == 0)
                RemoveGroup(group);
        }
    }

    /// <summary>
    /// Tries to let an animal join a group by its ID.
    /// </summary>
    public void EnterGroup(int groupID, int animalID, bool isChild)
    {
        GetGroupByID(groupID)?.TryJoin(animalID, isChild);
    }

    /// <summary>
    /// Creates a new group based on the given animal IDs.
    /// </summary>
    public void CreateGroup(List<int> animalIDs, GroupState state)
    {
        var animal = AnimalManager.Instance.GetAnimalByID(animalIDs.First());
        var animalDiet = animal.Model.Diet;
        var animalType = animal.Model.Type;
        int newID = IDGenerator.GenerateID();

        GameObject instance = Instantiate(groupPrefab);
        instance.transform.SetParent(groupParent.transform, true);

        var view = instance.GetComponent<GroupView>();
        activeViews.Add(newID, view);

        var group = new GroupModel(state, animalIDs, animalType, animalDiet, newID);
        view.Init(group);
        group.OnStateChanged += HandleGroupStateChanged;

        activeGroups.Add(group);
    }

    /// <summary>
    /// Removes a group from the system.
    /// </summary>
    public void RemoveGroup(GroupModel group)
    {
        if (!activeGroups.Contains(group)) return;

        group.OnStateChanged -= HandleGroupStateChanged;

        if (runningCoroutines.TryGetValue(group, out var coroutine))
        {
            StopCoroutine(coroutine);
            runningCoroutines.Remove(group);
        }

        activeGroups.Remove(group);
        activeViews.Remove(group.groupID, out var view);
        Destroy(view.gameObject);
    }

    /// <summary>
    /// Merges two groups into a new one if merging conditions are met.
    /// </summary>
    public void MergeGroups(int groupAID, int groupBID)
    {
        var a = GetGroupByID(groupAID);
        var b = GetGroupByID(groupBID);

        if (!CanMerge(a, b)) return;

        var mergedMembers = new HashSet<int>(a.Members);
        mergedMembers.UnionWith(b.Members);

        RemoveGroup(a);
        RemoveGroup(b);
        CreateGroup(mergedMembers.ToList(), GroupState.Idle);

        Debug.Log($"Merged group A: {a.groupID} and group B: {b.groupID}");
    }

    /// <summary>
    /// Checks if two groups can be merged based on size, type, and state.
    /// </summary>
    private bool CanMerge(GroupModel groupA, GroupModel groupB)
    {
        return groupA != null && groupB != null &&
               groupA.Members.Count + groupB.Members.Count <= 10 &&
               groupA.AnimalType == groupB.AnimalType &&
               groupA.State == groupB.State &&
               groupA.groupID < groupB.groupID;
    }

    /// <summary>
    /// Returns a random position within a defined radius of a given point, considering terrain height.
    /// </summary>
    public Vector3 GetRandomPointOnMap(Vector3 origin)
    {
        Vector2 offset2D = Random.insideUnitCircle * RADIUS;
        Vector3 candidate = origin + new Vector3(offset2D.x, 0, offset2D.y);

        var terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            float y = terrain.SampleHeight(candidate) + terrain.GetPosition().y;
            candidate.y = y;
        }

        return candidate;
    }

    ///// <summary>
    ///// Periodically checks for animals with different needs in the same group and separates them if necessary.
    ///// </summary>
    //private IEnumerator HandleDifferentNeeds()
    //{
    //    while (true)
    //    {
    //        foreach (var group in activeGroups)
    //        {
    //            group.ToMove.ToList().ForEach(group.LeaveGroup);

    //            if (group.ToMove.Count >= 2)
    //            {
    //                CreateGroup(group.ToMove.ToList());
    //            }

    //            group.ToMove.Clear();
    //            Debug.LogError($"Separated: {group.groupID} with {group.ToMove.Count} animals");
    //        }

    //        yield return new WaitForSeconds(SEPARATION_TIME);
    //    }
    //}

    /// <summary>
    /// Returns a group by its unique group ID.
    /// </summary>
    public GroupModel GetGroupByID(int groupID) =>
        activeGroups.FirstOrDefault(g => g.groupID == groupID);

    /// <summary>
    /// Reacts to group state changes by stopping any running coroutine and starting a new one based on the new state.
    /// </summary>
    private void HandleGroupStateChanged(GroupModel group)
    {
        if (runningCoroutines.TryGetValue(group, out var oldCoroutine))
        {
            StopCoroutine(oldCoroutine);
            runningCoroutines.Remove(group);
        }

        Coroutine newCoro = group.State switch
        {
            GroupState.SearchingFood or GroupState.SearchingWater => StartCoroutine(group.StartGroupSearching()),
            GroupState.Hungry or GroupState.Thirsty => StartCoroutine(group.SetGroupTargeting()),
            _ => null
        };

        if (newCoro != null)
            runningCoroutines[group] = newCoro;
    }
}
