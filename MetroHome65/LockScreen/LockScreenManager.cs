using System.Drawing;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.LockScreen
{
    public class LockScreenManager: Canvas
    {
        private UIElement _lockScreen;

        public LockScreenManager()
        {
            CreateLockScreen("MetroHome65.SimpleLock.SimpleLock");
            this.SizeChanged += (sender, args) => OnSizeChanged();

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);

        }

        private void CreateLockScreen(string lockScreenClass)
        {
            Clear();

            var pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            _lockScreen = pluginManager.CreateLockScreen(lockScreenClass) as UIElement;
            if (_lockScreen != null) return;

            _lockScreen.Location = new Point(0, 0);
            AddElement(_lockScreen);
        }

        private void OnSizeChanged()
        {
            _lockScreen.Size = this.Size;
        }

        private void OnSettingsChanged(SettingsChangedMessage settingsChangedMessage)
        {
            if (settingsChangedMessage.PropertyName == "LockScreenClass")
            {
                CreateLockScreen(settingsChangedMessage.Value as string);
            }

            if (settingsChangedMessage.PropertyName == "LockScreenSettings")
            {
                if (_lockScreen != null)
                    (_lockScreen as ILockScreen).ApplySettings(settingsChangedMessage.Value as ILockScreenSettings);
            }
        }

    }
}
