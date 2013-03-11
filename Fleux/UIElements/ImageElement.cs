using System;

namespace Fleux.UIElements
{
    using System.Drawing;
    using System.Reflection;
    using Core;
    using Core.GraphicsHelpers;

    public class ImageElement : UIElement
    {
        public enum StretchTypeOptions
        {
            None,
            Fill,
            Proportional
        }


        private Image _image;

        public Image Image
        {
            get { return _image; }
            set
            {
                if (_image == value) return;
                _image = value;
                Update();
            }
        }

        public StretchTypeOptions StretchType = StretchTypeOptions.Fill;


        public ImageElement(string resourceName)
            : this(ResourceManager.Instance.GetBitmapFromEmbeddedResource(resourceName, Assembly.GetCallingAssembly()))
        {
        }

        public ImageElement(Image image)
        {
            _image = image;
            if (image != null)
                Size = image.Size;
        }

        ~ImageElement()
        {
            if (_image != null)
                _image.Dispose();
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (_image == null) return;

            switch (StretchType)
            {
                case StretchTypeOptions.None:
                    drawingGraphics.DrawImage(_image, 0, 0);
                    break;
                case StretchTypeOptions.Fill:
                    drawingGraphics.DrawImage(_image, 0, 0, Size.Width, Size.Height);
                    break;
                case StretchTypeOptions.Proportional:
                    var scaleX = 1.0 * Size.Width / _image.Width;
                    var scaleY = 1.0 * Size.Height / _image.Height;
                    var scale = Math.Min(1, Math.Min(scaleX, scaleY));
                    drawingGraphics.DrawImage(_image, 0, 0, (int)Math.Round(_image.Width * scale), (int)Math.Round(_image.Height * scale));
                    break;
            }
        }

        // GIANNI Added
        public void DrawTransparent(IDrawingGraphics drawingGraphics)
        {
            if (_image == null) return;

            // Set the transparency color key based on the upper-left pixel of the image.
            var bmp = _image as Bitmap;
            var transparentKeyColor = Color.Black; // DEFAULT BLACK, IF IT IS NOT POSSIBLE TO READ FROM IMAGE
            if(bmp != null)
                transparentKeyColor = bmp.GetPixel(0, 0);

            drawingGraphics.DrawImage(_image, 0, 0, Size.Width, Size.Height, transparentKeyColor);
        }
    }
}
