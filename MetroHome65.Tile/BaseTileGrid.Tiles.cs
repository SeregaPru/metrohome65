using System;
using System.Drawing;
using System.Windows.Forms;

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
            tile.HoldHandler = p => TileHoldAt(p, tile);

            return tile;
        }

        /// <summary>
        /// Delete current widget from grid.
        /// Then widgets are realigned, if deleted widget was alone on its row.
        /// </summary>
        public void DeleteTile(TileWrapper tile)
        {
            _tiles.Remove(tile);
            _tilesCanvas.DeleteElement(tile);

            RealignTiles();
            WriteSettings();
        }

        public void Clear()
        {
            foreach (var tile in _tiles)
            {
                tile.TapHandler = null;
                tile.HoldHandler = null;
            }
            _tilesCanvas.Clear();
            _tiles.Clear();
        }

        /// <summary>
        /// Click at tile handler
        /// </summary>
        /// <param name="location"></param>
        /// <param name="tile"> </param>
        private bool TileClickAt(Point location, TileWrapper tile)
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

            var clickResult = tile.OnClick(location);

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
        private bool TileHoldAt(Point location, TileWrapper tile)
        {
            if (SelectionMode)
                return false;
                
            if (ParentControl == null)
                return false;

            var tileMenu = new ContextMenu();

            var menuTileEdit = new MenuItem { Text = "Edit" };
            menuTileEdit.Click += (s, e) => ShowTileSettings(tile);
            tileMenu.MenuItems.Add(menuTileEdit);

            var menuTileMove = new MenuItem { Text = "Move" };
            menuTileMove.Click += (s, e) => { SelectedTile = tile; };
            tileMenu.MenuItems.Add(menuTileMove);

            var menuTileDelete = new MenuItem { Text = "Delete" };
            menuTileDelete.Click += (s, e) => DeleteTile(tile);
            tileMenu.MenuItems.Add(menuTileDelete);

            var position = location;
            position.Offset(tile.Location.X, tile.Location.Y);
            tileMenu.Show(ParentControl, position);

            return true;
        }

    }
}
