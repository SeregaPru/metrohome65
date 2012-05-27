using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
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
    public class MainSettings : INotifyPropertyChanged
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
        /// special background image for lock screen
        /// </summary>
        private string _lockScreenImage;
        public string LockScreenImage
        {
            get { return _lockScreenImage; }
            set { SetField(ref _lockScreenImage, value, "LockScreenImage"); }
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
            cloneSettings.LockScreenImage = mainSettings.LockScreenImage;

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

            if (mainSettings.LockScreenImage != this.LockScreenImage)
            {
                mainSettings.LockScreenImage = this.LockScreenImage;
                messenger.Publish(new SettingsChangedMessage("LockScreenImage"));
            }
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetField<T>(ref T field, T value, string propertyName)
        {
            try
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return;
                field = value;
                OnPropertyChanged(propertyName);
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion

    }
}
