using UnityEngine;

public class VegetationView : MonoBehaviour, IInteractable, IStageable, IPlaceable
{
    [SerializeField] private BuildingData data;

    private Structure MySelectable;
    private bool isActive = false;

    // Fade effekt komponens, ami elhalványítja ha rávisszük az egeret
    private FadeEffect fadeEffect;
    private StageEffect stageEffect;

    private float MyStage = 0f;

    private void Update()
    {
        if (GetStage() != MyStage)
        {
            SetStage(GetStage());
            MyStage = GetStage();
        }
    }

    // Lekéri a komponenseket
    private void Awake()
    {
        fadeEffect = GetComponent<FadeEffect>();
        stageEffect = GetComponent<StageEffect>();
    }

    // Inicializálja a view-t a kapcsolódó modell adattal
    public void Init(Structure selectable)
    {
        this.MySelectable = selectable;
        isActive = false;
        fadeEffect.FadeOut();
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
        PopupManager.Instance.ActivateStructurePopup(((ISelectable)MySelectable).GetUIData(), GetGameObject());
    }

    // Interakció megszüntetése, állapot alaphelyzetbe (fade out)
    public void OnCancel()
    {
        isActive = false;
        fadeEffect.FadeOut();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // Visszaadja a modell azonosítóját
    public int GetID()
    {
        return MySelectable.GetID();
    }

    public BuildingType GetBuildingType()
    {
        return MySelectable.GetBuildingType();
    }

    public float GetStage()
    {
        if (MySelectable is Vegetation vegetation)
        {
            return vegetation.GetStage();
        }
        return 0;
    }

    public void SetStage(float stage)
    {
        stageEffect.SetMaterialsOpacity(stage);
    }

    public Structure GetStructure()
    {
        return MySelectable;
    }

    public BuildingData GetData()
    {
        return data;
    }
}
