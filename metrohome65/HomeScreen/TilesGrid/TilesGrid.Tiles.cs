using System;
using System.Drawing;
using Fleux.UIElements;

namespace MetroHome65.HomeScreen.TilesGrid
{

    public partial class TilesGrid : ScrollViewer
    {
        private void AddTileHandler(Point aLocation)
        {
            Point cell = GetTileCell(aLocation);
            if (cell.X + 2 > 4)
                cell.X = 2;
            MoveTileTo(
                AddTile(cell, new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", true),
                aLocation);
            WriteSettings();
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        /// <param name="aGridPosition"> </param>
        /// <param name="aGridSize"> </param>
        /// <param name="aWidgetName"> </param>
        /// <param name="doRealign"> </param>
        private TileWrapper AddTile(Point aGridPosition, Size aGridSize, String aWidgetName, bool doRealign)
        {
            return AddTile(
                new TileWrapper(aGridSize, aGridPosition, aWidgetName), doRealign);
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        private TileWrapper AddTile(TileWrapper tile, bool doRealign)
        {
            // draw tile when adding
            tile.Draw(null);

            _tiles.Add(tile);
            _tilesCanvas.AddElement(tile);

            if (doRealign)
            {
                RealignTiles();
            }

            tile.TapHandler = p => { TileClickAt(p, tile); return true; };
            tile.HoldHandler = p => { TileHoldAt(p, tile); return true; };

            return tile;
        }

        /// <summary>
        /// Delete current widget from grid.
        /// Then widgets are realigned, if deleted widget was alone on its row.
        /// </summary>
        private void DeleteTile()
        {
            var deletingTile = MovingTile;
            MovingTile = null;

            _tiles.Remove(deletingTile);
            _tilesCanvas.DeleteElement(deletingTile);

            RealignTiles();
            WriteSettings();
        }

    }
}
