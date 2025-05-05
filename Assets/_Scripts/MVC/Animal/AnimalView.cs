using System.Collections.Generic;
using System;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class AnimalView : MonoBehaviour, INavigatable, IInteractable
{
    // === Állat azonosító és modell ===
    private int iD;
    private AnimalModel model;
    private bool isActive = false;

    // === Komponensek ===
    [SerializeField] private Animator animator;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private NavigatorComponent navigator;
    [SerializeField] private FadeEffect fadeEffect;

    // === Életkor alapján skála-értékek ===
    private readonly Dictionary<int, Vector3> ageScales = new Dictionary<int, Vector3>
    {
        [1] = Vector3.one * 0.5f,
        [2] = Vector3.one * 0.7f,
        [3] = Vector3.one
    };

    // === Detektált colliderek nyilvántartása ===
    private readonly HashSet<Collider> _inside = new HashSet<Collider>();

    [Header("Detection Layers")]
    public LayerMask selectableMask;  // Étel/építmény réteg
    public LayerMask waterMask;       // Vízforrás réteg
    public LayerMask animalMask;      // Más állatok rétege

    [Header("Animations")]
    // Animator paraméterek (hash-ek)
    public static readonly int IsWalking = Animator.StringToHash("IsWalking");
    public static readonly int IsRunning = Animator.StringToHash("IsRunning");
    public static readonly int IsEating = Animator.StringToHash("IsEating");
    public static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    public static readonly int IsDrinking = Animator.StringToHash("IsDrinking");
    public static readonly int IsSleeping = Animator.StringToHash("IsSleeping");

    // === Események más rendszerek számára ===
    public event Action<IFoodSource, Vector3> OnFoodSourceFound;
    public event Action<IWaterSource, Vector3> OnWaterSourceFound;
    public event Action<AnimalView> OnAnimalFound;

    // === Aktuális célpont, amit követ az állat ===
    private Vector3 currentDestination;

    // === Egyszerűsített publikus hozzáférések ===
    public Animator Animator => animator;
    public int ID => iD;
    public AnimalModel Model => model;
    public NavMeshAgent Agent => navigator.Agent;

    void Awake()
    {
        // Collider triggerként működjön (átjárható érzékelő)
        sphereCollider.isTrigger = true;

        // Ha nincs navigator hozzárendelve, megkeressük
        if (navigator == null)
            navigator = GetComponent<NavigatorComponent>();
        if (fadeEffect == null)
            fadeEffect = GetComponent<FadeEffect>();
    }

    // === Inicializálás modell alapján ===
    public void Init(AnimalModel model)
    {
        this.model = model;
        iD = model.ID;
        AdvanceAge(); // Életkor alapján skálázás
    }

    // === Méretnövelés életkor alapján ===
    public void AdvanceAge()
    {
        if (!ageScales.TryGetValue(model.Age, out var targetScale))
            return;

        StartCoroutine(ScaleOverTime(targetScale, 3f));
    }

    // === Skálázás animáltan adott idő alatt ===
    private IEnumerator ScaleOverTime(Vector3 targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    // === INavigatable metódusok delegálása ===
    public void SetTarget(Vector3 dest) => navigator.SetTarget(dest);
    public void SetWayPoints(List<Vector3> wp) => navigator.SetWayPoints(wp);
    public void Follow(MonoBehaviour t)
    {
        navigator.Follow(t);
        model.SetTarget(t.transform.position);
    }

    public void StopMovementInstantly() => navigator.StopMovementInstantly();
    public void StopMovement() => navigator.StopMovement();
    public void ResetMovement() => navigator.ResetMovement();

    // === Érkezés logika ===
    public bool Arrived => navigator.Arrived;

    // === Animáció befejezésének ellenőrzése ===
    public bool FinishedAnimation
    {
        get
        {
            if (!navigator.Agent.enabled) return true;
            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (animator.IsInTransition(0)) return false;
            return state.normalizedTime >= 1f;
        }
    }

    // === Visszaad egy random pozíciót a Collider-en belül, és be is állítja célként ===
    public Vector3 GetSetRandomPosition()
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * sphereCollider.radius;
        randomDir += transform.position;
        NavMesh.SamplePosition(randomDir, out NavMeshHit hit, sphereCollider.radius, NavMesh.AllAreas);
        SetTarget(hit.position);
        return hit.position;
    }

    // === Collider trigger belépés – új objektum detektálása ===
    private void OnTriggerEnter(Collider other)
    {
        if (_inside.Add(other))
            ProcessDetection(other);
    }

    // === Collider trigger kilépés ===
    private void OnTriggerExit(Collider other)
    {
        _inside.Remove(other);
    }

    // === Érzékelt Collider feldolgozása ===
    private void ProcessDetection(Collider other)
    {
        var go = other.gameObject;
        int layer = go.layer;

        // === Étel vagy víz detektálása ===
        if ((selectableMask & (1 << layer)) != 0)
        {
            var placeable = other.GetComponentInParent<IPlaceable>();
            if (placeable != null)
            {
                var structure = placeable.GetStructure();

                // Pozíció lekerekítve
                Vector3 rawPos = placeable.GetGameObject().transform.position;
                Vector3 roundedPos = new Vector3(
                    rawPos.x,
                    Mathf.Round(rawPos.y),
                    rawPos.z
                );

                if (structure is IFoodSource food && model.Diet == food.GetDietType())
                    OnFoodSourceFound?.Invoke(food, roundedPos);
                else if (structure is IWaterSource water)
                    OnWaterSourceFound?.Invoke(water, roundedPos);
            }
        }
        // === Másik állat érzékelése ===
        else if ((animalMask & (1 << layer)) != 0)
        {
            var av = go.GetComponent<AnimalView>();
            if (av != null && av.enabled)
                OnAnimalFound?.Invoke(av);
        }
    }

    // === Környezeti újraérzékelés, pl. indulás után ===
    public void RefreshDetection()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            sphereCollider.radius,
            selectableMask | waterMask | animalMask);

        foreach (var hit in hits)
        {
            if (hit == sphereCollider)
                continue;

            if (hit.attachedRigidbody == this.GetComponent<Rigidbody>())
                continue;

            ProcessDetection(hit);
        }
    }

    // Egér rámutatás esemény kezelése (fade in effekt)
    public void OnHover()
    {
        fadeEffect.FadeIn();
    }

    // Egér elhagyás esemény, ha nem aktív (fade out)
    public void OnExit()
    {
        if (!isActive)
        {
            fadeEffect.FadeOut();
        }
    }

    // Kattintás vagy aktiválás kezelése (fade in)
    public void OnAction()
    {
        isActive = true;
        fadeEffect.FadeIn();
        PopupManager.Instance.ActivateAnimalPopup(iD);
    }

    // Interakció megszüntetése, állapot alaphelyzetbe (fade out)
    public void OnCancel()
    {
        isActive = false;
        fadeEffect.FadeOut();
    }
}


public enum ColliderTrigger
{
    None,
    Food,
    Prey,
    Mate,
    Water
}