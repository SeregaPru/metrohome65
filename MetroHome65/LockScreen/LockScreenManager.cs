using System;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines.File;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.LockScreen
{
    public class LockScreenManager: Canvas
    {
        private UIElement _lockScreen;

        public LockScreenManager()
        {
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            CreateLockScreen(mainSettings.LockScreenSettings.LockScreenClass);

            this.SizeChanged += (sender, args) => OnSizeChanged();

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);
        }

        private void CreateLockScreen(string lockScreenClass)
        {
            Clear();

            var pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            try
            {
                _lockScreen = pluginManager.CreateLockScreen(lockScreenClass) as UIElement;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, ex.Message);
                return;
            }

            _lockScreen.Location = new Point(0, 0);
            AddElement(_lockScreen);
        }

        private void OnSizeChanged()
        {
            if (_lockScreen == null) return;
            _lockScreen.Size = this.Size;
        }

        private void OnSettingsChanged(SettingsChangedMessage settingsChangedMessage)
        {
            try
            {
                if (settingsChangedMessage.PropertyName == "LockScreenClass")
                {
                    CreateLockScreen(settingsChangedMessage.Value as string);
                }

                if (settingsChangedMessage.PropertyName == "LockScreenSettings")
                {
                    if (_lockScreen == null) return;
                    (_lockScreen as ILockScreen).ApplySettings(settingsChangedMessage.Value as ILockScreenSettings);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, ex.Message);
            }
        }

    }
}
