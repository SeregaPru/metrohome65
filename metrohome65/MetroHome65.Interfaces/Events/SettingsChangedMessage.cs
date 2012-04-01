using TinyMessenger;

namespace MetroHome65.Interfaces.Events
{
    public class SettingsChangedMessage : ITinyMessage
    {
        public SettingsChangedMessage(string propertyName)
        {
            PropertyName = propertyName;
        }

        public readonly string PropertyName;

        public object Sender { get { return null; } }
    }
}
