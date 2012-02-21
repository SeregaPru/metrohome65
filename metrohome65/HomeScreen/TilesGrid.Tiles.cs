using System;
using System.Collections.Generic;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.Routines;
using System.Xml.Serialization;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen
{

    public partial class TilesGrid : ScrollViewer
    {

        // fill grid with debug values
        private void DebugFill()
        {
            _tiles.Clear();

            String IconsDir = FileRoutines.CoreDir + "\\icons\\" +
                ((ScreenRoutines.IsQVGA) ? "small\\small-" : "");

            AddTile(new Point(0, 0), new Size(2, 2), "MetroHome65.Widgets.SMSWidget", false).
                SetParameter("CommandLine", @"\Windows\tmail.exe").
                SetParameter("Caption", "SMS").
                SetParameter("IconPath", IconsDir + "message.png").
                SetParameter("TileColor", Color.Orange.ToArgb());

            AddTile(new Point(0, 2), new Size(2, 2), "MetroHome65.Widgets.PhoneWidget", false).
                SetParameter("CommandLine", @"\Windows\cprog.exe").
                SetParameter("Caption", "Phone").
                SetParameter("IconPath", IconsDir + "phone.png").
                SetParameter("TileImage", FileRoutines.CoreDir + "\\buttons\\button gray.png");

            AddTile(new Point(2, 0), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\addrbook.lnk").
                SetParameter("Caption", "Contacts").
                SetParameter("IconPath", IconsDir + "contacts.png").
                SetParameter("TileImage", FileRoutines.CoreDir + "\\buttons\\button darkgray.png");

            AddTile(new Point(2, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\iexplore.exe").
                SetParameter("Caption", "Internet Explorer").
                SetParameter("IconPath", IconsDir + "iexplore.png").
                SetParameter("TileImage", FileRoutines.CoreDir + "\\buttons\\button blue.png");

            AddTile(new Point(0, 4), new Size(4, 2), "MetroHome65.Widgets.DigitalClockWidget", false).
                SetParameter("TileImage", FileRoutines.CoreDir + "\\buttons\\bg2.png");

            AddTile(new Point(0, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\camera.exe").
                SetParameter("IconPath", @"\Windows\camera.exe").
                SetParameter("Caption", "").
                SetParameter("TileImage", FileRoutines.CoreDir + "\\buttons\\button gray.png");

            AddTile(new Point(1, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wrlsmgr.exe").
                SetParameter("IconPath", @"\Windows\wrlsmgr.exe").
                SetParameter("Caption", "").
                SetParameter("TileColor", Color.DarkGreen.ToArgb());

            AddTile(new Point(2, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\taskmgr.exe").
                SetParameter("IconPath", @"\Windows\taskmgr.exe").
                SetParameter("Caption", "").
                SetParameter("TileImage", FileRoutines.CoreDir + "\\buttons\\button blue.png"); ;

            AddTile(new Point(3, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\calendar.exe").
                SetParameter("IconPath", @"\Windows\calendar.exe").
                SetParameter("Caption", "").
                SetParameter("TileColor", Color.Maroon.ToArgb());

            AddTile(new Point(0, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddTile(new Point(2, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddTile(new Point(0, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\MobileCalculator.exe").
                SetParameter("Caption", "Calculator").
                SetParameter("IconPath", IconsDir + "calc.png");

            AddTile(new Point(2, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wmplayer.exe").
                SetParameter("Caption", "Media player").
                SetParameter("IconPath", IconsDir + "media.png");

            AddTile(new Point(0, 11), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\fexplore.exe").
                SetParameter("Caption", "Explorer").
                SetParameter("IconPath", IconsDir + "folder.png");

            AddTile(new Point(2, 11), new Size(2, 2), "MetroHome65.Widgets.EMailWidget", false).
                SetParameter("CommandLine", @"\Windows\tmail.exe").
                SetParameter("Caption", "E-mail").
                SetParameter("IconPath", IconsDir + "mail.png");

            RealignTiles();
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
        private WidgetWrapper AddTile(Point aGridPosition, Size aGridSize, String aWidgetName, bool doRealign)
        {
            return AddTile(
                new WidgetWrapper(aGridSize, aGridPosition, aWidgetName), doRealign);
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        private WidgetWrapper AddTile(WidgetWrapper tile, bool doRealign)
        {
            // draw tile when adding
            tile.Draw(null);

            _tiles.Add(tile);
            _tilesCanvas.AddElement(tile);

            if (doRealign)
            {
                RealignTiles();
                WriteSettings();
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

        #region settings file

        private String SettingsFile() { return FileRoutines.CoreDir + "\\widgets.xml"; }

        [XmlType("ArrayOfWidgetWrapper")]
        public class StoredSettings : List<WidgetWrapperSettings> {};
        public StoredSettings _storedSettings;

        /// <summary>
        /// Read widgets settings from XML file
        /// </summary>
        private void ReadSettings()
        {
            _tiles.Clear();

            try
            {
                _storedSettings = new StoredSettings();

                var serializer = new XmlSerializer(_storedSettings.GetType());
                System.IO.TextReader reader = new System.IO.StreamReader(SettingsFile());
                _storedSettings = (StoredSettings)serializer.Deserialize(reader);
                reader.Close();

                foreach (var settings in _storedSettings)
                {
                    var tile = new WidgetWrapper();
                    tile.DeserializeSettings(settings);
                    AddTile(tile, false);
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "ReadSettings error");
                DebugFill();
            }
            _storedSettings = null;

            RealignTiles();
        }

        /// <summary>
        /// Store widgets position and specific settings to XML file
        /// </summary>
        private void WriteSettings()
        {
            _storedSettings = new StoredSettings();

            foreach (var widget in _tiles)
                _storedSettings.Add(widget.SerializeSettings());

            try
            {
                var serializer = new XmlSerializer(_storedSettings.GetType());
                System.IO.TextWriter writer = new System.IO.StreamWriter(SettingsFile(), false);
                serializer.Serialize(writer, _storedSettings);
                writer.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "WriteSettings error");
            }
            _storedSettings = null;
        }

        #endregion

    }
}
