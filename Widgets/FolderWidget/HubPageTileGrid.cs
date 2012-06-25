using System.Drawing;
using MetroHome65.Routines;
using MetroHome65.Tile;

namespace FolderWidget
{
    public class HubPageTileGrid : BaseTileGrid
    {
        public HubPageTileGrid() : base(FileRoutines.CoreDir + @"\widgets.xml", 4, 100)
        {
            AddTile(new Point(0, 0), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", true);
            AddTile(new Point(0, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", true);
            AddTile(new Point(2, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", true);
        }
        
    }
}