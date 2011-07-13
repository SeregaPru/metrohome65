using System;
using System.IO;
using System.Runtime.InteropServices;

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


        [DllImport("coredll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SHGetFileInfo([MarshalAs(UnmanagedType.VBByRefStr)] ref string A_0, int A_1, ref structa A_2, int A_3, int A_4);

        [StructLayout(LayoutKind.Sequential)]
        public struct structa
        {
            public IntPtr a;
            public int b;
            public int c;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string d;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string e;
        }
    
    }

}
