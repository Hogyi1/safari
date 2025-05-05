using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class RoadView : MonoBehaviour, IInteractable, IPlaceable
{
    private int ID;
    private bool isActive = false;
    [SerializeField] private Sprite Icon;
    [SerializeField] private FadeEffect fadeEffect;

    public Dictionary<StructureUIValues, object> GetUIData()
    {
        return new Dictionary<StructureUIValues, object> {
            { StructureUIValues.Name_text, "Road" },
            { StructureUIValues.ID, ID },
            { StructureUIValues.Sprite_icon, Icon },
        };
    }
    public BuildingType GetBuildingType()
    {
        return BuildingType.Road;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public int GetID()
    {
        return ID;
    }

    public void SetID(int ID)
    {
        this.ID = ID;
    }

    public void Init(Structure structure)
    {
        return;
    }

    public Structure GetStructure()
    {
        return null;
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
        PopupManager.Instance.ActivateStructurePopup(GetUIData(), GetGameObject());
    }

    // Interakció megszüntetése, állapot alaphelyzetbe (fade out)
    public void OnCancel()
    {
        isActive = false;
        fadeEffect.FadeOut();
    }
}
