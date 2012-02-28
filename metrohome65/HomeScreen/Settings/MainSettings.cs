using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
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
        public XmlColor ThemeColor { get; set; }

        /// <summary>
        /// Main screen background image. If not set, solid background of ThemeColor will be used.
        /// </summary>
        public string ThemeImage { get; set; }

        /// <summary>
        /// font color for items in program list
        /// </summary>
        public XmlColor ListFontColor { get; set; }

        /// <summary>
        /// default tile color
        /// </summary>
        public XmlColor TileColor { get; set; }


        public MainSettings()
        {
            ThemeColor = Color.Black;
            ListFontColor = Color.White;
            TileColor = Color.Blue;
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

    }


    public class MainSettingsProvider
    {
        private MainSettings _settings;

        public MainSettings Settings
        {
            get
            {
                if (_settings == null)
                    ReadSettings();
                return _settings;
            } 
        }

        /// <summary>
        /// Read settings from XML file
        /// </summary>
        private void ReadSettings()
        {
            try
            {
                _settings = new MainSettings();

                var serializer = new XmlSerializer(_settings.GetType());
                System.IO.TextReader reader = new System.IO.StreamReader(SettingsFile());
                _settings = (MainSettings)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Read main settings error");
                // return default settings
                _settings = new MainSettings();
            }
        }

        public void WriteSettings(MainSettings settings)
        {
            try
            {
                var serializer = new XmlSerializer(settings.GetType());
                System.IO.TextWriter writer = new System.IO.StreamWriter(SettingsFile(), false);
                serializer.Serialize(writer, settings);
                writer.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Write main settings error");
            }
        }

        private String SettingsFile() { return FileRoutines.CoreDir + "\\settings.xml"; }

    }
}
