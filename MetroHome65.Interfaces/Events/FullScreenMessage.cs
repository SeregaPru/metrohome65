using TinyMessenger;

namespace MetroHome65.Interfaces.Events
{
    public class FullScreenMessage : ITinyMessage
    {
        public readonly bool FullScreen;
        public object Sender { get { return null; } }

        public FullScreenMessage(bool fullScreen)
        {
            FullScreen = fullScreen;
        }
    }
}