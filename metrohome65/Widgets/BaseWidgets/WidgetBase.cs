using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace MetroHome65.Widgets
{

    public abstract class BaseWidget : IWidget, INotifyPropertyChanged
    {
        private readonly EventHandlerList _events = new EventHandlerList();
        private static readonly object _EventWidgetUpdate = new object();

        protected virtual Size[] GetSizes() { return null; }
        public Size[] Sizes { get { return GetSizes(); } }

        protected Size _Size;
        protected virtual void SetSize(Size value) { _Size = value; }
        public Size Size { set { SetSize(value); } }

        protected virtual String[] GetMenuItems() { return null; } 
        public String[] MenuItems { get { return GetMenuItems(); } }
        
        protected virtual Boolean GetTransparent() { return false; }
        public Boolean Transparent { get { return GetTransparent(); } }
        
        public virtual void Paint(Graphics g, Rectangle rect) { }

        public virtual bool OnClick(Point location) { return false; }

        public virtual bool OnDblClick(Point location) { return false; }

        public virtual void OnMenuItemClick(String itemName) { }


        /// <summary>
        /// Event raised when Widget needs to be updated (repainted)
        /// </summary>
        public event WidgetUpdateEventHandler WidgetUpdate
        {
            add { _events.AddHandler(_EventWidgetUpdate, value); }
            remove { _events.RemoveHandler(_EventWidgetUpdate, value); }       
        }

        protected void OnWidgetUpdate()
        {
            var handler = _events[_EventWidgetUpdate] as WidgetUpdateEventHandler;
            if (handler != null)
                handler();
        }

        public virtual bool AnimateExit { get { return false; } }

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
