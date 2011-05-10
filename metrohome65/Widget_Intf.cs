using System;
using System.Drawing;
using System.Collections;


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

    /// <summary>
    /// Signs field or property of widget, that it is user defined parameter,
    /// and it should be stored in widget's user settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class WidgetParameter : Attribute { }


    /// <summary>
    /// Base interface for widget
    /// </summary>
    public interface IWidget
    {
        /// <summary>
        /// widget possible sizes, in cells
        /// 1x1 1x2 2x2 etc. .. 4x4 is max
        /// </summary>
        Size[] Sizes { get; }

        /// <summary>
        /// Additional popup menu items for widget.
        /// They will be shown in widget popup menu, above standart items.
        /// </summary>
        String[] MenuItems { get; }

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

        /// <summary>
        /// Hanler for custom menu item click
        /// </summary>
        /// <param name="ItemName"></param>
        void MenuItemClick(String ItemName);
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
