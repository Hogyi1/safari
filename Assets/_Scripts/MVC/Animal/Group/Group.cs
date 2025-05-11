using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Group
{
    // === Állapot és típus ===
    public int ID;
    public GroupState State;
    public AnimalType AnimalType;

    // === Tagok és célpontok ===
    private List<Animal> members;
    private Vector3 center;
    private Vector3 Target = Vector3.zero;
    private Vector3 PreyTarget = Vector3.zero;
    public AnimalView Prey = null;
    public HashSet<Animal> ToMove = new();

    // === Források és események ===
    public HashSet<Vector3> waterSources;
    public HashSet<Vector3> foodSources;
    private bool sourceFound;
    public event Action<Group> OnStateChanged;

    // === Beállított sugarú kör sugara ===
    private float radius;

    public Group(GroupState state, List<Animal> members, float radius)
    {
        this.members = members;
        this.radius = radius;
        AnimalType = members[0].Model.Type;
        State = state;
        waterSources = new HashSet<Vector3>();
        foodSources = new HashSet<Vector3>();

        UpdateCircle();
        foreach (var member in members)
        {
            member.SetGroup(this);
            member.Brain.ReenterState();
        }
        InitializeSources();
    }

    // A csoportból minden View megérkezett
    public bool AllArrived() => members.TrueForAll(t => t.View.Arrived);
    public bool AllFinished() => members.TrueForAll(t => t.View.FinishedAnimation && !t.Model.IsConsuming);
    public Vector3 Position => center;
    public List<Animal> Members => members;

    // Belépés, ezt maga az állat fogja megpróbálni
    public bool TryJoin(Animal animal)
    {
        if (animal.Model.Type != AnimalType) return false;
        if (members.Count >= 5) return false;
        if (members.Contains(animal)) return false;
        if (State != GroupState.Idle) return false;

        members.Add(animal);
        animal.SetGroup(this);
        animal.Brain.ReenterState();
        return true;
    }

    // Kilépés
    public void LeaveGroup(Animal animal)
    {
        members.Remove(animal);
        if (!animal.Model.IsDead)
            animal.Brain.ReenterState();
        animal.SetGroup(null);
    }

    // Updateben mindig lefut, kör közepét állítod be
    public void UpdateCircle()
    {
        int count = members.Count;
        Vector3 averageCenter = Vector3.zero;
        foreach (var member in members)
        {
            averageCenter += member.View.transform.position;
        }
        averageCenter /= count;

        center = averageCenter;
    }

    // A körben egy random pont ami benne van
    public Vector3 GetRandomPointInCircle()
    {
        float Random01 = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));
        float r = radius * Random01;
        float theta = Random01 * 2 * Mathf.PI;

        float x = center.x + r * Mathf.Cos(theta);
        float z = center.z + r * Mathf.Sin(theta);

        return new Vector3(x, 1, z);
    }

    // Egy kis eltolás a körben
    public Vector3 GetRandomOffset(float max)
    {
        return new Vector3(UnityEngine.Random.Range(1, max), 0, UnityEngine.Random.Range(1, max));
    }

    // Beállítjuk a közös tudástárat
    private void InitializeSources()
    {
        waterSources.Clear();
        foodSources.Clear();

        foreach (var member in members)
        {
            waterSources.UnionWith(member.Model.waterSources);
            foodSources.UnionWith(member.Model.foodSources);
        }
    }

    // Updateben mindig lefut
    // Csoport szükségleteihez való alkalmazkodás
    public void CalculateGroupState()
    {
        if (members.Count == 1)
        {
            LeaveGroup(members[0]);
            return;
        }

        List<Animal> hungryAnimals = members.Where(m => m.Model.IsHungry).ToList();
        List<Animal> thirstyAnimals = members.Where(m => !m.Model.IsHungry && m.Model.IsThirsty).ToList();

        if (!hungryAnimals.Any() && !thirstyAnimals.Any() && AllFinished())
        {
            // Ha Idle-ben vannak akkor mindenre figyelnek gy mindig lehet új forrás, amint viszont szükségleteik lesznek
            // már nem figyelnek semmire csak abból gazdálkodnak ami a memóriájukban van és azt pörgetik végig a keresésnél,
            // ha viszont találnak egy a keresés alatt egy új Resource-ot akkor az bekerül a közös memóriába, és elindulnak oda, 
            // amennyiben még mindig nem volt elég nekik akkor újra kitörlődik és kezdik előlről a keresést. Egy préda a teljes csoportnak 100% élelmet ad.

            InitializeSources();
            SetState(GroupState.Idle);
            return;
        }
        else
        {
            if (hungryAnimals.Any() && thirstyAnimals.Any())
            {
                ToMove.AddRange(new List<Animal>(thirstyAnimals));
                foreach (var animal in ToMove)
                {
                    LeaveGroup(animal);
                }
                return;
            }

            if (State == GroupState.Hungry && (foodSources.Count == 0 && Prey.IsUnityNull()))
            {
                SetState(GroupState.SearchingFood);
                return;
            }
            else if (State == GroupState.Thirsty && waterSources.Count == 0)
            {
                SetState(GroupState.SearchingWater);
                return;
            }
            else if (hungryAnimals.Any() && State != GroupState.SearchingFood)
            {
                SetState(GroupState.Hungry);
                return;
            }
            else if (thirstyAnimals.Any() && State != GroupState.SearchingWater)
            {
                SetState(GroupState.Thirsty);
                return;
            }
        }
    }

    // Ha tényleges State változás történt akkor azt jelezzük
    private void SetState(GroupState newState)
    {
        if (newState != State)
        {
            State = newState;
            members.ForEach(t => t.Brain.ReenterState());
            OnStateChanged?.Invoke(this);
        }
    }

    // Jelenlegi Target lekérése
    public Vector3 GetCurrentTarget()
    {
        if (!Prey.IsUnityNull()) return Prey.transform.position;
        else return Target;

    }

    // Ha nincsen seholsem mentett víz vagy élelem forrás vagy már mindegyiket bejártuk
    // Addig fut ameddig nem találtunk valamilyen forrást legyen az préda vagy etető
    public IEnumerator StartGroupSearching()
    {

        sourceFound = false;
        members.ForEach(t => t.View.ResetMovement());

        while (!sourceFound)
        {
            Vector3 targetPosition = GroupManager.Instance.GetRandomPointOnMap(Position, radius); //GetRandomPointInCircle(); //
            // Random offset, csak a View kapja meg a targetet, így biztosítjuk a keresés State aktiválódását
            foreach (var t in members)
            {
                Vector3 offset = GetRandomOffset(5f);
                t.View.SetTarget(targetPosition + offset);
                t.Model.SetTarget(Vector3.zero);
            }

            HashSet<Vector3> activeSources = (State == GroupState.SearchingFood ? foodSources : waterSources);

            if (!Prey.IsUnityNull() || activeSources.Count != 0)
            {
                sourceFound = true;
                activeSources.Add(Target);
                if (State == GroupState.SearchingFood) SetState(GroupState.Hungry);
                else SetState(GroupState.Thirsty);
            }

            // Megvárjuk míg mindenki odaért
            yield return new WaitUntil(() => AllArrived());
        }

        // Ha találtak állatot vagy vizet vagy egyéb élelmet, hozzáadom a közös tudáshoz
    }

    // Ez csak akkor aktív, hogyha van mentett pozicio, tehat a vadaszas nem tartozik ide. Mivel az állatok mentik illetve frissítik a tudásukat,
    // ezért ha valahol egy üres etetővel találkoznak akkor azt elfelejtik, ugyanígy, hogyha a fa már nem létezik akkor is elfelejtik.
    public IEnumerator SetGroupTargeting()
    {
        HashSet<Vector3> activeSources = (State == GroupState.Hungry ? foodSources : waterSources);
        members.ForEach(t => t.View.ResetMovement());
        if (activeSources.Count == 0) yield return null;
        GroupState startingState = State;

        while (startingState == State)
        {
            if (Prey == null)
            {
                // Kiválasztjuk a legközelebbit a Setből
                float minDistance = 99999f;

                foreach (var source in activeSources)
                {
                    float distance = Vector3.Distance(center, source);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        Target = source;
                    }
                }

                // Kitöröljük, mert ide már elmentünk, így, mindig egy másik helyre megyünk el, illetve,
                // ha kimerítettük az összes lehetséges helyet akkor automatikusan átvált a CalculateGroupState miatt Searchingbe
                // ami, pedig megakasztja ezt a Coroutinet

                members.ForEach(t => t.SetTarget(Target));
                members.ForEach(m => m.Model.SetTarget(Target));
            }
            else
            {
                Vector3 target = Prey.transform.position;
                members.ForEach(t =>
                {
                    t.Model.SetPrey(Prey.Model);
                    t.Model.SetTarget(target);
                    t.View.Follow(Prey);
                });

                Target = PreyTarget;
            }

            // Megvárjuk míg mindenki odaér, itt manipulálhatjuk a későbbiekben ha van animációnk hozzá,
            // hogy várjuk meg az evést is, de jelenleg nincsen animációnk, illetve az evés is instant történik
            yield return new WaitUntil(() => AllArrived() && AllFinished());

            activeSources.Remove(Target);
        }

        members.ForEach(t => t.Model.ClearPrey());
        Prey = null;
    }

    // Related to Search
    // Az állatok szólnak a Group-nak ha találtak valamit
    public void SourceFound(Vector3 targetPosition)
    {
        Target = targetPosition;
    }

    // Related to Search
    // Az állatok szólnak a Group-nak ha találtak egy prédát
    public void PreyFound(AnimalView prey)
    {
        if (members[0].Model.Diet == DietType.Carnivore && Prey == null)
        {
            Prey = prey;
            PreyTarget = prey.transform.position;
            Target = prey.transform.position;
        }
    }

    public void UpdateSources()
    {
        InitializeSources();
    }
}

// === Lehetséges csoportállapotok ===
public enum GroupState
{
    Idle,
    SearchingFood,
    SearchingWater,
    Hungry,
    Thirsty,
}
