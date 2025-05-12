using UnityEngine;

public class Ranger
{

    public int ID;
    private RangerModel model;
    private RangerView view;
    public RangerModel Model => model;
    public RangerView View => view;


    public Ranger(int ID, RangerModel model, RangerView view)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;

        this.view.Init(this.model);
    }
}

