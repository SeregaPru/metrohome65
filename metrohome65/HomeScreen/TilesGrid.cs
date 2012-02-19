﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.Core.Scaling;
using Fleux.UIElements;
using Fleux.Core;

namespace MetroHome65.HomeScreen
{
    public partial class TilesGrid : ScrollViewer
    {
        private readonly FleuxControl _homeScreenControl;
        private readonly List<WidgetWrapper> _tiles = new List<WidgetWrapper>();
        private readonly Canvas _tilesCanvas;
        private readonly TransparentImageElement _buttonSettings;
        private readonly TransparentImageElement _buttonUnpin;

        public TilesGrid(FleuxControl homeScreenControl)
            : base()
        {
            _homeScreenControl = homeScreenControl;

            // кнопка настроек            
            _buttonSettings = new TransparentImageElement(
                ResourceManager.Instance.GetIImageFromEmbeddedResource("settings.png"))
                                  {
                                      Size = new Size(48, 48),
                                      TapHandler = ButtonSettingsClick,
                                  };
            // кнопка удаления плитки
            _buttonUnpin = new TransparentImageElement(
                ResourceManager.Instance.GetIImageFromEmbeddedResource("cancel.png"))
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

            TapHandler = GridClickHandler;
            HoldHandler = p =>
                              {
                                  if (! MoveMode)
                                      ShowMainPopupMenu(p);
                                  return true;
                              };

            ReadSettings();
        }

        private Boolean _active = true;

        public Boolean Active
        {
            set
            {
                if (_active == value) return;

                _active = value;

                MovingTile = null;

                if ((_active) && (_launching))
                {
                    _launching = false;
                    _homeScreenControl.AnimateEntrance();
                }

                // start updatable widgets
                foreach (var wsInfo in _tiles)
                    wsInfo.Active = _active;
            }
        }

        /// <summary>
        /// Fill popup menu for widget grid with grid settings
        /// </summary>
        /// <param name="aLocation"> </param>
        private void ShowMainPopupMenu(Point aLocation)
        {
            var mainMenu = new ContextMenu();

            var menuAddWidget = new MenuItem {Text = "Add widget"};
            menuAddWidget.Click += (s, e) => AddTileClick(aLocation);
            mainMenu.MenuItems.Add(menuAddWidget);

            // add separator
            mainMenu.MenuItems.Add(new MenuItem {Text = "-",});

            var menuExit = new MenuItem {Text = "Exit"};
            menuExit.Click += (s, e) => Application.Exit();
            mainMenu.MenuItems.Add(menuExit);

            mainMenu.Show(_homeScreenControl, aLocation);
        }

        private void AddTileClick(Point aLocation)
        {
            Point cell = GetTileCell(aLocation);
            if (cell.X + 2 > 4)
                cell.X = 2;
            MoveTileTo(
                AddTile(cell, new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", true),
                aLocation);
        }

        /// <summary>
        /// Click at tile handler
        /// </summary>
        /// <param name="aLocation"></param>
        /// <param name="tile"> </param>
        private void TileClickAt(Point aLocation, WidgetWrapper tile)
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

            if (tile.Widget.AnimateExit)
            {
                Active = false;

                var scrollRect = Bounds;
                scrollRect.Offset(0, -VerticalOffset);

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

            bool clickResult = tile.OnClick(aLocation);

            if ((tile.Widget.AnimateExit) && (!clickResult))
            {
                Active = true;
                // when activate plays entrance animation
            }
        }

        /// <summary>
        /// Long tap handler - entering to customizing mode
        /// </summary>
        private void TileHoldAt(Point aLocation, WidgetWrapper tile)
        {
            if (!MoveMode)
            {
                //!! if (!ShowWidgetPopupMenu(Widget, ALocation))
                MovingTile = tile;
            }
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
        private bool ShowTileSettings(WidgetWrapper tile)
        {
            var widgetSettingsForm = new FrmWidgetSettings
                                         {
                                             Widget = tile,
                                             Owner = null
                                         };

            return (widgetSettingsForm.ShowDialog() == DialogResult.OK);
        }

    }
}
