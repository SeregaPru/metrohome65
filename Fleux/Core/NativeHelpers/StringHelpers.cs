namespace Fleux.Core.NativeHelpers
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public static class StringHelpers
    {
        private const int DTCALCRECT = 0x00000400;
        private const int DTWORDBREAK = 0x00000010;
        private const int DTEDITCONTROL = 0x00002000;

        /// <summary>
        /// Measure a multiline string
        /// </summary>
        public static Size MeasureString(Graphics gr, Font font, string text, int width)
        {
            if (text == null) return new Size(1, 1);

            Rect bounds = new Rect() { Left = 0, Right = width, Bottom = 1, Top = 0 };
            IntPtr hDc = gr.GetHdc();
            try
            {
                int flags = DTCALCRECT | DTWORDBREAK;
                IntPtr controlFont = font.ToHfont();
                IntPtr originalObject = SelectObject(hDc, controlFont);
                try
                {
                    DrawText(hDc, text, text.Length, ref bounds, flags);
                }
                finally
                {
                    SelectObject(hDc, originalObject); // Release resources
                }
            }
            finally
            {
                gr.ReleaseHdc(hDc);
            }

            return new Size(bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
        }

        [DllImport("coredll.dll")]
        private static extern int DrawText(IntPtr hdc, string lpstr, int ncount, ref Rect lprect, int wformat);

        [DllImport("coredll.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hobject);

        internal struct Rect
        {
            public int Left, Top, Right, Bottom;

            public Rect(Rectangle r)
            {
                this.Left = r.Left;
                this.Top = r.Top;
                this.Bottom = r.Bottom;
                this.Right = r.Right;
            }
        }
    }
}
