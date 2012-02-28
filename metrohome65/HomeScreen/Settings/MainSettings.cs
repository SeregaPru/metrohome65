using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        public XmlColor ThemeColor
        {
            get { return _themeColor; }
            set { SetField(ref _themeColor, value, "ThemeColor"); }
        }
        private XmlColor _themeColor;

        /// <summary>
        /// Main screen background image. If not set, solid background of ThemeColor will be used.
        /// </summary>
        public string ThemeImage
        {
            get { return _themeImage; }
            set { SetField(ref _themeImage, value, "ThemeImage"); }
        }
        private string _themeImage;

        /// <summary>
        /// font color for items in program list
        /// </summary>
        public XmlColor FontColor
        {
            get { return _FontColor; }
            set { SetField(ref _FontColor, value, "FontColor"); }
        }
        private XmlColor _FontColor;

        /// <summary>
        /// default tile color
        /// </summary>
        public XmlColor TileColor
        {
            get { return _tileColor; }
            set { SetField(ref _tileColor, value, "TileColor"); }
        }
        private XmlColor _tileColor;


        public MainSettings()
        {
            ThemeColor = Color.Black;
            FontColor = Color.White;
            TileColor = Color.Blue;
        }


        #region INotifyPropertyChanged

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

    }
}
