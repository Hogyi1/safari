using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(UpgradeEffect))]
[RequireComponent(typeof(FadeEffect))]
public class FacilityView : MonoBehaviour, IPlaceable, IInteractable, IUpgradeable, IHasInteractingPosition
{
    [SerializeField] private BuildingData data; //ScriptableObject vagy adatmodell, amiben a szintek, árak tárolódnak
    private Structure MySelectable;                     // A hozzá tartozó modell
    private bool isActive;
    private FadeEffect fadeEffect;
    private UpgradeEffect upgradeEffect;

    private void Awake()
    {
        fadeEffect = GetComponent<FadeEffect>();
        if (fadeEffect.IsUnityNull())
            fadeEffect = gameObject.AddComponent<FadeEffect>();
        upgradeEffect = gameObject.AddComponent<UpgradeEffect>();
        if (upgradeEffect.IsUnityNull())
            upgradeEffect = gameObject.AddComponent<UpgradeEffect>();
    }

    public void Init(Structure selectable)
    {
        MySelectable = selectable;
        isActive = false;
    }

    public void LevelUp(int amount)
    {
        Facility facility = (Facility)MySelectable;
        int level = facility.GetCurrentLevel();
        upgradeEffect.RefreshView(level);
    }

    public void LevelDown(int amount)
    {
        Facility facility = (Facility)MySelectable;
        int level = facility.GetCurrentLevel();
        upgradeEffect.RefreshView(level);
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
        PopupManager.Instance.ActivatePopup(((ISelectable)MySelectable).GetUIData(), GetGameObject());
    }

    // Interakció megszüntetése, állapot alaphelyzetbe (fade out)
    public void OnCancel()
    {
        isActive = false;
        fadeEffect.FadeOut();
    }

    public GameObject GetGameObject() => gameObject;
    public int GetID() => MySelectable.GetID();
    public BuildingType GetBuildingType() => MySelectable.GetBuildingType();
    public Structure GetStructure() => MySelectable;
    public BuildingData GetData() => data;

    public Vector3 GetInteractingPosition()
    {
        Vector3 pos;
        try
        {
            pos = gameObject.transform.Find("InteractPoint").position;
        }
        catch (Exception)
        {
            Debug.LogWarning("Nincsen beállítva InteractPoint az alap beállításokat fogom használni");
            pos = gameObject.GetComponent<Renderer>().bounds.center;
        }
        return pos;
    }
}

