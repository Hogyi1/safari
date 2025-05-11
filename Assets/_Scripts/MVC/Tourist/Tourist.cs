using System.Collections.Generic;

public class Tourist
{
    public int ID;
    private TouristModel model;
    private TouristView view;
    public TouristModel Model => model;
    public TouristView View => view;
    public bool CanRemove => model.State == TouristState.Finished;


    public Tourist(int ID, TouristModel model, TouristView view)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;

        this.view.Init(this.model);
    }
}