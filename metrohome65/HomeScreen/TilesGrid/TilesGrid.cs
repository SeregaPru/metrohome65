using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public partial class TilesGrid : ScrollViewer, IActive
    {
        private readonly List<TileWrapper> _tiles = new List<TileWrapper>();
        private readonly TilesCanvas _tilesCanvas;
        private readonly UIElement _buttonSettings;
        private readonly UIElement _buttonUnpin;
        private Boolean _active = true;

        public Action OnExit;

        public TilesGrid() : base()
        {
            // запрет перерисовки во время скроллирования
            OnStartScroll = () => FreezeUpdate(true); 
            OnStopScroll = () => FreezeUpdate(false); 

            VerticalScroll = true;

            TapHandler = GridClickHandler;
            HoldHandler = p =>
            {
                if (!MoveMode)
                    ShowMainPopupMenu(p);
                return true;
            };

            // подписка на событие добавления программы из меню
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<PinProgramMessage>(msg => PinProgram(msg.Name, msg.Path));


            // кнопка настроек            
            _buttonSettings = new ThemedImageButton("settings")
                                  {
                                      TapHandler = ButtonSettingsClick,
                                  };
            // кнопка удаления плитки
            _buttonUnpin = new ThemedImageButton("cancel")
                               {
                                   TapHandler = ButtonUnpinClick,
                               };
            RealignSettingsButtons(false);
            var parentControl = TinyIoCContainer.Current.Resolve<FleuxControlPage>().Control;
            parentControl.AddElement(_buttonUnpin);
            parentControl.AddElement(_buttonSettings);

            // холст контейнер плиток
            _tilesCanvas = new TilesCanvas { Size = new Size(400, 100) };

            Content = _tilesCanvas;

            ReadSettings();
        }

        ~TilesGrid()
        {
            ActivateTilesSync(false);
        }

        // fast drawind method instead of double bufferes scrollview's method
        // because we know that height is the whole screen and we don't neet cropping
        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            Content.Draw(
                drawingGraphics.CreateChild(new Point(0, this.VerticalOffset)));
            //base.Draw(drawingGraphics);
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
                    _tilesCanvas.AnimateEntrance();
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
            ActivateTiles(!freeze);
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
                                    ActivateTilesSync(active);
                                }
                            }
            ).Start();
        }

        private void ActivateTilesSync(bool active)
        {
            foreach (var wsInfo in _tiles)
                wsInfo.Active = active;
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
            menuExit.Click += (s, e) => OnExit(); 
            mainMenu.MenuItems.Add(menuExit);

            mainMenu.Show(TinyIoCContainer.Current.Resolve<FleuxControlPage>().Control, location);
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
        private bool TileHoldAt(Point aLocation, TileWrapper tile)
        {
            if (!MoveMode)
            {
                MovingTile = tile;
                return true;
            }
            return false;
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
