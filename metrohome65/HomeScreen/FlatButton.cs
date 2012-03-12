using System;
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
        //private Bitmap _image;
        private AlphaImage _image;

        public string ResourceName
        {
            set
            {
                try
                {
                    //_image = ResourceManager.Instance.GetBitmapFromEmbeddedResource(value);
                    _image = new AlphaImage(value, this.GetType().Assembly);
                }
                catch (Exception) { }
                Update();
            }
        }

        public FlatButton(string resourceName) : base()
        {
            ResourceName = resourceName;
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            //drawingGraphics.DrawText("*");

            _image.PaintIcon(drawingGraphics.Graphics, drawingGraphics.CalculateX(0), drawingGraphics.CalculateY(0));

            /*
            Color transparentKeyColor = Color.Black; // DEFAULT BLACK, IF IT IS NOT POSSIBLE TO READ FROM IMAGE
            if (_image != null)
                transparentKeyColor = _image.GetPixel(0, 0);
            drawingGraphics.DrawImage(_image, 0, 0, Size.Width, Size.Height, transparentKeyColor);
            */
        }

    }
}
