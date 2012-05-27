using System;
using System.Xml.Serialization;
using MetroHome65.Routines;
using TinyIoC;

namespace MetroHome65.HomeScreen.Settings
{
    public class MainSettingsProvider
    {
        /// <summary>
        /// Read settings from XML file
        /// </summary>
        public void ReadSettings()
        {
            try
            {
                var settings = new MainSettings();

                TinyIoCContainer.Current.Register<MainSettings>(settings);

                var serializer = new XmlSerializer(settings.GetType());
                System.IO.TextReader reader = new System.IO.StreamReader(SettingsFile());
                settings = (MainSettings)serializer.Deserialize(reader);
                reader.Close();

                settings.ApplyTheme();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Read main settings error");
            }
        }

        public void WriteSettings()
        {
            try
            {
                var settings = TinyIoCContainer.Current.Resolve<MainSettings>();

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

        private String SettingsFile() { return FileRoutines.CoreDir + @"\settings.xml"; }

    }
}