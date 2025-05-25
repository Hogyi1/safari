using System.Collections.Generic;
using System;
using UnityEngine;
using static UIKeys;

public class Animal : ISelectable
{
    public int ID;
    public bool CanRemove;
    public bool CanLeave;
    private AnimalModel model;
    private AnimalView view;
    private AnimalStateMachine brain;
    private Group group;
    public AnimalModel Model => model;
    public AnimalView View => view;
    public AnimalStateMachine Brain => brain;
    public Group Group => group;

    public Animal(int ID, AnimalModel model, AnimalView view, AnimalStateMachine brain)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;
        this.brain = brain;
        this.group = null;
        CanRemove = false;
        CanLeave = false;

        this.brain.Init(this);
        this.view.Init(this.model);
    }

    public Animal(int ID, AnimalModel model, AnimalView view, AnimalStateMachine brain, Group group)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;
        this.brain = brain;
        this.group = group;
        CanRemove = false;
        CanLeave = false;

        this.brain.Init(this);
        this.view.Init(this.model);
    }

    public void SetTarget(Vector3 target)
    {
        model.SetTarget(target);
        view.SetTarget(target);
    }

    public void SetGroup(Group newGroup)
    {
        if (group != null && group != newGroup)
            Debug.Log("Vagyok már egy groupban");

        if (GroupManager.Instance.Contains(newGroup))
            group = newGroup;
    }

    public Dictionary<UIKeys, object> GetUIData()
    {
        return new Dictionary<UIKeys, object> {
            { Name_text, Model.Type.ToString() },
            { Sprite_icon, Model.Icon },
            { Animalmood_text, new Func<string>(() => brain.RootState.ToString()) },
            { Animaltype_text, new Func<string>(() => Model.Diet.ToString()) },
            { Animalage_text, new Func<string>(() => Model.Age.ToString()) },
            { Value_slider, new Func<float>(() => Model.Hp) },
            { MaxValue_slider, 100f },
            { Healthbar, true },
            { Hunt_action, new Action(() => { RangerManager.Instance.HuntDownAnimal(ID); })},
            { Hunt_interact, new Func<bool>(() => RangerManager.Instance.CanHunt(ID) && !Model.IsDead) }
        };
    }

    public BuildingType GetBuildingType() => BuildingType.None;

    public int GetID() => ID;
}

