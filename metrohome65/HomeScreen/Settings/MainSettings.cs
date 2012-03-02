using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
using MetroHome65.Routines;

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
        private XmlColor _tileColor;
        public XmlColor TileColor
        {
            get { return _tileColor; }
            set { SetField(ref _tileColor, value, "TileColor"); }
        }

        public MainSettings()
        {
            ThemeIsDark = (MetroTheme.PhoneBackgroundBrush != Color.White);
            ThemeImage = MetroTheme.PhoneBackgroundImage;
            TileColor = MetroTheme.PhoneAccentBrush;
        }

        public void ApplyTheme()
        {
            MetroTheme.PhoneForegroundBrush = (ThemeIsDark) ? Color.White : Color.Black;
            MetroTheme.PhoneBackgroundBrush = (ThemeIsDark) ? Color.Black : Color.White;
            MetroTheme.PhoneBackgroundImage = ThemeImage;
            MetroTheme.PhoneAccentBrush = TileColor;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

    }
}
