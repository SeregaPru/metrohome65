using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace MetroHome65.Widgets
{

    public abstract class BaseWidget : IWidget, INotifyPropertyChanged
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

        public virtual void OnClick(Point Location) { }

        public virtual void OnDblClick(Point Location) { }

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


        public virtual List<Control> EditControls { get { return new List<Control>(); } }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
               PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

    }

}
