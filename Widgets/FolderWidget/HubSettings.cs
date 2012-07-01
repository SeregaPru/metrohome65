using System;
using MetroHome65.Routines;
using MetroHome65.Tile;

namespace FolderWidget
{
    public class HubSettings : CustomSettings
    {
        public BaseTileGrid.StoredSettings TileSettings;

        private string _background;
        public string Background
        {
            get { return _background; }
            set { SetField(ref _background, value, "Background"); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetField(ref _title, value, "Title"); }
        }

        private string _offset;
        public string Offset
        {
            get { return _offset; }
            set { SetField(ref _offset, value, "Offset"); }
        }

    }
}