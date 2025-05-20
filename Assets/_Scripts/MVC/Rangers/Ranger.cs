/// <summary>
/// Represents a complete Ranger unit, combining its model (logic/state)
/// and view (visual representation). Acts as the bridge between the two.
/// </summary>
public class Ranger
{
    /// <summary>
    /// Unique identifier for the ranger.
    /// </summary>
    public int ID;

    /// <summary>
    /// The internal logic and state of the ranger (energy, state, prey).
    /// </summary>
    private RangerModel model;

    /// <summary>
    /// The visual representation of the ranger in the scene.
    /// </summary>
    private RangerView view;

    /// <summary>
    /// Public accessor for the ranger's model.
    /// </summary>
    public RangerModel Model => model;

    /// <summary>
    /// Public accessor for the ranger's view.
    /// </summary>
    public RangerView View => view;

    /// <summary>
    /// Constructs a new Ranger entity from a model and view.
    /// Initializes the view with the model reference.
    /// </summary>
    /// <param name="ID">Unique ID of the ranger.</param>
    /// <param name="model">The model containing logic and state.</param>
    /// <param name="view">The view representing the ranger visually.</param>
    public Ranger(int ID, RangerModel model, RangerView view)
    {
        this.ID = ID;
        this.model = model;
        this.view = view;

        this.view.Init(this.model);
    }
}
