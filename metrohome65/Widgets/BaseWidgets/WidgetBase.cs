using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Fleux.Core.GraphicsHelpers;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{

    public abstract class BaseWidget : UIElement, ITile, INotifyPropertyChanged
    {
    
        #region ITile

        protected virtual Size[] GetSizes() { return null; }
        public Size[] Sizes { get { return GetSizes(); } }

        protected Size _gridSize;
        public Size GridSize { set{ _gridSize = value; } }

        public virtual void PaintBuffer(Graphics g, Rectangle rect) { }

        public virtual bool OnClick(Point location) { return false; }

        public virtual bool OnDblClick(Point location) { return false; }

        public virtual bool AnimateExit { get { return false; } }

        public virtual List<Control> EditControls { get { return new List<Control>(); } }

        #endregion



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
                _buffer = new DoubleBuffer(Size);
                PaintBuffer(_buffer.Graphics, new Rectangle(0, 0, Size.Width, Size.Height));
                _needRepaint = false;
            }

            drawingGraphics.DrawImage(_buffer.Image, 0, 0);
           
            //drawingGraphics.Graphics.DrawImage(_buffer.Image, 0, 0, 
            //    new Rectangle(0, 0, _buffer.Image.Width, _buffer.Image.Height), GraphicsUnit.Pixel);

            //drawingGraphics.Color(Color.Red);
            //drawingGraphics.FillRectangle(Bounds);
        }

        public virtual void ForceUpdate()
        {
            _needRepaint = true;
            Update();
            Application.DoEvents();
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
