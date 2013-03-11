using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;
using Fleux.UIElements;

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


    public static class XMLHelper<T>
    {
        public static T Read(String fileName)
        {
            if (!File.Exists(fileName)) return default(T);

            var settings = default(T);
            try
            {
                TextReader reader = new StreamReader(fileName);
                var serializer = new XmlSerializer(typeof(T));
                settings = (T)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Read XML error");
            }

            return settings;
        }

        public static void Write(T settings, String fileName)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                TextWriter writer = new StreamWriter(fileName, false);
                serializer.Serialize(writer, settings);
                writer.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Write XML error");
            }
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

                if (Microsoft.WindowsMobile.Status.SystemState.DisplayRotation % 180 == 0)
                    _IsQVGA = (ScreenConsts.ScreenWidth < 245);
                else
                    _IsQVGA = (ScreenConsts.ScreenHeight < 245);

                _IsQVGACalculated = true;
                return _IsQVGA;
            }
        }

        public static int Scale(int Size)
        {
            //return (IsQVGA) ? (Size / 2) : Size;
            return Size;
        }

        private static Cursor _oldCursor = Cursors.Default;

        public static void CursorWait()
        {
            _oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
        }

        public static void CursorNormal()
        {
            Cursor.Current = _oldCursor;
        }

        public static Point ScreenLocaton(UIElement element)
        {
            var location = element.Location;
            var curElement = element;

            while ((curElement = curElement.Parent) != null)
                location.Offset(curElement.Location.X, curElement.Location.Y);

            return location;
        }
    }

    public static class ScreenConsts
    {
        public const int TopBarSize = 36;
        public static readonly int ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        public static readonly int ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
    }


}
