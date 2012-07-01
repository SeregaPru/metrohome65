using System;
using System.Drawing;

namespace MetroHome65.Tile
{
    public partial class BaseTileGrid
    {
        public void AddTile(Point location)
        {
            var cell = GetTileCell(location);
            if (cell.X + 2 > _gridWidth)
                cell.X = _gridWidth - 2;
            MoveTileTo(
                AddTile(cell, new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", true),
                location);
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
        protected TileWrapper AddTile(Point aGridPosition, Size aGridSize, String aWidgetName, bool doRealign)
        {
            return AddTile(
                new TileWrapper(aGridSize, aGridPosition, aWidgetName, GetPadding()), doRealign);
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        protected TileWrapper AddTile(TileWrapper tile, bool doRealign)
        {
            // draw tile when adding
            tile.Draw(null);

            _tiles.Add(tile);
            _tilesCanvas.AddElement(tile);

            if (doRealign)
            {
                RealignTiles();
            }

            tile.TapHandler = p => TileClickAt(p, tile);
            tile.HoldHandler = p => TileHoldAt(tile);

            return tile;
        }

        /// <summary>
        /// Delete current widget from grid.
        /// Then widgets are realigned, if deleted widget was alone on its row.
        /// </summary>
        public void DeleteSelectedTile()
        {
            if (SelectedTile == null) return;

            var deletingTile = SelectedTile;
            SelectedTile = null;

            _tiles.Remove(deletingTile);
            _tilesCanvas.DeleteElement(deletingTile);

            RealignTiles();
            WriteSettings();
        }

        /// <summary>
        /// Click at tile handler
        /// </summary>
        /// <param name="aLocation"></param>
        /// <param name="tile"> </param>
        private bool TileClickAt(Point aLocation, TileWrapper tile)
        {
            // if Move mode is enabled, place selected widget to the new position
            // if we click at moving widget, exit from move mode
            if (SelectionMode)
            {
                // if click at moving tile - exit from moving mode
                // if click at another tile - change moving tile to selected
                SelectedTile = (tile == SelectedTile) ? null : tile;
                return true;
            }

            // if tile launches external program, start exit animation for visible tiles
            if (tile.Tile.DoExitAnimation)
            {
                Active = false;
                _launching = true;
                _tilesCanvas.AnimateExit();
            }

            var clickResult = tile.OnClick(aLocation);

            // if tile's onClick action failed, play back entrance animation
            if ((tile.Tile.DoExitAnimation) && (!clickResult))
            {
                // when page activate it plays entrance animation
                Active = true;
            }
            return true;
        }

        /// <summary>
        /// Long tap handler - entering to customizing mode
        /// </summary>
        private bool TileHoldAt(TileWrapper tile)
        {
            if (!SelectionMode)
            {
                SelectedTile = tile;
                return true;
            }
            return false;
        }

    }
}
