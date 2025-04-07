using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingView : MonoBehaviour
{
    [SerializeField]
    private Building MyBuilding;

    [SerializeField]
    public bool isHovered, isActive;

    private FadeEffect fadeEffect;

    private void Awake()
    {
        fadeEffect = GetComponent<FadeEffect>();
    }

    public void Init(Building building)
    {
        this.MyBuilding = building;
        isHovered = false;
        isActive = false;
    }

    private void Update()
    {
        if (!isActive)
        {
            if (isHovered && !fadeEffect.IsFaded)
            {
                fadeEffect.FadeIn();
            }
            else if (!isHovered && fadeEffect.IsFaded)
            {
                fadeEffect.FadeOut();
            }
        }
    }

    public int GetID()
    {
        return MyBuilding.GetID();
    }

    public BuildingType GetBuildingType()
    {
        return MyBuilding.GetBuildingType();
    }
}

