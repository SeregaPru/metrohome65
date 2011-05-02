using System;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace SmartDeviceProject1
{
  
    /// <summary>
    /// Container for widget, middle layer between widget grid and widget plugin.
    /// </summary>
    public class WidgetWrapper
    {
        public static int CellWidth = 92;

        public static int CellHeight = 90;

        public static int CellSpacingVer = 9;

        public static int CellSpacingHor = 9;

        /// <summary>
        /// Widget position in grid
        /// </summary>
        public Point Position = new Point(0, 0);

        /// <summary>
        /// Widget size in cells
        /// </summary>
        public Size Size = new Size(1, 1);

        ///!! todo - button style, color
        public Color bgColor = System.Drawing.Color.Blue;
        
        public IWidget Widget = null;

        /// <summary>
        /// Widget absolute position on screen and size (in pixels) 
        /// </summary>
        [XmlIgnore]
        public Rectangle ScreenRect = new Rectangle(0, 0, 0, 0);
        
        public WidgetWrapper(Size Size, Point Position, IWidget Widget)
        {
            this.Size = Size;
            this.Position = Position;
            this.Widget = Widget;

            ScreenRect.X = Position.X * (CellWidth + CellSpacingHor) + CellSpacingHor;
            ScreenRect.Y = Position.Y * (CellHeight + CellSpacingVer) + CellSpacingVer;
            ScreenRect.Width = Size.Width * (CellWidth + CellSpacingHor) - CellSpacingHor;
            ScreenRect.Height = Size.Height * (CellHeight + CellSpacingVer) - CellSpacingVer;

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
