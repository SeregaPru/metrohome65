using Fleux.Controls;
using TinyMessenger;

namespace MetroHome65.Interfaces.Events
{
    /// <summary>
    /// message for notification main screen to show new page
    /// </summary>
    public class ShowPageMessage : ITinyMessage
    {
        public ShowPageMessage(FleuxControlPage page)
        {
            Page = page;
        }

        public readonly FleuxControlPage Page;

        public object Sender { get { return null; } }
    }
}
