using Fleux.UIElements;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public class FreezedCanvas : Canvas
    {
        #region Properties

        public bool FreezeUpdate { get; set; }

        #endregion

        /// <summary>
        /// Handler for update event from child.
        /// Redraw only in non-freezed mode
        /// </summary>
        /// <param name="element"></param>
        protected override void OnUpdated(UIElement element)
        {
            if (!FreezeUpdate)
            {
                this.Updated(this);
            }
        }

    }
}
