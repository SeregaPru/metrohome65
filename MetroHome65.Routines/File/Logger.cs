using System.IO;

namespace MetroHome65.Routines.File
{
    public class Logger
    {
        public static void WriteLog(string stackTrace, string message)
        {
            using (Stream stream = System.IO.File.Open(FileRoutines.CoreDir + @"\errors.txt",
                FileMode.Append, FileAccess.Write))
            {
                var filewriter = new StreamWriter(stream);
                filewriter.WriteLine("=== " + message + " ===\n\r" + stackTrace);
                filewriter.Flush();
                filewriter.Close();
            }
        }
    }
}
