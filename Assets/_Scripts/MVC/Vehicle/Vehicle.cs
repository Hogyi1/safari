using System.Collections.Generic;

public class Vehicle
{
    public int ID;
    private VehicleModel model;
    private VehicleView view;
    public VehicleModel Model => model;
    public VehicleView View => view;

    public Vehicle(int ID, VehicleModel model, VehicleView view)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;

        this.view.Init(this.model);
    }
}

