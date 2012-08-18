using System;
using System.IO;
using MetroHome65.Routines.File;

namespace MetroHome65
{
    class PrepareEnvironment
    {
        public void Prepare()
        {
            MoveSettingsFiles();
        }

        private void MoveSettingsFiles()
        {
            try
            {
                // settings.xml to settings folder
                var oldSettingsFile = FileRoutines.CoreDir + @"\settings.xml";
                var newSettingsFile = FileRoutines.CoreDir + @"\settings\settings.xml";

                if (File.Exists(oldSettingsFile) && !File.Exists(newSettingsFile))
                    File.Move(oldSettingsFile, newSettingsFile);


                // widgets.xml to settings\widgets-wp7.xml
                var oldWidgetsFile = FileRoutines.CoreDir + @"\widgets.xml";
                var newWidgetsFile = FileRoutines.CoreDir + @"\settings\widgets-wp7.xml";

                if (File.Exists(oldWidgetsFile) && !File.Exists(newWidgetsFile))
                    File.Move(oldWidgetsFile, newWidgetsFile);
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, e.Message);
            }
        }
    }
}
