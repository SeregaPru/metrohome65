using System.Drawing;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    sealed partial class ProgramsMenu : ListElement
    {
        #region Fields

        private DoubleBuffer _buffer;

        private readonly UIElement _bgImage;

        private IDrawingGraphics _drawingGraphics;

        private readonly Graphics _controlGraphics;

        #endregion


        // fast drawind method instead of double bufferes scrollview's method
        // because we know that height is the whole screen and we don't neet cropping
        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (Content == null) return;
            Content.Draw(drawingGraphics.CreateChild(new Point(0, VerticalOffset)));
        }

        // draw direct to screen instead of buffer
        protected override void OnUpdated(UIElement element)
        {
            DirectDraw(this.VerticalOffset);
        }
        
        public void DirectDraw(int verticalOffset)
        {
            if (_buffer == null) return;

            // draw background
            _buffer.Graphics.Clear(MetroTheme.PhoneBackgroundBrush);
            _bgImage.Draw(_drawingGraphics.CreateChild(new Point(-this.Location.X, -this.Location.Y)));

            // draw tiles
            Content.Draw(_drawingGraphics.CreateChild(new Point(0, verticalOffset)));

            // draw buffer directly to screen
            _controlGraphics.DrawImage(_buffer.Image, Location.X, Location.Y);
        }

        private void CreateBuffer()
        {
            // create new buffer with new size
            _buffer = new DoubleBuffer(new Size(Size.Width, ScreenConsts.ScreenHeight));

            _drawingGraphics = DrawingGraphics.FromGraphicsAndRect(
                _buffer.Graphics, _buffer.Image,
                new Rectangle(0, 0, _buffer.Image.Width, _buffer.Image.Height));
        }

    }
}
