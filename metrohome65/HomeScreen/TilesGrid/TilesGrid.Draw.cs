using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public partial class TilesGrid : ScrollViewer, IActive
    {
        // fast drawind method instead of double bufferes scrollview's method
        // because we know that height is the whole screen and we don't neet cropping
        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            Content.Draw(drawingGraphics.CreateChild(new Point(0, VerticalOffset)));
        }

        // draw direct to screen instead of buffer
        protected override void OnUpdated(UIElement element)
        {
            if ((_tilesCanvas != null) 
                /* && (_tilesCanvas.FreezeUpdate) */) 
                _tilesCanvas.DirectDraw(this.VerticalOffset);
            //else
            //{
            //    base.OnUpdated(element);
            //    Application.DoEvents();
            //}
        }

        public override bool IsShowing(UIElement child)
        {
            // allow redraw when page is active (current) or page is in moving mode (in moving mode active flag is off to prevent live tiles redraw)
            return _active || MoveMode;
        }

        // don't stop tile's animation but simple turn off redraw during animation
        // to speed-up scrolling (avoid tiles animation during scrolling)
        private void FreezeUpdate(bool freeze)
        {
            _tilesCanvas.FreezeUpdate = freeze;
            //ActivateTilesAsync(!freeze);
            foreach (var wsInfo in _tiles)
                wsInfo.Pause = freeze;
        }

        // IActive
        public Boolean Active
        {
            get { return _active; }
            set
            {
                if (!value)
                {
                    // stop scroll animation
                    Pressed(new Point(-1, -1));

                    // stop moving animation
                    MovingTile = null;
                }

                if (_active == value) return;
                _active = value;

                // когда активируем после запуска внешнего приложения - играем входящую анимацию
                if ((_active) && (_launching))
                {
                    _launching = false;
                    _tilesCanvas.AnimateEntrance();
                }

                FreezeUpdate(!_active);
                ActivateTilesAsync(_active);
            }
        }

        // start/stop updatable widgets
        private void ActivateTilesAsync(bool active)
        {
            new Thread(() =>
            {
                // lock asynchronous activisation
                // for sequental runing activation - deactivation
                lock (this)
                {
                    foreach (var wsInfo in _tiles)
                        wsInfo.Active = active;
                }
            }
            ).Start();
        }


    }
}
