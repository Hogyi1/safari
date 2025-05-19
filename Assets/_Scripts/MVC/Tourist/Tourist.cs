public class Tourist : ISaveable<TouristSaveData>
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

    public TouristSaveData GetSaveData()
    {
        return new TouristSaveData
        {
            ID = model.ID,
            State = model.State,
            WaitingMood = model.WaitingMood,
            TourMood = model.TourMood,
            TotalMood = model.TotalMood,
            ElapsedTime = model.ElapsedTime,
            PatienceLevel = model.PatienceLevel,
            VehicleID = model.VehicleID,
            FavouriteAnimal = model.FavouriteAnimalType,
            CurrentPosition = view.transform.position,
            CurrentDestination = view.CurrentDestination
        };
    }
}