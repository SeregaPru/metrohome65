using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.HomeScreen.ProgramsMenu;
using MetroHome65.Interfaces;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public partial class TilesGrid : ScrollViewer, IActive
    {
        private TinyIoCContainer _container;
        private readonly List<TileWrapper> _tiles = new List<TileWrapper>();
        private readonly TilesCanvas _tilesCanvas;
        private readonly UIElement _buttonSettings;
        private readonly UIElement _buttonUnpin;
        private Boolean _active = true;

        public Action OnExit;

        public TilesGrid(TinyIoCContainer container) : base()
        {
            _container = container;

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
            var control = _container.Resolve(typeof (FleuxControl)) as FleuxControl;
            control.AddElement(_buttonUnpin);
            control.AddElement(_buttonSettings);

            // холст контейнер плиток
            _tilesCanvas = new TilesCanvas {Size = new Size(400, 100)};

            Content = _tilesCanvas;
            VerticalScroll = true;
            OnStartScroll = () => FreezeUpdate(true); //ActivateTiles(false);
            OnStopScroll = () => FreezeUpdate(false); //ActivateTiles(true);

            TapHandler = GridClickHandler;
            HoldHandler = p =>
                              {
                                  if (! MoveMode)
                                      ShowMainPopupMenu(p);
                                  return true;
                              };

            // подписка на событие добавления программы из меню
            var messenger = _container.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<PinProgramMessage>( msg => PinProgram(msg.Name, msg.Path) );

            ReadSettings();
        }

        // IActive
        public Boolean Active
        {
            get { return _active; }
            set
            {
                if (!value)
                {
                    // stop scroll animation
                    Pressed(new Point(-1, -1));

                    // stop moving animation
                    MovingTile = null;
                }

                if (_active == value) return;
                _active = value;

                // когда активируем после запуска внешнего приложения - играем входящую анимацию
                if ((_active) && (_launching))
                {
                    _launching = false;
                    (_container.Resolve(typeof (FleuxControl)) as FleuxControl).AnimateEntrance();
                }

                FreezeUpdate(!_active);
                ActivateTiles(_active);
            }
        }

        // don't stop tile's animation but simple turn off redraw during animation
        // to speed-up scrolling (avoid tiles animation during scrolling)
        private void FreezeUpdate(bool freeze)
        {
            _tilesCanvas.FreezeUpdate = freeze;
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
        /// <param name="location"> </param>
        private void ShowMainPopupMenu(Point location)
        {
            var mainMenu = new ContextMenu();

            var menuAddWidget = new MenuItem {Text = "Add widget"};
            menuAddWidget.Click += (s, e) => AddTileHandler(location);
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

            mainMenu.Show(_container.Resolve(typeof(FleuxControl)) as Control, location);
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
                (_container.Resolve(typeof(FleuxControl)) as FleuxControl).AnimateExit();
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
