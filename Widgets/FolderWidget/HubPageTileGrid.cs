using System;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using MetroHome65.Tile;
using TinyIoC;
using TinyMessenger;

namespace FolderWidget
{
    public class HubPageTileGrid : BaseTileGrid
    {
        //?? strange for me that I should override empty constructor with parameters
        public HubPageTileGrid(UIElement background, string settingsFile, int gridWidth, int gridHeight) 
            : base(background, settingsFile, gridWidth, gridHeight)
        {
        }

        override protected Point GetPadding()
        {
            return new Point(TileConsts.TilesPaddingLeft, 0);
        }

    }
}