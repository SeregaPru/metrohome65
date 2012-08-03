using System;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines.File;
using MetroHome65.Routines.Settings;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.LockScreen
{
    public class LockScreenManager : Canvas, IActive, IVisible
    {
        private UIElement _lockScreen;

        public LockScreenManager()
        {
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            CreateLockScreen(mainSettings.LockScreenSettings.LockScreenClass);

            var settings = CreateSettings(_lockScreen.GetType(), mainSettings.LockScreenSettings);
            (_lockScreen as ILockScreen).ApplySettings(settings);

            this.SizeChanged += (sender, args) => OnSizeChanged();

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);
        }

        private void CreateLockScreen(string lockScreenClass)
        {
            Clear();

            var pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            _lockScreen = pluginManager.CreateLockScreen(lockScreenClass) as UIElement;
            if (_lockScreen == null) return;

            _lockScreen.Location = new Point(0, 0);
            AddElement(_lockScreen);
        }

        public static ILockScreenSettings CreateSettings(Type lockScreenType, LockScreenSettings storedSettings)
        {
            Type settingsType = null;
            var props = lockScreenType.GetProperties();
            foreach (var srcPropInfo in props)
            {
                // set only properties that are marked as lockscreen setings
                var attributes = srcPropInfo.GetCustomAttributes(typeof(LockScreenSettingsAttribute), true);
                if (attributes.Length > 0)
                {
                    settingsType = srcPropInfo.PropertyType;
                    break;
                }
            }

            // create empty settings object for selected lockscreen type
            if (settingsType == null) return null;

            var settings = Activator.CreateInstance(settingsType);
            var lockScreenSettings = settings as ILockScreenSettings;
            if (lockScreenSettings == null) return null;

            // fill new settings properties with according properties from old settings
            StoredSettingsHelper.StoredSettingsToObject(storedSettings.Parameters,
                lockScreenSettings, typeof(LockScreenParameterAttribute));

            return lockScreenSettings;
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

        // IActive
        public bool Active
        {
            get { return ((_lockScreen != null) && (_lockScreen is IActive) && ((_lockScreen as IActive).Active)); }
            set {
                if ((_lockScreen != null) && (_lockScreen is IActive))
                    (_lockScreen as IActive).Active = value;
            }
        }

        // IVisible
        public bool Visible
        {
            get { return ((_lockScreen != null) && (_lockScreen is IVisible) && ((_lockScreen as IVisible).Visible)); }
            set
            {
                if ((_lockScreen != null) && (_lockScreen is IVisible))
                    (_lockScreen as IVisible).Visible = value;

                SwitchFullScreen(value);
            }
        }

        private void SwitchFullScreen(bool fullScreen)
        {
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Publish(new FullScreenMessage(fullScreen));
        }
    }
}
