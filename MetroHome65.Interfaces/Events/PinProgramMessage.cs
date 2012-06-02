using TinyMessenger;

namespace MetroHome65.Interfaces.Events
{
    /// <summary>
    /// message for notification to tiles grid to add new tile
    /// </summary>
    public class PinProgramMessage : ITinyMessage
    {
        public string Name;
        public string Path;

        public object Sender { get { return null; } }
    }
}
