using System;
using MetroHome65.Routines.File;
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
            var settings = XmlHelper<MainSettings>.Read(SettingsFile());
            if (settings == null)
                settings = new MainSettings();

            TinyIoCContainer.Current.Register<MainSettings>(settings);
            settings.ApplyTheme();
        }

        public void WriteSettings()
        {
            try
            {
                var settings = TinyIoCContainer.Current.Resolve<MainSettings>();
                XmlHelper<MainSettings>.Write(settings, SettingsFile());
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Write main settings error");
            }
        }

        private String SettingsFile() { return FileRoutines.CoreDir + @"\settings\settings.xml"; }

    }
}