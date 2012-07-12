using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces.Events;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.Controls
{
    public class ThemedBackground : Routines.UIControls.ScaledBackground
    {
        public ThemedBackground() : base("")
        {
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);
        }

        protected override string GetImage()
        {
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            return mainSettings.ThemeImage;
        }

        protected virtual void OnSettingsChanged(SettingsChangedMessage settingsChangedMessage)
        {
            if (settingsChangedMessage.PropertyName == "ThemeImage")
            {
                SetImage(GetImage());
                Update();
            }
        }
    }
}