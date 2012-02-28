namespace MetroHome65.Widgets
{
    public delegate void UpdateEventHandler();


    public interface IActive
    {
        bool Active { get; set; }
    }

    /// <summary>
    /// Additional interface for elements which may update itself
    /// </summary>
    public interface IUpdatable : IActive
    {
        /// <summary>
        /// Event raised when Widget needs to be updated (repainted)
        /// </summary>
        event UpdateEventHandler OnUpdate;

    }
}
