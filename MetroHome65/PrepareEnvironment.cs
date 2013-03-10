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
                UpgradeFromOldVersion();

                PrepareAfterInstall();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, e.Message);
            }
        }

        private void PrepareAfterInstall()
        {
            // .xml.orig to .xml
            ReplaceFile(@"\settings\settings.xml.orig",    @"\settings\settings.xml");
            ReplaceFile(@"\settings\widgets-wp7.xml.orig", @"\settings\widgets-wp7.xml");
            ReplaceFile(@"\settings\widgets-wp8.xml.orig", @"\settings\widgets-wp8.xml");
        }

        private void UpgradeFromOldVersion()
        {
            // settings.xml to settings folder
            ReplaceFile(@"\settings.xml", @"\settings\settings.xml");

            // widgets.xml to settings\widgets-wp7.xml
            ReplaceFile(@"\widgets.xml", @"\settings\widgets-wp7.xml");
        }

        private void ReplaceFile(string oldFile, string newFile)
        {
            if (File.Exists(FileRoutines.CoreDir + oldFile) && !File.Exists(FileRoutines.CoreDir + newFile))
                File.Move(FileRoutines.CoreDir + oldFile, FileRoutines.CoreDir + newFile);
        }

    }
}
