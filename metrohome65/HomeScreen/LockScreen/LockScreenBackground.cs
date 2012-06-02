using MetroHome65.Controls;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces.Events;
using TinyIoC;

namespace MetroHome65.HomeScreen.LockScreen
{
    public class LockScreenBackground : ScaledBackground
    {
        protected override string GetImage()
        {
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            return mainSettings.LockScreenImage;
        }

        protected override void OnSettingsChanged(SettingsChangedMessage settingsChangedMessage)
        {
            if (settingsChangedMessage.PropertyName == "LockScreenImage")
            {
                SetImage(GetImage());
                Update();
            }
        }
    }
}
