/// <summary>
/// Represents a tourist entity in the simulation, including model, view, and save logic.
/// </summary>
public class Tourist : ISaveable<TouristSaveData>
{
    public int ID;

    // Logical state and properties of the tourist
    private TouristModel model;

    // Visual representation and movement logic
    private TouristView view;

    // Public accessors
    public TouristModel Model => model;
    public TouristView View => view;

    /// <summary>
    /// Indicates whether this tourist can be safely removed from the simulation.
    /// </summary>
    public bool CanRemove => model.State == TouristState.Finished;

    /// <summary>
    /// Constructor connects the tourist's ID, model and view. Initializes the view.
    /// </summary>
    public Tourist(int ID, TouristModel model, TouristView view)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;

        this.view.Init(this.model);
    }

    /// <summary>
    /// Creates a serializable save data object from the tourist's current state.
    /// </summary>
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
