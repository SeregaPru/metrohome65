using System;
using System.Text;
using System.Drawing;

namespace SmartDeviceProject1
{
    /// <summary>
    /// Container for widget, middle layer between widget grid and widget plugin.
    /// </summary>
    class WidgetWrapper
    {
        /// <summary>
        /// Widget position in grid
        /// </summary>
        public Point Position = new Point(0, 0);

        /// <summary>
        /// Widget size in cells
        /// </summary>
        public Point Size = new Point(1, 1);

        ///!! todo - button style, color
        public Color bgColor = System.Drawing.Color.Blue;
        
        private IWidget Widget = null;

        /// <summary>
        /// Widget absolute position on screen and size (in pixels) 
        /// </summary>
        public Rectangle ScreenRect = new Rectangle(0, 0, 0, 0);
        
        public WidgetWrapper(Point Size, Point Position, IWidget Widget)
        {
            this.Size = Size;
            this.Position = Position;
            this.Widget = Widget;
        }

        public void Paint(Graphics g, Rectangle Rect)
        {
            if (Widget != null)
            {
                if (Widget.Transparent)
                    PaintBackground(g, Rect);
                Widget.Paint(g, Rect);
            }
        }

        /// <summary>
        /// Paints backgroud for transparent widget.
        /// Style and color are user-defined.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        private void PaintBackground(Graphics g, Rectangle Rect)
        {
            Brush bgBrush = new System.Drawing.SolidBrush(bgColor);
            g.FillRectangle(bgBrush, Rect.Left, Rect.Top, Rect.Width, Rect.Height);
        }

        public void OnClick(Point Location)
        {
            if (Widget != null)
                Widget.OnClick(Location);
        }
    }
}
