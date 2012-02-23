using System.Drawing;
using Fleux.Core;
using Fleux.Core.NativeHelpers;
using Fleux.UIElements;

namespace MetroHome65.HomeScreen
{
    public class FlatButton : UIElement
    {
        //!!private IImageWrapper _image;
        //!!private Image _image;

        public string ResourceName
        {
            set
            {
                //!!_image = ResourceManager.Instance.GetIImageFromEmbeddedResource(value);
                Update();
            }
        }

        public FlatButton(string resourceName) : base()
        {
            ResourceName = resourceName;
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.DrawText("*");
            //!!drawingGraphics.DrawAlphaImage(_image, 0, 0);
            //!!drawingGraphics.DrawImage(_image, 0, 0);
        }

    }
}
