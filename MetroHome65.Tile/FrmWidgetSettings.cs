using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;

namespace MetroHome65.Tile
{
    public class FrmWidgetSettings : CustomSettingsPage<TileWrapper> 
        // TileWrapper doesn't really contain settings, that is stub
    {
        private readonly TileWrapper _sourceTile;
        private ITile _selectedTile;
        private readonly List<Type> _widgetTypes = new List<Type>();

        private ComboBox _cbSize;
        private ComboBox _cbType;
        private StackPanel _controlsPanel;

        private IPluginManager _pluginManager;


        public FrmWidgetSettings(TileWrapper sourceTile)
        {
            _sourceTile = sourceTile;

            for (var i = 0; i < _widgetTypes.Count; i++ )
                if (_widgetTypes[i].FullName == _sourceTile.Tile.GetType().FullName)
                {
                    if (_cbType.SelectedIndex != i)
                        _cbType.SelectedIndex = i;
                    else
                        ChangeTileType(); // manually call method if selectedindex not changed (=0)
                    break;
                }
        }

        protected override void CreateSettingsControls()
        {
            _pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            AddPage(CreateStyleSettings(), "style");
            AddPage(CreateTileSettings(), "tile");
        }

        private UIElement CreateTileSettings()
        {
            var stackPanel = new StackPanel { Size = new Size(Size.Width - SettingsConsts.PaddingHor * 2, 1), };

            var scroller = new ScrollViewer
            {
                Content = stackPanel,
                Location = new Point(SettingsConsts.PaddingHor, 0),
                ShowScrollbars = true,
                HorizontalScroll = false,
                VerticalScroll = true,
            };

            stackPanel.AddElement(
                new TextElement("Tile type") { Size = new Size(SettingsConsts.MaxWidth, 50), }
                );

            _cbType = new ComboBox { Size = new Size(SettingsConsts.MaxWidth, 50), };
            _cbType.SelectedIndexChanged += (s, e) => ChangeTileType();
            FillWidgetTypes();
            stackPanel.AddElement(_cbType);

            stackPanel.AddElement(Separator());


            stackPanel.AddElement(
                new TextElement("Tile size") { Size = new Size(SettingsConsts.MaxWidth, 50), }
                );

            _cbSize = new ComboBox { Size = new Size(SettingsConsts.MaxWidth, 50), };
            stackPanel.AddElement(_cbSize);
            // do not fill sizes here because tile is undefined. sizes will filled when tile assigned

            return scroller;
        }

        private void ChangeTileType()
        {
            if (_sourceTile == null) return;
            var widgetName = _widgetTypes[_cbType.SelectedIndex].FullName;
            var newTile = _pluginManager.CreateTile(widgetName);
            CopyTileProperties(_sourceTile.Tile, newTile);
            SetWidgetType(newTile);
        }

        private void FillWidgetTypes()
        {
            _widgetTypes.Clear();
            var items = new List<object>();

            foreach (Type plugin in _pluginManager.GetTileTypes())
            {
                // get human readable widget name for display in list
                var widgetName = "";
                var attributes = plugin.GetCustomAttributes(typeof(TileInfoAttribute), true);
                if (attributes.Length > 0)
                    widgetName = ((TileInfoAttribute)attributes[0]).Caption;
                if (widgetName == "")
                    widgetName = plugin.Name;

                items.Add(widgetName);
                _widgetTypes.Add(plugin);
            }

            _cbType.Items = items;
        }

        private void FillSizes()
        {
            var items = new List<object>();
            var selectedIndex = 0;
            var idx = 0;

            foreach (var gridSize in _selectedTile.Sizes)
            {
                items.Add(gridSize.Width + " x " + gridSize.Height);

                if (gridSize.Equals(_sourceTile.GridSize))
                    selectedIndex = idx;

                idx++;
            }

            _cbSize.Items = items;
            _cbSize.SelectedIndex = selectedIndex;
        }


        private UIElement CreateStyleSettings()
        {
            _controlsPanel = new StackPanel { Size = new Size(Size.Width - SettingsConsts.PaddingHor * 2, 1), };

            var scroller = new ScrollViewer
            {
                Content = _controlsPanel,
                Location = new Point(SettingsConsts.PaddingHor, 0),
                ShowScrollbars = true,
                HorizontalScroll = false,
                VerticalScroll = true,
            };

            return scroller;
        }

        private void SetWidgetType(ITile tile)
        {
            if ((_selectedTile == null) ||
                (((object)_selectedTile).GetType() != ((object)tile).GetType()) )
            {
                _selectedTile = tile;

                FillSizes();
                FillWidgetProperties();
            }
        }

        private void FillWidgetProperties()
        {
            _controlsPanel.Clear();

            var controls = _selectedTile.EditControls(this);
            if (controls != null)
            {
                foreach (var userControl in controls)
                {
                    _controlsPanel.AddElement(userControl);
                    _controlsPanel.AddElement(Separator());
                }
            }
        }

        private void CopyTileProperties(ITile srcTile, ITile dstTile)
        {
            try
            {
                var srcProps = ((object)srcTile).GetType().GetProperties();
                var dstProps = ((object)dstTile).GetType().GetProperties();

                // apply custom widget parameters
                foreach (var srcPropInfo in srcProps)
                {
                    // set only properties that are marked as tile parameter
                    var attributes = srcPropInfo.GetCustomAttributes(typeof(TileParameterAttribute), true);
                    if (attributes.Length > 0)
                    {
                        //foreach (var attribute in attributes)
                        //{
                            foreach (var dstPropInfo in dstProps.Where(
                                dstPropInfo => dstPropInfo.Name == srcPropInfo.Name))
                            {
                                dstPropInfo.SetValue(dstTile, Convert.ChangeType(
                                    srcPropInfo.GetValue((object)srcTile, null), dstPropInfo.PropertyType, null), null);
                                break;
                            }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, "Copy tile properties error");
            }
        }

        protected override void ApplySettings()
        {
            _sourceTile.TileClass = ((object)_selectedTile).GetType().ToString();
            _sourceTile.GridSize = _selectedTile.Sizes[_cbSize.SelectedIndex];

            CopyTileProperties(_selectedTile, _sourceTile.Tile);

            base.ApplySettings();
        }

    }
}