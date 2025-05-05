using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public static GroupManager Instance;
    private static readonly float RADIUS = 10f;

    private List<Group> activeGroups = new List<Group>();
    private Dictionary<Group, Coroutine> runningCoroutines = new Dictionary<Group, Coroutine>();

    [SerializeField] private int Count;

    /// <summary>
    /// Singleton beállítása. Ha már létezik másik példány, megsemmisítjük ezt.
    /// </summary>
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Különböző csoportigények kezelése coroutine-on keresztül
    /// </summary>
    private void Start()
    {
        StartCoroutine(HandleDifferentNeeds());
    }

    /// <summary>
    /// Minden frame-ben frissítjük a csoportok állapotát és ellenőrizzük a feloszlottakat
    /// </summary>
    private void Update()
    {
        Count = activeGroups.Count;

        List<Group> groupsToRemove = new();
        for (int i = activeGroups.Count - 1; i >= 0; i--)
        {
            activeGroups[i].CalculateGroupState();
            activeGroups[i].UpdateCircle();

            if (activeGroups[i].Members.Count == 0)
            {
                groupsToRemove.Add(activeGroups[i]);
            }
        }

        foreach (var group in groupsToRemove)
        {
            RemoveGroup(group);
        }

        CheckCollision();
    }

    /// <summary>
    /// Megpróbálunk egy állatot beléptetni egy meglévő csoportba.
    /// </summary>
    public bool EnterGroup(Group group, Animal animal)
    {
        return group.TryJoin(animal);
    }

    /// <summary>
    /// Új csoport létrehozása adott állatlistából.
    /// </summary>
    public void CreateNewGroup(List<Animal> animals)
    {
        var group = new Group(GroupState.Idle, animals, RADIUS);
        group.OnStateChanged += HandleGroupStateChanged;
        activeGroups.Add(group);
        group.ID = IDGenerator.GenerateID();
    }

    /// <summary>
    /// Egy csoport eltávolítása a rendszerből.
    /// </summary>
    public void RemoveGroup(Group group)
    {
        if (!activeGroups.Contains(group))
            return;

        group.OnStateChanged -= HandleGroupStateChanged;

        if (runningCoroutines.TryGetValue(group, out var coroutine))
        {
            StopCoroutine(coroutine);
            runningCoroutines.Remove(group);
        }

        activeGroups.Remove(group);
    }

    /// <summary>
    /// Két csoport összeolvasztása egy új csoporttá.
    /// </summary>
    private void MergeGroups(Group a, Group b)
    {
        var mergedMembers = new List<Animal>();
        mergedMembers.AddRange(a.Members);
        mergedMembers.AddRange(b.Members);

        RemoveGroup(a);
        RemoveGroup(b);

        CreateNewGroup(mergedMembers);
    }

    /// <summary>
    /// Meghatározza, hogy két csoport összeolvadhat-e (létszám, típus, állapot alapján).
    /// </summary>
    private bool CanMerge(Group groupA, Group groupB)
    {
        return (groupA.Members.Count + groupB.Members.Count <= 10 &&
                groupA.AnimalType == groupB.AnimalType &&
                groupA.State == groupB.State);
    }

    /// <summary>
    /// Csoportok közti térbeli ütközés vizsgálata, ha túl közel vannak → összeolvadás.
    /// </summary>
    public void CheckCollision()
    {
        for (int i = activeGroups.Count - 1; i >= 0; i--)
        {
            for (int j = activeGroups.Count - 1; j > i; j--)
            {
                Group groupA = activeGroups[i];
                Group groupB = activeGroups[j];

                if (Vector3.Distance(groupA.Position, groupB.Position) < RADIUS * 2)
                {
                    if (CanMerge(groupA, groupB))
                    {
                        MergeGroups(groupA, groupB);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Egy véletlenszerű pontot ad vissza a pályán, megadott távolságon belül, figyelembe véve a terep magasságát is.
    /// </summary>
    public Vector3 GetRandomPointOnMap(Vector3 origin, float maxDistance)
    {
        Vector2 offset2D = UnityEngine.Random.insideUnitCircle * maxDistance;
        Vector3 candidate = origin + new Vector3(offset2D.x, 0, offset2D.y);

        var terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            float y = terrain.SampleHeight(candidate) + terrain.GetPosition().y;
            candidate.y = y;
        }

        return candidate;
    }

    /// <summary>
    /// Kezeli azokat a csoportokat, ahol egyes tagok igénye eltér a többiekétől (pl. szomjas vs. éhes).
    /// Az ilyen tagokat kiválasztja és új csoportot hoz létre nekik.
    /// </summary>
    private IEnumerator HandleDifferentNeeds()
    {
        foreach (var group in activeGroups)
        {
            group.ToMove.ToList().ForEach(t => group.LeaveGroup(t));

            if (group.ToMove.Count >= 2)
            {
                CreateNewGroup(group.ToMove.ToList());
                group.ToMove.Clear();
            }
        }

        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// Reagál a csoport állapotának megváltozására.
    /// Leállítja a régi coroutine-t, és új keresést vagy célra indulást indít a csoport új állapota alapján.
    /// </summary>
    private void HandleGroupStateChanged(Group group)
    {
        if (runningCoroutines.TryGetValue(group, out var oldCoroutine))
        {
            StopCoroutine(oldCoroutine);
            runningCoroutines.Remove(group);
        }

        Coroutine newCoro = null;
        switch (group.State)
        {
            case GroupState.SearchingFood:
            case GroupState.SearchingWater:
                newCoro = StartCoroutine(group.StartGroupSearching());
                break;
            case GroupState.Hungry:
            case GroupState.Thirsty:
                newCoro = StartCoroutine(group.SetGroupTargeting());
                break;
            case GroupState.Idle:
            default:
                break;
        }

        if (newCoro != null)
            runningCoroutines[group] = newCoro;
    }
}
