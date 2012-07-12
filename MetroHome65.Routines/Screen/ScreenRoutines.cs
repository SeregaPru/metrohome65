using System;
using System.Drawing;
using System.Windows.Forms;
using Fleux.UIElements;

namespace MetroHome65.Routines.Screen
{
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
}