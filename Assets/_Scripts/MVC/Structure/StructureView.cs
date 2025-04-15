using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.VisualScripting;
using UnityEngine;

public class StructureView : MonoBehaviour, IInteractable, IPlaceable
{
    public Structure MySelectable;

    private bool isActive;

    // Fade effekt komponens, ami elhalványítja ha rávisszük az egeret
    private FadeEffect fadeEffect;

    // Lekéri a komponenseket
    private void Awake()
    {
        fadeEffect = GetComponent<FadeEffect>();
    }

    // Inicializálja a view-t a kapcsolódó modell adattal
    public void Init(Structure selectable)
    {
        this.MySelectable = selectable;
        isActive = false;
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
}

