using TinyMessenger;

namespace MetroHome65.Interfaces.Events
{
    public class SettingsChangedMessage : ITinyMessage
    {
        public readonly string PropertyName;

        public readonly object Value;

        public object Sender { get { return null; } }

        public SettingsChangedMessage(string propertyName, object value)
        {
            PropertyName = propertyName;
            Value = value;
        }
    }
}
