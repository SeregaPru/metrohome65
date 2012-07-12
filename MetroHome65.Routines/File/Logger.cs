using System.IO;

namespace MetroHome65.Routines.File
{
    public class Logger
    {
        public static void WriteLog(string StackTrace, string Message)
        {
            using (Stream stream = System.IO.File.Open(FileRoutines.CoreDir + @"\errors.txt",
                FileMode.Append, FileAccess.Write))
            {
                var filewriter = new StreamWriter(stream);
                filewriter.WriteLine("=== " + Message + " ===\n\r" + StackTrace);
                filewriter.Flush();
                filewriter.Close();
            }
        }
    }
}
