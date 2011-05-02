using System;
using System.Drawing;

namespace SmartDeviceProject1
{
    public class WidgetUpdateEventArgs : EventArgs
    {
        public IWidget Widget { get; private set; }

        public WidgetUpdateEventArgs(IWidget Widget)
        {
            this.Widget = Widget;
        }
    }

    public delegate void WidgetUpdateEventHandler(object sender, WidgetUpdateEventArgs e);

    public interface IWidget
    {
        /// <summary>
        /// widget possible sizes, in cells
        /// 1x1 1x2 2x2 etc. .. 4x4 is max
        /// </summary>
        Size[] Sizes { get; }

        /// <summary>
        /// Widget transparency.
        /// Transparent widget draws itself over grid background.
        /// Not transparent widget should draw the whole area.
        /// </summary>
        Boolean Transparent { get; }

        /// <summary>
        /// Handler for click event
        /// </summary>
        /// <param name="Location">
        ///   Coordinates of click event, 
        ///   relative to widget's left upper corner
        /// </param>
        void OnClick(Point Location);

        /// <summary>
        /// paint widget's internal area.
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="rect">Drawing area</param>
        void Paint(Graphics g, Rectangle Rect);

    }


    public interface IWidgetUpdatable : IWidget
    {
        /// <summary>
        /// Event raised when Widget needs to be updated (repainted)
        /// </summary>
        event WidgetUpdateEventHandler WidgetUpdate;

        void StartUpdate();
        void StopUpdate();
    }
}
