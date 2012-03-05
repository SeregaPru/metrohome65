using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Fleux.Core.GraphicsHelpers;
using Fleux.UIElements;
using MetroHome65.Interfaces;

namespace MetroHome65.Widgets
{

    public abstract class BaseWidget : UIElement, ITile, INotifyPropertyChanged
    {

        ~BaseWidget()
        {
            ClearBuffer();
        }
     
        #region ITile

        protected virtual Size[] GetSizes() { return null; }
        public Size[] Sizes { get { return GetSizes(); } }

        protected Size _Size;
        protected virtual void SetSize(Size value) { _Size = value; }
        public Size Size { set { SetSize(value); } }

        public virtual void Paint(Graphics g, Rectangle rect) { }

        public virtual bool OnClick(Point location) { return false; }

        public virtual bool OnDblClick(Point location) { return false; }

        public virtual bool AnimateExit { get { return false; } }

        public virtual List<Control> EditControls { get { return new List<Control>(); } }

        #endregion



        #region Draw

        // double buffer
        private Bitmap _doubleBuffer;
        private Graphics _graphics;
        private bool _needRepaint = true;

        private void ClearBuffer()
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_doubleBuffer != null)
            {
                _doubleBuffer.Dispose();
                _doubleBuffer = null;
            }
        }

        private void PrepareBuffer()
        {
            ClearBuffer();

            _doubleBuffer = new Bitmap(Bounds.Width, Bounds.Height);
            _graphics = Graphics.FromImage(_doubleBuffer);
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (drawingGraphics == null)
                return;

            if (Bounds.Width + Bounds.Height == 0)
                return;

            if (_needRepaint)
            {
                PrepareBuffer();
                Paint(_graphics, new Rectangle(0, 0, Bounds.Width, Bounds.Height));
                _needRepaint = false;
            }

            drawingGraphics.DrawImage(_doubleBuffer, 0, 0);
            //.Graphics.DrawImage(_doubleBuffer, -drawingGraphics.VisibleRect.Left, -drawingGraphics.VisibleRect.Top);
        }

        public void ForceUpdate()
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
