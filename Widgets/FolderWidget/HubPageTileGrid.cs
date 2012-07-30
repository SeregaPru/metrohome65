using Fleux.UIElements;
using MetroHome65.Tile;

namespace FolderWidget
{
    public class HubPageTileGrid : BaseTileGrid
    {
        public HubPageTileGrid(UIElement background, string settingsFile, int gridWidth, int gridHeight) 
            : base(background, settingsFile, gridWidth, gridHeight)
        {
            // in hubs always use 
            TileTheme = new TileThemeWP7();
        }

        override protected int GetTopPadding()
        {
            return 0;
        }

    }
}