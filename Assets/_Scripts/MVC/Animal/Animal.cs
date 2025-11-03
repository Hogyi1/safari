using System.Collections.Generic;
using System;
using UnityEngine;
using static UIKeys;

/// <summary>
/// Represents the high-level wrapper of an animal entity, tying together its model, view, and state machine.
/// Provides access to UI data and group logic.
/// </summary>
public class Animal : ISelectable
{
    /// <summary> Unique ID of the animal. </summary>
    public int ID { get; private set; }

    /// <summary> Determines whether the animal can be removed (e.g. hunted or deleted). </summary>
    public bool CanRemove = false;

    /// <summary> True if the animal is currently part of a group. </summary>
    public bool InGroup => GroupID > 0;

    /// <summary> The ID of the group this animal belongs to, or -1 if none. </summary>
    public int GroupID { get; private set; }

    private AnimalModel model;
    private AnimalView view;
    private AnimalStateMachine brain;

    /// <summary> Logic/data model containing physiological state and memory. </summary>
    public AnimalModel Model => model;

    /// <summary> The visual and behavioral Unity component. </summary>
    public AnimalView View => view;

    /// <summary> The state machine controlling this animal's behavior. </summary>
    public AnimalStateMachine Brain => brain;

    /// <summary> Event fired when the animal joins a new group. </summary>
    public event Action OnGroupChange;

    /// <summary>
    /// Constructs a new Animal object and links its components.
    /// </summary>
    public Animal(int ID, AnimalModel model, AnimalView view, AnimalStateMachine brain)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;
        this.brain = brain;
        GroupID = -1;

        this.brain.Init(this);
        this.view.Init(this.model);
    }

    /// <summary>
    /// Sets the movement target for both the model and view.
    /// </summary>
    public void SetTarget(Vector3 target)
    {
        model.SetTarget(target);
        view.SetTarget(target);
    }

    /// <summary>
    /// Returns structured UI data representing the animal’s status and interaction options.
    /// Used to populate popup panels or sidebars.
    /// </summary>
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
            { Hunt_action, new Action(() => { RangerManager.Instance.HuntDownAnimal(ID); }) },
            { Hunt_interact, new Func<bool>(() => RangerManager.Instance.CanHunt(ID) && !Model.IsDead) }
        };
    }

    /// <summary>
    /// Updates the animal’s group ID and triggers group change event if applicable.
    /// </summary>
    public void SetGroup(int groupID)
    {
        if (GroupID != groupID && groupID != -1)
            OnGroupChange?.Invoke();
        GroupID = groupID;
        Debug.Log("I joined the group: " + groupID);
    }

    /// <summary>
    /// Returns the type of building the animal is associated with (always None).
    /// </summary>
    public BuildingType GetBuildingType() => BuildingType.None;

    /// <summary>
    /// Returns the unique ID of this animal (for interface compatibility).
    /// </summary>
    public int GetID() => ID;

    /// <summary>
    /// Returns the current world position of the animal.
    /// </summary>
    public Vector3 GetPosition() => View.transform.position;
}
