using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Routines;
using System.Xml.Serialization;

namespace MetroHome65.HomeScreen.TilesGrid
{

    public partial class TilesGrid : ScrollViewer
    {
        [XmlType("ArrayOfWidgetWrapper")]
        public class StoredSettings : List<TileWrapperSettings> {};

        /// <summary>
        /// Read widgets settings from XML file
        /// </summary>
        private void ReadSettings()
        {
            _tiles.Clear();
            var storedSettings = new StoredSettings();

            try
            {
                var serializer = new XmlSerializer(storedSettings.GetType());
                System.IO.TextReader reader = new System.IO.StreamReader(SettingsFile());
                storedSettings = (StoredSettings) serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "ReadSettings error");
                DebugFill();
            }

            foreach (var settings in storedSettings)
            {
                var tile = new TileWrapper();
                try
                {
                    tile.DeserializeSettings(settings);
                    AddTile(tile, false);
                }
                catch (Exception e)
                {
                    Logger.WriteLog(e.StackTrace, "DeserializeSettings error");
                }
            }

            RealignTiles();
        }

        /// <summary>
        /// Store widgets position and specific settings to XML file
        /// </summary>
        private void WriteSettings()
        {
            var storedSettings = new StoredSettings();

            foreach (var tile in _tiles)
                storedSettings.Add(tile.SerializeSettings());

            try
            {
                var serializer = new XmlSerializer(storedSettings.GetType());
                System.IO.TextWriter writer = new System.IO.StreamWriter(SettingsFile(), false);
                serializer.Serialize(writer, storedSettings);
                writer.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "WriteSettings error");
            }
        }

        private String SettingsFile() { return FileRoutines.CoreDir + "\\widgets.xml"; }


        // fill grid with debug values
        private void DebugFill()
        {
            _tiles.Clear();

            String iconsDir = FileRoutines.CoreDir + @"\icons\" + ((ScreenRoutines.IsQVGA) ? @"small\small-" : "");

            AddTile(new Point(0, 0), new Size(2, 2), "MetroHome65.Widgets.SMSWidget", false).
                SetParameter("CommandLine", @"\Windows\tmail.exe").
                SetParameter("Caption", "SMS").
                SetParameter("IconPath", iconsDir + "message.png").
                SetParameter("TileColor", Color.Orange.ToArgb());

            AddTile(new Point(0, 2), new Size(2, 2), "MetroHome65.Widgets.PhoneWidget", false).
                SetParameter("CommandLine", @"\Windows\cprog.exe").
                SetParameter("Caption", "Phone").
                SetParameter("IconPath", iconsDir + "phone.png").
                SetParameter("TileImage", FileRoutines.CoreDir + @"\buttons\button gray.png");

            AddTile(new Point(2, 0), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\addrbook.lnk").
                SetParameter("Caption", "Contacts").
                SetParameter("IconPath", iconsDir + "contacts.png").
                SetParameter("TileImage", FileRoutines.CoreDir + @"\buttons\button gray.png");

            AddTile(new Point(2, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\iexplore.exe").
                SetParameter("Caption", "Internet Explorer").
                SetParameter("IconPath", iconsDir + "iexplore.png").
                SetParameter("TileImage", FileRoutines.CoreDir + @"\buttons\button blue.png");

            AddTile(new Point(0, 4), new Size(4, 2), "MetroHome65.Widgets.DigitalClockWidget", false).
                SetParameter("TileImage", FileRoutines.CoreDir + @"\buttons\bg2.png");

            AddTile(new Point(0, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\camera.exe").
                SetParameter("IconPath", FileRoutines.CoreDir + @"\icons\small\small-camera.png").
                SetParameter("Caption", "").
                SetParameter("TileImage", FileRoutines.CoreDir + @"\buttons\button gray.png");

            AddTile(new Point(1, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wrlsmgr.exe").
                SetParameter("IconPath", FileRoutines.CoreDir + @"\icons\small\small-bluetooth.png").
                SetParameter("Caption", "").
                SetParameter("TileColor", Color.DarkGreen.ToArgb());

            AddTile(new Point(2, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\taskmgr.exe").
                SetParameter("IconPath", FileRoutines.CoreDir + @"\icons\small\small-gauge.png").
                SetParameter("Caption", "").
                SetParameter("TileImage", FileRoutines.CoreDir + @"\buttons\button blue.png"); 

            AddTile(new Point(3, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\calendar.exe").
                SetParameter("IconPath", FileRoutines.CoreDir + @"\icons\small\small-calendar.png").
                SetParameter("Caption", "").
                SetParameter("TileColor", Color.Maroon.ToArgb());

            AddTile(new Point(0, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddTile(new Point(2, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddTile(new Point(0, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\MobileCalculator.exe").
                SetParameter("Caption", "Calculator").
                SetParameter("IconPath", iconsDir + "calc.png");

            AddTile(new Point(2, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wmplayer.exe").
                SetParameter("Caption", "Media player").
                SetParameter("IconPath", iconsDir + "media.png");

            AddTile(new Point(0, 11), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\fexplore.exe").
                SetParameter("Caption", "Explorer").
                SetParameter("IconPath", iconsDir + "folder.png");

            AddTile(new Point(2, 11), new Size(2, 2), "MetroHome65.Widgets.EMailWidget", false).
                SetParameter("CommandLine", @"\Windows\tmail.exe").
                SetParameter("Caption", "E-mail").
                SetParameter("IconPath", iconsDir + "mail.png");

            RealignTiles();
            WriteSettings();
        }


        /// <summary>
        /// shows main settings dialog
        /// </summary>
        private Boolean ShowMainSettings()
        {
            var mainSettingsForm = new FrmMainSettings { Owner = null };
            return (mainSettingsForm.ShowDialog() == DialogResult.OK);
        }
    }
}
