using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public partial class TilesGrid : ScrollViewer, IActive
    {
        private readonly FleuxControl _homeScreenControl;
        private readonly List<TileWrapper> _tiles = new List<TileWrapper>();
        private readonly Canvas _tilesCanvas;
        private readonly UIElement _buttonSettings;
        private readonly UIElement _buttonUnpin;
        private Boolean _active = true;

        public Action OnExit;

        public TilesGrid(FleuxControl homeScreenControl) : base()
        {
            _homeScreenControl = homeScreenControl;

            // кнопка настроек            
            _buttonSettings = new FlatButton("MetroHome65.Images.settings.png")
                                  {
                                      Size = new Size(48, 48),
                                      TapHandler = ButtonSettingsClick,
                                  };
            // кнопка удаления плитки
            _buttonUnpin = new FlatButton("MetroHome65.Images.cancel.png")
                               {
                                   Size = new Size(48, 48),
                                   TapHandler = ButtonUnpinClick,
                               };
            RealignSettingsButtons(false);
            homeScreenControl.AddElement(_buttonUnpin);
            homeScreenControl.AddElement(_buttonSettings);

            // холст контейнер плиток
            _tilesCanvas = new Canvas {Size = new Size(400, 100)};

            Content = _tilesCanvas;
            VerticalScroll = true;
            OnStartScroll = () => ActivateTiles(false);
            OnStopScroll = () => ActivateTiles(true);

            TapHandler = GridClickHandler;
            HoldHandler = p =>
                              {
                                  if (! MoveMode)
                                      ShowMainPopupMenu(p);
                                  return true;
                              };

            ReadSettings();
        }

        // IActive
        public Boolean Active
        {
            get { return _active; }
            set
            {
                if (_active == value) return;

                _active = value;

                if (! _active)
                {
                    // stop scroll animation
                    Pressed(new Point(-1, -1));

                    // stop moving animation
                    MovingTile = null;
                }

                // когда активируем после запуска внешнего приложения - играем входящую анимацию
                if ((_active) && (_launching))
                {
                    _launching = false;
                    _homeScreenControl.AnimateEntrance();
                }

                ActivateTiles(_active);
            }
        }

        // start/stop updatable widgets
        private void ActivateTiles(bool active)
        {
            new Thread(() =>
            {
                // lock asynchronous activisation
                // for sequental runing activation - deactivation
                lock (this)
                {
                    foreach (var wsInfo in _tiles)
                        wsInfo.Active = active;
                }
            }).Start();
        }

        /// <summary>
        /// Fill popup menu for widget grid with grid settings
        /// </summary>
        /// <param name="aLocation"> </param>
        private void ShowMainPopupMenu(Point aLocation)
        {
            var mainMenu = new ContextMenu();

            var menuAddWidget = new MenuItem {Text = "Add widget"};
            menuAddWidget.Click += (s, e) => AddTileHandler(aLocation);
            mainMenu.MenuItems.Add(menuAddWidget);

            // add separator
            mainMenu.MenuItems.Add(new MenuItem {Text = "-",});

            var menuSetings = new MenuItem { Text = "Settings" };
            menuSetings.Click += (s, e) => ShowMainSettings();
            mainMenu.MenuItems.Add(menuSetings);

            // add separator
            mainMenu.MenuItems.Add(new MenuItem { Text = "-", });

            var menuExit = new MenuItem {Text = "Exit"};
            menuExit.Click += (s, e) => OnExit(); //Application.Exit();
            mainMenu.MenuItems.Add(menuExit);

            mainMenu.Show(_homeScreenControl, aLocation);
        }

        /// <summary>
        /// Click at tile handler
        /// </summary>
        /// <param name="aLocation"></param>
        /// <param name="tile"> </param>
        private void TileClickAt(Point aLocation, TileWrapper tile)
        {
            // if Move mode is enabled, place selected widget to the new position
            // if we click at moving widget, exit from move mode
            if (MoveMode)
            {
                // if click at moving tile - exit from moving mode
                // if click at another tile - change moving tile to selected
                MovingTile = (tile == MovingTile) ? null : tile;
                return;
            }

            // if tile launches external program, start exit animation for visible tiles
            if (tile.Tile.AnimateExit)
            {
                Active = false;

                var scrollRect = new Rectangle(0, -VerticalOffset, Bounds.Width, Bounds.Height);

                foreach (var curTile in _tiles)
                {
                    if (curTile.GetScreenRect().IntersectsWith(scrollRect))
                    {
                        SetExitAnimation(curTile);
                        SetEntranceAnimation(curTile);
                    }
                    else
                    {
                        curTile.ExitAnimation = null;
                        curTile.EntranceAnimation = null;
                    }
                }
                _launching = true;
                _homeScreenControl.AnimateExit();
            }

            var clickResult = tile.OnClick(aLocation);

            if ((tile.Tile.AnimateExit) && (!clickResult))
            {
                Active = true;
                // when activate plays entrance animation
            }
        }

        /// <summary>
        /// Long tap handler - entering to customizing mode
        /// </summary>
        private void TileHoldAt(Point aLocation, TileWrapper tile)
        {
            if (!MoveMode)
                MovingTile = tile;
        }

        private bool ButtonSettingsClick(Point aLocation)
        {
            if (MoveMode)
            {
                // when show setting dialog, stop selected widget animation
                var tile = MovingTile;
                MovingTile = null;
                var prevGridSize = tile.GridSize;

                if (ShowTileSettings(tile))
                {
                    tile.ForceUpdate(); // repaint widget with new style
                    if (!prevGridSize.Equals(tile.GridSize))
                        RealignTiles(); // if widget size changed - realign widgets
                    WriteSettings();
                }
                else
                    MovingTile = tile;

            }
            return true;
        }

        private bool ButtonUnpinClick(Point aLocation)
        {
            if (MoveMode)
                DeleteTile();
            return true;
        }

        /// <summary>
        /// Shows widget settings dialog and applies changes in widget settings.
        /// </summary>
        private bool ShowTileSettings(TileWrapper tile)
        {
            var widgetSettingsForm = new FrmWidgetSettings
                                         {
                                             Tile = tile, Owner = null
                                         };
            return (widgetSettingsForm.ShowDialog() == DialogResult.OK);
        }

    }
}
