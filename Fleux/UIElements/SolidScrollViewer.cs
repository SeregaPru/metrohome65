using System.Drawing;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;

namespace Fleux.UIElements
{
    public class SolidScrollViewer : ScrollViewer
    {
        public Color Background;

        public SolidScrollViewer()
        {
            Background = MetroTheme.PhoneBackgroundBrush;
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (this.Content == null) return;

            //!! todo - потом можно выделить в какой нибудь класс обертку над drawingGraphics
            var location = this.Location;
            UIElement curElement = this;
            while ((curElement = curElement.Parent) != null)
                location.Offset(curElement.Location.X, curElement.Location.Y);
            drawingGraphics.Graphics.Clip = new Region(new Rectangle(location.X, location.Y, Size.Width, Size.Height)); 


            this.Content.Draw(drawingGraphics.CreateChild(new Point(this.HorizontalOffset, this.VerticalOffset),
                                                 this.Content.TransformationScaling, this.Content.TransformationCenter));
            if (this.ShowScrollbars)
            {
                this.DoDrawScrollBar(drawingGraphics);
            }

            if (this.DrawShadows)
            {
                DoDrawShadows(drawingGraphics);
            }


            drawingGraphics.Graphics.ResetClip(); //
        }
    }
}