using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.Tile
{
    public partial class BaseTileGrid
    {

        [XmlType("ArrayOfWidgetWrapper")]
        public class StoredSettings : List<TileWrapperSettings> { };

        /// <summary>
        /// Read widgets settings from XML file
        /// </summary>
        protected virtual void ReadSettings()
        {
            _tiles.Clear();
            var storedSettings = new StoredSettings();

            try
            {
                var serializer = new XmlSerializer(storedSettings.GetType());
                System.IO.TextReader reader = new System.IO.StreamReader(_settingsFile);
                storedSettings = (StoredSettings)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "ReadSettings error");
            }

            foreach (var settings in storedSettings)
            {
                var tile = new TileWrapper(GetPadding());
                tile.DeserializeSettings(settings);
                AddTile(tile, false);
            }

            RealignTiles();
        }

        /// <summary>
        /// Store widgets position and specific settings to XML file
        /// </summary>
        protected virtual void WriteSettings()
        {
            var storedSettings = new StoredSettings();

            foreach (var tile in _tiles)
                storedSettings.Add(tile.SerializeSettings());

            try
            {
                var serializer = new XmlSerializer(storedSettings.GetType());
                System.IO.TextWriter writer = new System.IO.StreamWriter(_settingsFile, false);
                serializer.Serialize(writer, storedSettings);
                writer.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "WriteSettings error");
            }
        }

        private String _settingsFile;

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

                //!!TileConsts.TilesPaddingLeft + TileConsts.TileSize * 4 + TileConsts.TileSpacing * 3
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
        protected void ShowTileSettings()
        {
            if (!SelectionMode) return;

            // when show setting dialog, stop selected widget animation
            var tile = SelectedTile;
            SelectedTile = null;
            var prevGridSize = tile.GridSize;

            try
            {
                var widgetSettingsForm = new FrmWidgetSettings(tile);

                widgetSettingsForm.OnApplySettings += (s, e) =>
                {
                    tile.Pause = false; //!! todo ?????? может не надо
                    tile.ForceUpdate(); // repaint widget with new style
                    if (!prevGridSize.Equals(tile.GridSize))
                        // if widget size changed - realign widgets
                        RealignTiles();
                    WriteSettings();
                };

                var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
                messenger.Publish(new ShowPageMessage(widgetSettingsForm));
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
            menuAddWidget.Click += (s, e) => AddTileHandler(location);
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
            return false;
        }

        virtual protected Point GetPadding()
        {
            return new Point(0, 0);
        }
    }
}
