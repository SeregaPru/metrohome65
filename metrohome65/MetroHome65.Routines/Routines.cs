using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MetroHome65.Routines
{
    /// <summary>
    /// Various service file routines
    /// </summary>
    public static class FileRoutines
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


    /// <summary>
    /// Various service screen routines
    /// </summary>
    public static class ScreenRoutines
    {
        private static Boolean _IsQVGA = true;
        private static Boolean _IsQVGACalculated = false;

        /// <summary>
        /// Flag, indicates when screen resolution is QVGA - 320*240
        /// </summary>
        public static Boolean IsQVGA
        {
            get
            {
                if (_IsQVGACalculated)
                    return _IsQVGA;

                int ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                int ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

                if (Microsoft.WindowsMobile.Status.SystemState.DisplayRotation % 180 == 0)
                    _IsQVGA = (ScreenWidth < 245);
                else
                    _IsQVGA = (ScreenHeight < 245);

                _IsQVGACalculated = true;
                return _IsQVGA;
            }
        }

        public static int Scale(int Size)
        {
            return (IsQVGA) ? (Size / 2) : Size;
        }
    }

}
