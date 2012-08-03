using System;
using System.Drawing;
using Fleux.Styles;
using MetroHome65.Interfaces.Events;
using MetroHome65.LockScreen;
using MetroHome65.Routines;
using MetroHome65.Routines.Settings;
using MetroHome65.Tile;
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
        /// Theme style - light / dark - controls main font and background colors
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
        /// style for tile screen. 
        /// WP7 style - tiles with paddning and right arrow. 
        /// WP8 - tiles fit full screen without paddings and without arrow.
        /// </summary>
        private int _tileThemeIndex;
        public int TileThemeIndex
        {
            get { return _tileThemeIndex; }
            set
            {
                SetField(ref _tileThemeIndex, value, "TileThemeIndex");
                _tileTheme = (_tileThemeIndex == 0) ? (TileTheme)new TileThemeWP7() : new TileThemeWindows8();
            }
        }

        // приходится испльзовать метод вместо поля, чтобы этот объект не сериализовался в файл настроек
        private TileTheme _tileTheme;
        public TileTheme GetTileTheme() { return _tileTheme; }

        /// <summary>
        /// collection of various settings for lock screen.
        /// </summary>
        private LockScreenSettings _lockScreenSettings;

        public LockScreenSettings LockScreenSettings
        {
            get { return _lockScreenSettings ?? (_lockScreenSettings = new LockScreenSettings()); }
            set { SetField(ref _lockScreenSettings, value, "LockScreenSettings"); }
        }

        private bool _fullScreen;
        public bool FullScreen
        {
            get { return _fullScreen; }
            set { SetField(ref _fullScreen, value, "FullScreen"); }
        }


        public MainSettings()
        {
            ThemeIsDark = (MetroTheme.PhoneBackgroundBrush != Color.White);
            AccentColor = MetroTheme.PhoneAccentBrush;

            TileThemeIndex = 0;
            _tileTheme = new TileThemeWP7();
        }

        public static MainSettings Clone()
        {
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            var cloneSettings = new MainSettings
                                    {
                                        // set additional properties that are not stored in main theme
                                        ThemeImage = mainSettings.ThemeImage,
                                        LockScreenSettings = mainSettings.LockScreenSettings
                                    };

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
                messenger.Publish(new SettingsChangedMessage("ThemeIsDark", this.ThemeIsDark));
            }

            if (mainSettings.AccentColor != this.AccentColor)
            {
                mainSettings.AccentColor = this.AccentColor;
                messenger.Publish(new SettingsChangedMessage("AccentColor", this.AccentColor));
            }

            if (mainSettings.ThemeImage != this.ThemeImage)
            {
                mainSettings.ThemeImage = this.ThemeImage;
                messenger.Publish(new SettingsChangedMessage("ThemeImage", this.ThemeImage));
            }

            if (mainSettings.TileThemeIndex != this.TileThemeIndex)
            {
                mainSettings.TileThemeIndex = this.TileThemeIndex;
                messenger.Publish(new SettingsChangedMessage("TileTheme", this.GetTileTheme()));
            }

            if (mainSettings.FullScreen != this.FullScreen)
            {
                mainSettings.FullScreen = this.FullScreen;
                messenger.Publish(new FullScreenMessage(this.FullScreen));
            }

        }

    }
}
