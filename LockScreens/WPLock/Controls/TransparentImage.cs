//!! todo - заменить на TransparentImageElement

using System.Reflection;
using Fleux.Core;
using Fleux.UIElements;
using Fleux.Core.GraphicsHelpers;
using Fleux.Core.NativeHelpers;

namespace MetroHome65.WPLock.Controls
{
    public class TransparentImage : UIElement
    {
        private IImageWrapper image;

        public TransparentImage(string resourceName)
            : this(ResourceManager.Instance.GetIImageFromEmbeddedResource(resourceName, Assembly.GetCallingAssembly()))
        {
        }

        public TransparentImage(IImageWrapper image)
        {
            this.image = image;
            this.Size = image.Size;
        }

        public IImageWrapper Image
        {
            get { return this.image; }
            set { this.image = value; }
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.DrawAlphaImage(this.image, 0, 0, Size.Width, Size.Height);
        }

    }
}
