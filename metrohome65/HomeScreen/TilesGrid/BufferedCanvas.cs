using System.Linq;
using Fleux.Core.GraphicsHelpers;
using Fleux.UIElements;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public class BufferedCanvas : Canvas
    {
        #region Fields

        private bool _needRepaint;

        private DoubleBuffer _buffer;

        #endregion


        #region Properties

        public bool FreezeUpdate { get; set; }

        #endregion


        #region override methods
    
        public override void AddElement(UIElement element)
        {
            base.AddElement(element);
            _needRepaint = true;
        }

        public override void AddElementAt(int index, UIElement element)
        {
            base.AddElementAt(index, element);
            _needRepaint = true;
        }

        protected override void OnUpdated(UIElement element)
        {
            if (!FreezeUpdate)
            {
                Update();
            }
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (drawingGraphics == null)
                return;

            if (
                ((_buffer == null) || (_needRepaint)) &&
                (!FreezeUpdate)
                )
            {
                _buffer = new DoubleBuffer(Size);
                Children
                    .ToList()
                    .ForEach(e => e.Draw(drawingGraphics.CreateChild(e.Location, e.TransformationScaling, e.TransformationCenter)));
                _needRepaint = false;
            }

            if (_buffer != null)
                drawingGraphics.DrawImage(_buffer.Image, 0, 0);
        }

        #endregion


        #region methods

        public void DeleteElement(UIElement element)
        {
            element.Parent = null;
            element.Updated = null;
            Children.Remove(element);
            _needRepaint = true;
        }

        #endregion

    }
}

