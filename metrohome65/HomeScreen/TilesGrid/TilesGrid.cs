using System;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.Controls;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using MetroHome65.Tile;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public partial class TilesGrid : BaseTileGrid
    {
        private readonly UIElement _buttonSettings;
        private readonly UIElement _buttonUnpin;

        public Action OnExit;

        public TilesGrid() : base(FileRoutines.CoreDir + @"\widgets.xml", 4, 100)
        {
            VerticalScroll = true;
            HorizontalScroll = false;

            // подписка на событие добавления программы из меню
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<PinProgramMessage>(msg => PinProgram(msg.Name, msg.Path));

            // кнопка настроек            
            _buttonSettings = new ThemedImageButton("settings")
            {
                TapHandler = p => { ShowTileSettings(); return true; },
            };

            // кнопка удаления плитки
            _buttonUnpin = new ThemedImageButton("cancel")
            {
                TapHandler = ButtonUnpinClick,
            };
            ShowSettingsButtons(false);
        }

        override protected UIElement GetBackground()
        {
            return TinyIoCContainer.Current.Resolve<ScaledBackground>();
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
        override protected ContextMenu GetMainPopupMenu(Point location)
        {
            var mainMenu = base.GetMainPopupMenu(location);

            // add separator
            mainMenu.MenuItems.Add(new MenuItem { Text = "-", });

            var menuExit = new MenuItem {Text = "Exit"};
            menuExit.Click += (s, e) => OnExit(); 
            mainMenu.MenuItems.Add(menuExit);

            return mainMenu;
        }

        private bool ButtonUnpinClick(Point aLocation)
        {
            if (SelectionMode)
                DeleteSelectedTile();
            return true;
        }

        protected override void ShowSettingsButtons(bool visible)
        {
            if (visible)
            {
                _buttonUnpin.Location = new Point(TileConsts.ArrowPosX, 150);
                _buttonSettings.Location = new Point(TileConsts.ArrowPosX, 240);
            }
            else
            {
                _buttonSettings.Location = new Point(-100, -100);
                _buttonUnpin.Location = new Point(-100, -100);
            }

            Update();
        }

        /// <summary>
        /// shows main settings dialog
        /// </summary>
        override protected Boolean ShowMainSettings()
        {
            try
            {
                var mainSettingsForm = new FrmMainSettings();

                var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
                messenger.Publish(new ShowPageMessage(mainSettingsForm));

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, "Tile settings dialog error");
                return false;
            }
        }

        /// <summary>
        /// pin program to start from programs menu
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        private void PinProgram(string name, string path)
        {
            AddTile(new Point(0, 99), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", path).
                SetParameter("Caption", name).
                SetParameter("IconPath", path);

            RealignTiles();
            WriteSettings();
        }


    }
}
