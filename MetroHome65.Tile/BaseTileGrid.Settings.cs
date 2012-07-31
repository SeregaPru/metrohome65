using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines.File;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.Tile
{
    
    public partial class BaseTileGrid
    {
        private readonly String _settingsFile;

        public Action OnReadSettings;
        public Action OnWriteSettings;
        public Action OnShowMainSettings;


        [XmlType("ArrayOfWidgetWrapper")]
        public class StoredSettings : List<TileWrapperSettings> { };

        public StoredSettings TileSettings
        {
            get
            {
                var storedSettings = new StoredSettings();

                foreach (var tile in _tiles)
                    storedSettings.Add(tile.SerializeSettings());

                return storedSettings;
            }
            set
            {
                _tiles.Clear();
                if (value != null)
                    ApplyTilesSettings(value);
            }
        }

        /// <summary>
        /// Read tiles settings from XML file
        /// </summary>
        protected virtual void ReadTilesSettings()
        {
            if (OnReadSettings != null)
            {
                OnReadSettings();
                return;
            }

            var storedSettings = XmlHelper<StoredSettings>.Read(_settingsFile);
            if (storedSettings != null)
                TileSettings = storedSettings;
        }

        private void ApplyTilesSettings(StoredSettings storedSettings)
        {
            foreach (var settings in storedSettings)
            {
                var tile = new TileWrapper() { TileTheme = _tileTheme };
                tile.DeserializeSettings(settings);
                AddTile(tile, false);
            }

            RealignTiles();
        }

        /// <summary>
        /// Store widgets position and specific settings to XML file
        /// </summary>
        protected virtual void WriteTilesSettings()
        {
            if (OnWriteSettings != null)
            {
                OnWriteSettings();
                return;
            }

            var storedSettings = TileSettings;
            XmlHelper<StoredSettings>.Write(storedSettings, _settingsFile);
        }


        /// <summary>
        /// Update may be change widgets screen positions
        /// </summary>
        protected void RealignTiles()
        {
            try
            {
                var maxRow = GetWidgetMaxRow();

                var cells = new object[maxRow + 1, _gridWidth];
                foreach (var wsInfo in _tiles)
                {
                    for (var y = 0; y < wsInfo.GridSize.Height; y++)
                        for (var x = 0; x < wsInfo.GridSize.Width; x++)
                            cells[wsInfo.GridPosition.Y + y, Math.Min(_gridWidth - 1, wsInfo.GridPosition.X + x)] = wsInfo;
                }

                // looking for empty rows and delete them - shift widgets 1 row top
                bool rowEmpty;
                for (var row = maxRow; row >= 0; row--)
                {
                    rowEmpty = true;
                    for (int col = 0; col < _gridWidth; col++)
                        if (cells[row, col] != null)
                        {
                            rowEmpty = false;
                            break;
                        }

                    if (rowEmpty)
                    {
                        foreach (var wsInfo in _tiles.Where(wsInfo => wsInfo.GridPosition.Y > row))
                            wsInfo.GridPosition = new Point(wsInfo.GridPosition.X, wsInfo.GridPosition.Y - 1);
                    }
                }


                // calc max image dimensions for widgets grid
                var widgetsHeight = 0;
                var widgetsWidth = 0;
                foreach (var tile in _tiles)
                {
                    widgetsHeight = Math.Max(widgetsHeight, tile.Bounds.Bottom);
                    widgetsWidth = Math.Max(widgetsWidth, tile.Bounds.Right);
                    //wsInfo.Update();
                }
                widgetsHeight += 10; // add padding at bottom and blank spaces at bottom

                _tilesCanvas.Size = new Size(widgetsWidth, widgetsHeight);
                _tilesCanvas.Update();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "RealignTiles error");
            }
        }

        private int GetWidgetMaxRow()
        {
            var maxRow = 0;
            foreach (var tile in _tiles)
                maxRow = Math.Max(maxRow, tile.GridPosition.Y + tile.GridSize.Height);
            return maxRow;
        }

        /// <summary>
        /// Displays widget settings dialog and applies changes in widget settings.
        /// </summary>
        public void ShowTileSettings(TileWrapper tile)
        {
            var prevGridSize = tile.GridSize;

            try
            {
                var tileSettingsForm = new TileSettingsForm(tile);

                tileSettingsForm.OnApplySettings += (s, e) =>
                {
                    tile.Pause = false; //!! todo ?????? может не надо
                    tile.ForceUpdate(); // repaint widget with new style
                    if (!prevGridSize.Equals(tile.GridSize))
                        // if widget size was changed - realign widgets
                        RealignTiles();
                    WriteTilesSettings();
                };

                var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
                messenger.Publish(new ShowPageMessage(tileSettingsForm));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, "Tile settings dialog error");
            }
        }


        /// <summary>
        /// Fill popup menu for widget grid with grid settings
        /// </summary>
        /// <param name="location"> </param>
        virtual protected ContextMenu GetMainPopupMenu(Point location)
        {
            var mainMenu = new ContextMenu();

            var menuAddWidget = new MenuItem { Text = "Add widget" };
            menuAddWidget.Click += (s, e) => AddTile(location);
            mainMenu.MenuItems.Add(menuAddWidget);

            // add separator
            mainMenu.MenuItems.Add(new MenuItem { Text = "-", });

            var menuSetings = new MenuItem { Text = "Settings" };
            menuSetings.Click += (s, e) => ShowMainSettings();
            mainMenu.MenuItems.Add(menuSetings);

            return mainMenu;
        }

        private void ShowMainPopupMenu(Point location)
        {
            if (ParentControl != null)
                GetMainPopupMenu(location).Show(ParentControl, location);
        }

        /// <summary>
        /// shows main settings dialog
        /// </summary>
        virtual protected Boolean ShowMainSettings()
        {
            if (OnShowMainSettings != null)
            {
                OnShowMainSettings();
                return true;
            }
            return false;
        }
    }
}
