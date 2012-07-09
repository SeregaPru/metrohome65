using System;
using System.Drawing;
using Fleux.Styles;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen.Settings
{
    /// <summary>
    /// Common settings for UI 
    /// </summary>
    [Serializable]
    public class MainSettings : CustomSettings
    {
        /// <summary>
        /// Main screen background color
        /// </summary>
        private bool _themeIsDark;
        public bool ThemeIsDark
        {
            get { return _themeIsDark; }
            set { SetField(ref _themeIsDark, value, "ThemeIsDark"); }
        }

        /// <summary>
        /// Main screen background image. If not set, solid background of ThemeColor will be used.
        /// </summary>
        private string _themeImage;
        public string ThemeImage
        {
            get { return _themeImage; }
            set { SetField(ref _themeImage, value, "ThemeImage"); }
        }

        /// <summary>
        /// default tile color
        /// </summary>
        private XmlColor _accentColor;
        public XmlColor AccentColor
        {
            get { return _accentColor; }
            set { SetField(ref _accentColor, value, "AccentColor"); }
        }


        /// <summary>
        /// ...
        /// </summary>
        private string _lockScreenClass;
        public string LockScreenClass
        {
            get { return _lockScreenClass; }
            set { SetField(ref _lockScreenClass, value, "LockScreenClass"); }
        }

        /// <summary>
        /// ...
        /// </summary>
        private object _lockScreenSettings;
        public object LockScreenSettings
        {
            get { return _lockScreenSettings; }
            set { SetField(ref _lockScreenSettings, value, "LockScreenSettings"); }
        }


        public MainSettings()
        {
            ThemeIsDark = (MetroTheme.PhoneBackgroundBrush != Color.White);
            AccentColor = MetroTheme.PhoneAccentBrush;
        }

        public static MainSettings Clone()
        {
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            var cloneSettings = new MainSettings();

            // setting additional properties that are not stored in main theme
            cloneSettings.ThemeImage = mainSettings.ThemeImage;

            cloneSettings.LockScreenClass = mainSettings.LockScreenClass;

            return cloneSettings;
        }

        public void ApplyTheme()
        {
            MetroTheme.PhoneForegroundBrush = (ThemeIsDark) ? Color.White : Color.Black;
            MetroTheme.PhoneBackgroundBrush = (ThemeIsDark) ? Color.Black : Color.White;
            MetroTheme.PhoneAccentBrush = AccentColor;

            NotifyChangedSettings();
        }

        // notify about new settings
        private void NotifyChangedSettings()
        {
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();

            if (mainSettings.ThemeIsDark != this.ThemeIsDark)
            {
                mainSettings.ThemeIsDark = this.ThemeIsDark;
                messenger.Publish(new SettingsChangedMessage("ThemeIsDark"));
            }

            if (mainSettings.AccentColor != this.AccentColor)
            {
                mainSettings.AccentColor = this.AccentColor;
                messenger.Publish(new SettingsChangedMessage("AccentColor"));
            }

            if (mainSettings.ThemeImage != this.ThemeImage)
            {
                mainSettings.ThemeImage = this.ThemeImage;
                messenger.Publish(new SettingsChangedMessage("ThemeImage"));
            }

            // lock screen settings
            if (mainSettings.LockScreenClass != this.LockScreenClass)
            {
                mainSettings.LockScreenClass = this.LockScreenClass;
                messenger.Publish(new SettingsChangedMessage("LockScreenClass"));
            }
        }

    }
}
