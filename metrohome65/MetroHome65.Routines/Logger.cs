using System;
using System.Text;
using System.IO;

namespace MetroHome65.Routines
{
    public class Logger
    {
        public static void WriteLog(string StackTrace, string Message)
        {
            using (Stream stream = File.Open(FileRoutines.CoreDir + @"\errors.txt",
                FileMode.Append, FileAccess.Write))
            {
                StreamWriter filewriter = new StreamWriter(stream);
                filewriter.WriteLine("=== " + Message + " ===\n\r" + StackTrace);
                filewriter.Flush();
                filewriter.Close();
            }
        }
    }
}
