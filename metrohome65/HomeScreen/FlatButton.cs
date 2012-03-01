using System.Drawing;
using System.Reflection;
using Fleux.Core;
using Fleux.Core.NativeHelpers;
using Fleux.UIElements;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen
{
    public class FlatButton : UIElement
    {
        private Image _image;
        //private IImageWrapper _image;
        //private AlphaImage _image;

        public string ResourceName
        {
            set
            {
                _image = ResourceManager.Instance.GetBitmapFromEmbeddedResource(value);
                //_image = ResourceManager.Instance.GetIImageFromEmbeddedResource(value);
                //_image = new AlphaImage(value, this.GetType().Assembly);
                Update();
            }
        }

        public FlatButton(string resourceName) : base()
        {
            ResourceName = resourceName;
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            //!!drawingGraphics.DrawText("*");
            //drawingGraphics.DrawAlphaImage(_image, 0, 0);
            //_image.PaintIcon(drawingGraphics.Graphics, - drawingGraphics.VisibleRect.Left, - drawingGraphics.VisibleRect.Top);
            drawingGraphics.DrawImage(_image, 0, 0);
        }

    }
}
