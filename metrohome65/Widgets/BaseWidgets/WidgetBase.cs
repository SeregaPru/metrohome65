using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{

    public abstract class BaseWidget : UIElement, ITile, INotifyPropertyChanged
    {
    
        #region ITile

        protected virtual Size[] GetSizes() { return null; }
        public virtual Size[] Sizes { get { return GetSizes(); } }

        protected Size _gridSize;
        public virtual Size GridSize { set { _gridSize = value; } }

        public virtual void PaintBuffer(Graphics g, Rectangle rect) { }

        public virtual bool OnClick(Point location) { return false; }

        public virtual bool OnDblClick(Point location) { return false; }

        protected virtual bool GetDoExitAnimation() { return false; }
        public virtual bool DoExitAnimation { get { return GetDoExitAnimation(); } }

        public virtual ICollection<UIElement> EditControls(FleuxControlPage settingsPage) { return new List<UIElement>(); }

        #endregion


        public BaseWidget()
        {
            MetroTheme.PropertyChanged += OnThemeSettingsChanged;
        }

        private void OnThemeSettingsChanged(PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "PhoneAccentBrush") || (e.PropertyName == "PhoneForegroundBrush"))
            {
                ForceUpdate();
            }
        }

        #region Draw

        // double buffer
        private DoubleBuffer _buffer;
        private bool _needRepaint;

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (drawingGraphics == null)
                return;

            if ((_buffer == null) || (_needRepaint))
            {
                var newbuffer = new DoubleBuffer(Size);
                PaintBuffer(newbuffer.Graphics, new Rectangle(0, 0, Size.Width, Size.Height));
                _buffer = newbuffer;
                _needRepaint = false;
            }

            drawingGraphics.DrawImage(_buffer.Image, 0, 0);
           
            // draw direct to graphic for speed
            //drawingGraphics.Graphics.DrawImage(_buffer.Image, drawingGraphics.CalculateX(0), drawingGraphics.CalculateY(0));
        }

        public virtual void ForceUpdate()
        {
            _needRepaint = true;
            Update();
        }

        #endregion


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
