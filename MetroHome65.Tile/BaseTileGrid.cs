using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Fleux.UIElements;
using MetroHome65.Interfaces;

namespace MetroHome65.Tile
{
    public partial class BaseTileGrid: ScrollViewer, IActive
    {
        #region Fields

        private readonly int _gridWidth;
        private readonly int _gridHeight;
        private TileTheme _tileTheme;

        private readonly List<TileWrapper> _tiles = new List<TileWrapper>();

        private readonly TilesCanvas _tilesCanvas;

        private Boolean _active = true;

        private bool _launching;

        #endregion


        // IActive
        public Boolean Active
        {
            get { return _active; }
            set { SetActive(value); }
        }

        public TileTheme TileTheme
        {
            get { return _tileTheme; }
            set
            {
                if (_tileTheme.ToString() == value.ToString()) return;
                _tileTheme = value;
                RefreshTilesGrid();
            }
        }

        protected void RefreshTilesGrid()
        {
            foreach (var tile in _tiles)
            {
                tile.TileTheme = _tileTheme;
                tile.ForceUpdate();
            }
            RealignTiles();
        }


        public BaseTileGrid(TileTheme tileTheme, UIElement background, string settingsFile, int gridWidth, int gridHeight)
        {
            _tileTheme = tileTheme;

            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
            _settingsFile = settingsFile;

            // запрет перерисовки во время скроллирования
            OnStartScroll = () => FreezeUpdate(true);
            OnStopScroll = () => FreezeUpdate(false);

            VerticalScroll = true;
            HorizontalScroll = false;

            TapHandler = GridClickHandler;
            HoldHandler = p =>
            {
                if (!SelectionMode)
                    ShowMainPopupMenu(p);
                return true;
            };

            // холст контейнер плиток
            _tilesCanvas = new TilesCanvas(background);
            Content = _tilesCanvas;

            ReadTilesSettings();
        }

        virtual protected void SetActive(Boolean active)
        {
            if (!active)
            {
                // stop scroll animation
                Pressed(new Point(-1, -1));

                // stop moving animation
                SelectedTile = null;
            }

            if (_active == active) return;
            _active = active;

            // когда активируем после запуска внешнего приложения - играем входящую анимацию
            if ((Active) && (_launching))
            {
                _launching = false;
                _tilesCanvas.AnimateEntrance();
            }

            FreezeUpdate(!_active);
            ActivateTilesAsync(_active);
        }

        // don't stop tile's animation but simple turn off redraw during animation
        // to speed-up scrolling (avoid tiles animation during scrolling)
        protected void FreezeUpdate(bool freeze)
        {
            _tilesCanvas.FreezeUpdate = freeze;
            //ActivateTilesAsync(!freeze);
            foreach (var wsInfo in _tiles)
                wsInfo.Pause = freeze;
        }

        // start/stop updatable widgets
        private void ActivateTilesAsync(bool active)
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
            }
            ).Start();
        }

    }
}
