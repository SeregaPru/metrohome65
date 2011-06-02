using System;

namespace MetroHome65
{
    /// <summary>
    /// Various service routines
    /// </summary>
    class Routines
    {

        public static void StartProcess(string FileName)
        {
            try
            {
                System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = FileName;
                myProcess.Start();
            }
            catch (Exception ex)
            {
                //!! write to log  (e.StackTrace, "StartProcess")
            }
        }

    }
}
