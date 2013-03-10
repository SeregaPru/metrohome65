using System;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.UIElements;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using MetroHome65.Routines.File;
using MetroHome65.Routines.UIControls;
using MetroHome65.Tile;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public partial class TilesGrid : BaseTileGrid
    {

        public Action OnExit;
        private string _str_exit = "Exit";


        public TilesGrid(TileTheme tileTheme)
            : base(tileTheme, GetBackground(), "", 4, 100)
        {
            // подписка на событие добавления программы из меню
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<PinProgramMessage>(msg => PinProgram(msg.Name, msg.Path));

            // подписываемся на событие смены настроек для отлова смены темы
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);

            SizeChanged += (sender, args) => OnSizeChanged();
        }

        // diffirent tile settings files for different themes
        protected override string GetSettingsFile()
        {
            return string.Format(@"{0}\settings\widgets-{1}.xml", FileRoutines.CoreDir, TileTheme.ThemeIdent);
        }

        private static UIElement GetBackground()
        {
            return TinyIoCContainer.Current.Resolve<ScaledBackground>();
        }

        protected override void SetParentControl(FleuxControl parentControl)
        {
            base.SetParentControl(parentControl);

            if (parentControl == null) return;
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

            var menuExit = new MenuItem { Text = _str_exit.Localize() };
            menuExit.Click += (s, e) => OnExit(); 
            mainMenu.MenuItems.Add(menuExit);

            return mainMenu;
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
            WriteTilesSettings();
        }

        // fast drawind method instead of double bufferes scrollview's method
        // because we know that height is the whole screen and we don't neet cropping
        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            Content.Draw(drawingGraphics.CreateChild(new Point(0, VerticalOffset)));
        }

        private void OnSettingsChanged(SettingsChangedMessage settingsChangedMessage)
        {
            if (settingsChangedMessage.PropertyName == "TileTheme")
            {
                TileTheme = settingsChangedMessage.Value as TileTheme;
            }
        }

        protected override void RefreshTilesGrid()
        {
            Clear();
            ReadTilesSettings();
        }

        private void OnSizeChanged()
        {
            // при изменении собственного размера надо пересчитать настройки скролла
            if (Content == null) return;

            var oldSize = Content.Size;
            Content.Size = new Size(10, 10); // делаем размер достаочно большой чтоб при масштабировании он не превратился в 0
            Content.Size = oldSize;
        }

    }
}
