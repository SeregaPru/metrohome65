using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Tile;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
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
            OnStartScroll = () => { FreezeUpdate(true); /*ActivateTilesAsync(false);*/ };
            OnStopScroll = () => { FreezeUpdate(false); /*ActivateTilesAsync(true);*/ };

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
                                      TapHandler = (p) => { ShowTileSettings(); return true; },
                                  };

            // кнопка удаления плитки
            _buttonUnpin = new ThemedImageButton("cancel")
                               {
                                   TapHandler = ButtonUnpinClick,
                               };
            RealignSettingsButtons(false);
            
            // холст контейнер плиток
            _tilesCanvas = new TilesCanvas();
            Content = _tilesCanvas;

            ReadSettings();
        }

        protected override void SetParentControl(FleuxControl parentControl)
        {
            base.SetParentControl(parentControl);

            if (parentControl == null) return;
            parentControl.AddElement(_buttonUnpin);
            parentControl.AddElement(_buttonSettings);
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

            if (ParentControl != null)
                mainMenu.Show(ParentControl, location);
        }

        private bool ButtonUnpinClick(Point aLocation)
        {
            if (MoveMode)
                DeleteTile();
            return true;
        }

        /// <summary>
        /// Displays widget settings dialog and applies changes in widget settings.
        /// </summary>
        private void ShowTileSettings()
        {
            if (!MoveMode) return;

            // when show setting dialog, stop selected widget animation
            var tile = MovingTile;
            MovingTile = null;
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

    }
}
