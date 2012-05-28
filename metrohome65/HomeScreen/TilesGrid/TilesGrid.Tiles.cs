﻿using System;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.Tile;

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

            tile.TapHandler = p => { return TileClickAt(p, tile); };
            tile.HoldHandler = p => { return TileHoldAt(tile); };

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

        /// <summary>
        /// pin program to ftart from programs menu
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        private void PinProgram(string name, string path)
        {
            AddTile(new Point(0, 99), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", path).
                SetParameter("Caption", name).
                SetParameter("IconPath", path);

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
            if (MoveMode)
            {
                // if click at moving tile - exit from moving mode
                // if click at another tile - change moving tile to selected
                MovingTile = (tile == MovingTile) ? null : tile;
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
            if (!MoveMode)
            {
                MovingTile = tile;
                return true;
            }
            return false;
        }

    }
}