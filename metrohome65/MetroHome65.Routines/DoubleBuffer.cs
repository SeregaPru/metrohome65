using System.Drawing;
using System.Drawing.Imaging;
using Fleux.Core.GraphicsHelpers;
using Fleux.Core.NativeHelpers;

namespace MetroHome65.Routines
{

    public class DoubleBuffer
    {
        private Size _size;
        private Bitmap _image;
        private Graphics _graphics;
        private DrawingGraphics _drawingGraphics;

        public DoubleBuffer(Size size)
        {
            _size = size;
            PrepareBuffer();
        }

        ~DoubleBuffer()
        {
            ClearBuffer();
        }

        private void ClearBuffer()
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }
            _drawingGraphics = null;
        }

        private void PrepareBuffer()
        {
            ClearBuffer();

            _image = new Bitmap(_size.Width, _size.Height, PixelFormat.Format16bppRgb565);
            _graphics = Graphics.FromImage(_image);
            _drawingGraphics = DrawingGraphics.FromGraphicsAndRect(_graphics, _image,
                    new Rectangle(0, 0, _image.Width, _image.Height));
        }

        public Bitmap Image { get { return _image; } }

        public Graphics Graphics { get { return _graphics; } }

        public DrawingGraphics DrawingGraphics { get { return _drawingGraphics; } }

    }
}