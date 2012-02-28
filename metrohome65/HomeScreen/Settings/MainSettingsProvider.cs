using System;
using System.Xml.Serialization;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen.Settings
{
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