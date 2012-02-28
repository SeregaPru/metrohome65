using System;
using System.Runtime.InteropServices;

namespace MetroHome65.Routines
{
    /// <summary>
    /// Various service file routines
    /// </summary>
    public static class FileRoutines
    {
        public static String CoreDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

        public static bool StartProcess(string FileName)
        {
            try
            {
                System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = FileName;
                myProcess.Start();
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "StartProcess error");
                return false;
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
            //return (IsQVGA) ? (Size / 2) : Size;
            return Size;
        }
    }

}
