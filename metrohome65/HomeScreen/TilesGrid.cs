using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Core;
using Fleux.Animations;
using Fleux.UIElements.Panorama;
using WindowsPhone7Sample.Elements;
using MetroHome65.Widgets;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen
{
    public partial class TilesGrid : ScrollViewer
    {
        private FleuxControl _HomeScreenControl = null;
        private List<WidgetWrapper> _tiles = new List<WidgetWrapper>();
        Canvas tilesCanvas;
        TransparentImageElement buttonSettings;
        TransparentImageElement buttonUnpin;

        private String _CoreDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);


        public TilesGrid(FleuxControl HomeScreenControl) : base()
        {
            _HomeScreenControl = HomeScreenControl;

            // кнопка настроек            
            buttonSettings = new TransparentImageElement(
                ResourceManager.Instance.GetIImageFromEmbeddedResource("settings.png")) {
                Size = new Size(48, 48),
                TapHandler = buttonSettingsClick,
            };
            // кнопка удаления плитки
            buttonUnpin = new TransparentImageElement(
                  ResourceManager.Instance.GetIImageFromEmbeddedResource("cancel.png"))
                  {
                      Size = new Size(48, 48),
                      TapHandler = buttonUnpinClick,
                  };
            RealignSettingsButtons(false);
            HomeScreenControl.AddElement(buttonUnpin);
            HomeScreenControl.AddElement(buttonSettings);

            // холст контейнер плиток
            tilesCanvas = new Canvas() { Size = new Size(400, 100) };

            Content = tilesCanvas;
            VerticalScroll = true;

            this.TapHandler = p => { GridClickAt(p); return true; };
            this.HoldHandler = p => { ShowMainPopupMenu(p); return true; };

            DebugFill();
        }

        private Boolean _Active = true;
        public Boolean Active { 
            set {
                if (_Active != value)
                {
                    _Active = value;

                    if ((_Active) && (this._launching))
                    {
                        this._launching = false;
                        this._HomeScreenControl.AnimateEntrance();
                    }

                    // start updatable widgets
                    foreach (WidgetWrapper wsInfo in _tiles)
                        wsInfo.Active = _Active;
                }
            } 
        }


        // fill grid with debug values
        private void DebugFill()
        {
            _tiles.Clear();

            String IconsDir = _CoreDir + "\\icons\\" +
                ((ScreenRoutines.IsQVGA) ? "small\\small-" : "");

            AddWidget(new Point(0, 0), new Size(2, 2), "MetroHome65.Widgets.SMSWidget", false).
                SetParameter("CommandLine", @"\Windows\tmail.exe").
                SetParameter("Caption", "SMS").
                SetParameter("IconPath", IconsDir + "message.png").
                SetParameter("TileColor", Color.Orange.ToArgb());

            AddWidget(new Point(0, 2), new Size(2, 2), "MetroHome65.Widgets.PhoneWidget", false).
                SetParameter("CommandLine", @"\Windows\cprog.exe").
                SetParameter("Caption", "Phone").
                SetParameter("IconPath", IconsDir + "phone.png").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button gray.png");

            AddWidget(new Point(2, 0), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\addrbook.lnk").
                SetParameter("Caption", "Contacts").
                SetParameter("IconPath", IconsDir + "contacts.png").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button darkgray.png");

            AddWidget(new Point(2, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\iexplore.exe").
                SetParameter("Caption", "Internet Explorer").
                SetParameter("IconPath", IconsDir + "iexplore.png").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button blue.png");

            AddWidget(new Point(0, 4), new Size(4, 2), "MetroHome65.Widgets.DigitalClockWidget", false).
                SetParameter("TileImage", _CoreDir + "\\buttons\\bg2.png");

            AddWidget(new Point(0, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\camera.exe").
                SetParameter("IconPath", @"\Windows\camera.exe").
                SetParameter("Caption", "").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button gray.png");

            AddWidget(new Point(1, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wrlsmgr.exe").
                SetParameter("IconPath", @"\Windows\wrlsmgr.exe").
                SetParameter("Caption", "").
                SetParameter("TileColor", Color.DarkGreen.ToArgb());

            AddWidget(new Point(2, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\taskmgr.exe").
                SetParameter("IconPath", @"\Windows\taskmgr.exe").
                SetParameter("Caption", "").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button blue.png"); ;

            AddWidget(new Point(3, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\calendar.exe").
                SetParameter("IconPath", @"\Windows\calendar.exe").
                SetParameter("Caption", "").
                SetParameter("TileColor", Color.Maroon.ToArgb());

            AddWidget(new Point(0, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddWidget(new Point(2, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddWidget(new Point(0, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\MobileCalculator.exe").
                SetParameter("Caption", "Calculator").
                SetParameter("IconPath", IconsDir + "calc.png");

            AddWidget(new Point(2, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wmplayer.exe").
                SetParameter("Caption", "Media player").
                SetParameter("IconPath", IconsDir + "media.png");

            AddWidget(new Point(0, 11), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\fexplore.exe").
                SetParameter("Caption", "Explorer").
                SetParameter("IconPath", IconsDir + "folder.png");

            AddWidget(new Point(2, 11), new Size(2, 2), "MetroHome65.Widgets.EMailWidget", false).
                SetParameter("CommandLine", @"\Windows\tmail.exe").
                SetParameter("Caption", "E-mail").
                SetParameter("IconPath", IconsDir + "mail.png");

            RealignWidgets();
            WriteSettings();
        }


        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        /// <param name="Widget"></param>
        public WidgetWrapper AddWidget(Point AGridPosition, Size AGridSize, String AWidgetName)
        {
            return AddWidget(AGridPosition, AGridSize, AWidgetName, true);
        }

        private WidgetWrapper AddWidget(Point AGridPosition, Size AGridSize, String AWidgetName, bool DoRealign)
        {
            WidgetWrapper Wrapper = new WidgetWrapper(AGridSize, AGridPosition, AWidgetName);

            _tiles.Add(Wrapper);
            tilesCanvas.AddElement(Wrapper);

            if (DoRealign)
            {
                RealignWidgets();
                WriteSettings();
            }

            SetExitAnimation(Wrapper);
            Wrapper.HoldHandler = p => { WidgetHoldAt(p, Wrapper); return true; };

            return Wrapper;
        }

        /// <summary>
        /// Read widgets settings from XML file
        /// </summary>
        private void ReadSettings()
        {
            //
        }

        /// <summary>
        /// Store widgets position and specific settings to XML file
        /// </summary>
        private void WriteSettings()
        {
            //
        }

        /// <summary>
        /// Fill popup menu for widget grid with grid settings
        /// </summary>
        /// <param name="Menu"></param>
        private void ShowMainPopupMenu(Point ALocation)
        {
            ContextMenu MainMenu = new ContextMenu();

            MenuItem menuAddWidget = new System.Windows.Forms.MenuItem();
            menuAddWidget.Text = "Add widget";
            menuAddWidget.Click += (s, e) => {
                AddWidget(new Point(0, 90), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget");
            };
            MainMenu.MenuItems.Add(menuAddWidget);

            // add separator
            MainMenu.MenuItems.Add(new System.Windows.Forms.MenuItem() { Text = "-", } );

            MenuItem menuExit = new System.Windows.Forms.MenuItem();
            menuExit.Text = "Exit";
            menuExit.Click += (s, e) => { Application.Exit(); };
            MainMenu.MenuItems.Add(menuExit);

            MainMenu.Show(_HomeScreenControl, ALocation);
        }

        /// <summary>
        /// Click at tile handler
        /// </summary>
        /// <param name="ALocation"></param>
        /// <param name="Widget"></param>
        private void WidgetClickAt(Point ALocation, WidgetWrapper Widget)
        {
            // if Move mode is enabled, place selected widget to the new position
            // if we click at moving widget, exit from move mode
            if (MoveMode)
            {
                // if click at moving widget - exit from moving mode
                if (Widget == MovingWidget)
                    MovingWidget = null;
                else
                // if click at another widget - change moving widget
                {
                    MovingWidget = Widget;
                }
                return;
            }

            this._launching = true;
            this._launchedTile = Widget;

            this._HomeScreenControl.AnimateExit();

            if (!Widget.OnClick(ALocation))
            {
                this._launching = false;
                this._HomeScreenControl.AnimateEntrance();
            }
        }

        /// <summary>
        /// Long tap handler - entering to customizing mode
        /// </summary>
        /// <param name="Location"></param>
        private void WidgetHoldAt(Point ALocation, WidgetWrapper Widget)
        {
            if (!MoveMode)
            {
                //!! if (!ShowWidgetPopupMenu(Widget, ALocation))
                    MovingWidget = Widget;
            }
        }

        private bool buttonSettingsClick(Point ALocation)
        {
            if (MoveMode)
                ShowTileSettings(_MovingWidget);
            return true;
        }

        private bool buttonUnpinClick(Point ALocation)
        {
            if (MoveMode)
                DeleteTile();
            return true;
        }

        /// <summary>
        /// Shows widget settings dialog and applies changes in widget settings.
        /// </summary>
        private void ShowTileSettings(WidgetWrapper Widget)
        {
            // when show setting dialog, stop selected widget animation
            WidgetWrapper prevMovingWidget = MovingWidget;
            MovingWidget = null;
            Size PrevGridSize = Widget.GridSize;

            FrmWidgetSettings WidgetSettingsForm = new FrmWidgetSettings();
            WidgetSettingsForm.Widget = Widget;
            WidgetSettingsForm.Owner = null;

            if (WidgetSettingsForm.ShowDialog() == DialogResult.OK)
            {
                Widget.Update(); // repaint widget with new style
                if (!PrevGridSize.Equals(Widget.GridSize))
                    RealignWidgets(); // if widget size changed - realign widgets
                WriteSettings();
            }
            else
                MovingWidget = prevMovingWidget;
        }

        /// <summary>
        /// Delete current widget from grid.
        /// Then widgets are realigned, if deleted widget was alone on its row.
        /// </summary>
        private void DeleteTile()
        {
            WidgetWrapper DeletingWidget = MovingWidget;
            MovingWidget = null;
            _tiles.Remove(DeletingWidget);
            DeletingWidget.Parent = null;
            RealignWidgets();
            WriteSettings();
        }

    }
}
