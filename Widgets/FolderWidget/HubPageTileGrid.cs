using System.Drawing;
using Fleux.UIElements;
using MetroHome65.Tile;

namespace FolderWidget
{
    public class HubPageTileGrid : BaseTileGrid
    {
        //?? strange for me that I should override empty constructor with parameters
        public HubPageTileGrid(UIElement background, string settingsFile, int gridWidth, int gridHeight) 
            : base(background, settingsFile, gridWidth, gridHeight)
        {
        }

        override protected void ReadSettings()
        {
            base.ReadSettings();

            AddTile(new Point(0, 0), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false);
            AddTile(new Point(0, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false);
            AddTile(new Point(2, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false);
            AddTile(new Point(0, 4), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false);
            AddTile(new Point(2, 6), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false);
            AddTile(new Point(0, 8), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false);

            RealignTiles();
        }

        override protected Point GetPadding()
        {
            return new Point(TileConsts.TilesPaddingLeft, 0);
        }

    }
}