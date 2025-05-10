using Unity.VisualScripting;
using UnityEngine;

public class Animal
{
    public int ID;
    public bool CanRemove;
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
        if (newGroup == null)
            group = null;
        else
            group = newGroup;
    }
}

