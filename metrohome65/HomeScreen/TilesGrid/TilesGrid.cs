﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.UIElements;
using MetroHome65.Interfaces.Events;
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

        public TilesGrid()
            : base(GetBackground(), FileRoutines.CoreDir + @"\widgets.xml", 4, 100)
        {
            // подписка на событие добавления программы из меню
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<PinProgramMessage>(msg => PinProgram(msg.Name, msg.Path));
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

            var menuExit = new MenuItem {Text = "Exit"};
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
            WriteSettings();
        }

        override protected Point GetPadding()
        {
            return new Point(TileConsts.TilesPaddingLeft, TileConsts.TilesPaddingTop);
        }

        // fast drawind method instead of double bufferes scrollview's method
        // because we know that height is the whole screen and we don't neet cropping
        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            Content.Draw(drawingGraphics.CreateChild(new Point(0, VerticalOffset)));
        }

    }
}
