using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;


namespace MetroHome65.Widgets
{

    /// <summary>
    /// Attribute - widget description, that will be diplayed in properties page
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WidgetInfoAttribute : Attribute
    {
        private String _Caption = "";

        public String Caption { get { return _Caption; } }


        public WidgetInfoAttribute() { }

        public WidgetInfoAttribute(String Caption)
        {
            this._Caption = Caption;
        }
    }


    /// <summary>
    /// Signs field or property of widget, that it is user defined parameter,
    /// and it should be stored in widget's user settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class WidgetParameterAttribute : Attribute {
        public WidgetParameterAttribute() { }
    }


    /// <summary>
    /// Widget base interface
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
        /// paint widget's internal area.
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="rect">Drawing area</param>
        void Paint(Graphics g, Rectangle Rect);

        /// <summary>
        /// Hanler for custom menu item click
        /// </summary>
        /// <param name="ItemName"></param>
        void OnMenuItemClick(String ItemName);

        /// <summary>
        /// Handler for click event
        /// </summary>
        /// <param name="Location">
        ///   Coordinates of click event, 
        ///   relative to widget's left upper corner
        /// </param>
        void OnClick(Point Location);

        void OnDblClick(Point Location);

        List<Control> EditControls { get; }
    }


    /// <summary>
    /// Event triggered when widget should be updated
    /// </summary>
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
    /// Additional interface for widget, that periodically updates its state
    /// </summary>
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
