namespace Fleux.UIElements
{
    using System.Drawing;
    using System.Reflection;
    using Core;
    using Core.GraphicsHelpers;

    public class ImageElement : UIElement
    {
        private Image _image;

        public ImageElement(string resourceName)
            : this(ResourceManager.Instance.GetBitmapFromEmbeddedResource(resourceName, Assembly.GetCallingAssembly()))
        {
        }

        public ImageElement(Image image)
        {
            this._image = image;
            if (image != null)
                this.Size = image.Size;
        }

        public Image Image
        {
            get { return this._image; }
            set { this._image = value; }
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (_image == null) return;

            drawingGraphics.DrawImage(this._image, 0, 0, Size.Width, Size.Height);
        }

        // GIANNI Added
        public void DrawTransparent(IDrawingGraphics drawingGraphics)
        {
            if (_image == null) return;

            // Set the transparency color key based on the upper-left pixel of the image.
            var bmp = this._image as Bitmap;
            var transparentKeyColor = Color.Black; // DEFAULT BLACK, IF IT IS NOT POSSIBLE TO READ FROM IMAGE
            if(bmp != null)
                transparentKeyColor = bmp.GetPixel(0, 0);

            drawingGraphics.DrawImage(this._image, 0, 0, Size.Width, Size.Height, transparentKeyColor);
        }
    }
}
