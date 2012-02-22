using Fleux.Core;
using Fleux.UIElements;

namespace MetroHome65.HomeScreen
{
    public class FlatButton : TransparentImageElement
    {
        public FlatButton(string resourceName) : base(ResourceManager.Instance.GetIImageFromEmbeddedResource(resourceName))
        {
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            //!!base.Draw(drawingGraphics);
            drawingGraphics.DrawText("*");
        }

    }
}
