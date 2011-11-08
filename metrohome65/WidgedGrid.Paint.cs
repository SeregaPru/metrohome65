using System;
using System.Drawing;
using System.Windows.Forms;

namespace MetroHome65.Pages
{
    partial class WidgetGrid
    {
        
        private Bitmap _DoubleBuffer = null;
        private Graphics _graphics = null;

        // create buffer bitmap with alpha channel
        private void CreateBuffer(int AWidth, int AHeight)
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_DoubleBuffer != null)
            {
                _DoubleBuffer.Dispose();
                _DoubleBuffer = null;
            }

            _DoubleBuffer = new Bitmap(AWidth, AHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            _graphics = Graphics.FromImage(_DoubleBuffer);
        }


        private void RepaintWidget(WidgetWrapper Widget)
        {
            Widget.Paint(_graphics, true);

            _WidgetsImage.Invalidate(Widget.ScreenRect);
        }


        private void RepaintGrid()
        {
            //??_graphics.Clear(Color.Transparent);

            // paint widgets
            foreach (WidgetWrapper wsInfo in Widgets)
                wsInfo.Paint(_graphics, false);

            //if (!BufferOnly)
            //    _WidgetsImage.Invalidate();

            //??if (_WidgetsImage.Image != null)
            //??    _WidgetsImage.Image.Dispose();
            _WidgetsImage.Image = _DoubleBuffer;
            _WidgetsImage.Size = _DoubleBuffer.Size;
        }


        private void WidgetGrid_Resize(object sender, EventArgs e)
        {
            _WidgetsContainer.Size = new Size(
                WidgetWrapper.CellWidth * 4 + WidgetWrapper.CellSpacingHor * 3,
                this.Height - _WidgetsContainer.Top);

            panelButtons.Location = new Point(
                this.Width - (this.Width - _WidgetsContainer.Left - _WidgetsContainer.Width) / 2 - panelButtons.Width / 2, _WidgetsContainer.Top);

            RepaintBackground();

            UpdateGridSize();
        }


        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (_background != null)
                e.Graphics.DrawImage(_background, 0, 0);
        }

        private void RepaintBackground()
        {
            _background = new Bitmap(Width, Height);
            Bitmap _tmp = new Bitmap(@"\My Documents\My Pictures\leaf.jpg");
            Graphics.FromImage(_background).DrawImage(_tmp,
                new Rectangle(0, 0, Width, Height),
                new Rectangle(0, 0, _tmp.Width, _tmp.Height),
                GraphicsUnit.Pixel);
        }

        private Image _background;

        public Image BackgroundImage
        {
            get { return _background; }
        }

    }
}
