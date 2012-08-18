using System;
using Fleux.Core.GraphicsHelpers;
using Fleux.Core.NativeHelpers;

namespace Fleux.UIElements
{

    public class TransparentImageElement : UIElement
    {
        private IImageWrapper _image;

        public IImageWrapper Image
        {
            get { return this._image; }
            set { this._image = value; }
        }


        public enum StretchTypeOptions
        {
            None,
            Fill,
            Proportional
        }

        public StretchTypeOptions StretchType = StretchTypeOptions.Fill;


        public TransparentImageElement(IImageWrapper image)
        {
            this._image = image;
            this.Size = image.Size;
        }

        ~TransparentImageElement()
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
                    drawingGraphics.DrawAlphaImage(_image, 0, 0);
                    break;
                case StretchTypeOptions.Fill:
                    drawingGraphics.DrawAlphaImage(_image, 0, 0, Size.Width, Size.Height);
                    break;
                case StretchTypeOptions.Proportional:
                    var scaleX = 1.0 * Size.Width / _image.Size.Width;
                    var scaleY = 1.0 * Size.Height / _image.Size.Height;
                    var scale = Math.Min(1, Math.Min(scaleX, scaleY));
                    drawingGraphics.DrawAlphaImage(_image, 0, 0, (int)Math.Round(_image.Size.Width * scale), (int)Math.Round(_image.Size.Height * scale));
                    break;
            }
        }
    }
}