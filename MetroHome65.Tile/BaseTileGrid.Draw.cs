using Fleux.UIElements;

namespace MetroHome65.Tile
{
    public partial class BaseTileGrid
    {
        // draw direct to screen instead of buffer
        protected override void OnUpdated(UIElement element)
        {
            if ((_tilesCanvas != null) 
                && (_tilesCanvas.FreezeUpdate) ) 
                _tilesCanvas.DirectDraw(this.VerticalOffset);
            else
            {
                base.OnUpdated(element);
            }
        }

        public override bool IsShowing(UIElement child)
        {
            // allow redraw when page is active (current) or page is in moving mode (in moving mode active flag is off to prevent live tiles redraw)
            return _active || SelectionMode;
        }

    }
}
