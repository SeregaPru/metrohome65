using System;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Core.Scaling;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;

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
            WriteTilesSettings();
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        /// <param name="gridPosition"> </param>
        /// <param name="gridSize"> </param>
        /// <param name="tileClass"> </param>
        /// <param name="doRealign"> </param>
        protected TileWrapper AddTile(Point gridPosition, Size gridSize, String tileClass, bool doRealign)
        {
            return AddTile(
                new TileWrapper()
                    {
                        TileTheme = _tileTheme,
                        TileClass = tileClass,
                        GridSize = gridSize,
                        GridPosition = gridPosition,
                    }, 
                doRealign);
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
            WriteTilesSettings();
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
            if (ParentControl == null)
                return false;

            SelectedTile = null;

            var tileMenu = new ContextMenu();

            var menuTileEdit = new MenuItem { Text = "Edit".Localize() };
            menuTileEdit.Click += (s, e) => ShowTileSettings(tile);
            tileMenu.MenuItems.Add(menuTileEdit);

            var menuTileMove = new MenuItem { Text = "Move".Localize() };
            menuTileMove.Click += (s, e) => { SelectedTile = tile; };
            tileMenu.MenuItems.Add(menuTileMove);

            var menuTileDelete = new MenuItem { Text = "Delete".Localize() };
            menuTileDelete.Click += (s, e) => DeleteTile(tile);
            tileMenu.MenuItems.Add(menuTileDelete);

            var position = ScreenRoutines.ScreenLocaton(tile);
            position.Offset(location.X, location.Y);
            tileMenu.Show(ParentControl, position.ToPixels());

            return true;
        }

    }
}
