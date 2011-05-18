using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace MetroHome65.Widgets
{

    public class BaseWidget : IWidget
    {
        private EventHandlerList _Events = new EventHandlerList();
        private static readonly object _EventWidgetUpdate = new object();

        protected virtual Size[] GetSizes() { return null; }
        public Size[] Sizes { get { return GetSizes(); } }

        protected virtual String[] GetMenuItems() { return null; } 
        public String[] MenuItems { get { return GetMenuItems(); } }
        
        protected virtual Boolean GetTransparent() { return false; }
        public Boolean Transparent { get { return GetTransparent(); } }
        
        public virtual void Paint(Graphics g, Rectangle Rect) { }

        public virtual void OnClick(Point Location)
        {
            MessageBox.Show(String.Format("click at widget at {0}:{1}", 
                Location.X, Location.Y));
        }

        public virtual void OnDblClick(Point Location)
        {
        }

        public virtual void OnMenuItemClick(String ItemName) { }

        /// <summary>
        /// Event raised when Widget needs to be updated (repainted)
        /// </summary>
        public event WidgetUpdateEventHandler WidgetUpdate
        {
            add { _Events.AddHandler(_EventWidgetUpdate, value); }
            remove { _Events.RemoveHandler(_EventWidgetUpdate, value); }       
        }

        protected void OnWidgetUpdate()
        {
            var handler = _Events[_EventWidgetUpdate] as WidgetUpdateEventHandler;
            if (handler != null)
            {
                WidgetUpdateEventArgs e = new WidgetUpdateEventArgs(this);
                handler(this, e);
            }
        }
    }


    /// <summary>
    /// Base class for abstract transparent widget.
    /// </summary>
    public class TransparentWidget : BaseWidget
    {
        protected override Boolean GetTransparent() { return true; }
    }

}
