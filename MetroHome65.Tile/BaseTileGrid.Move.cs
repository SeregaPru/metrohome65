using System;
using System.Drawing;
using Fleux.Controls.Gestures;

namespace MetroHome65.Tile
{

    public partial class BaseTileGrid 
    {
        private TileWrapper _selectedTile;

        protected bool SelectionMode { get { return _selectedTile != null; } }

        private TileWrapper SelectedTile
        {
            get { return _selectedTile; }
            set
            {
                if (_selectedTile == value) return;

                // всегда при смене пермещаемого виджета, выключаем текущий перемещаемый виджет
                if (_selectedTile != null)
                {
                    _selectedTile.Moving = false;
                    _selectedTile = null;
                }

                if (value != null)
                {
                    // отключаем живые плитки
                    Active = false;

                    // но перерисовку оставляем включенной
                    FreezeUpdate(false);

                    _selectedTile = value;
                    _selectedTile.Moving = true;
                }
                else
                {
                    WriteSettings();

                    // включаем живые плитки
                    Active = true;
                }
            }
        }

        /// <summary>
        /// click handler for tiles grid - enter to moving mode
        /// </summary>
        private bool GridClickHandler(Point location)
        {
            if (SelectionMode && _tilesCanvas.Bounds.Contains(location))
            {
                MoveTileTo(SelectedTile, location);
                return true;
            }

            return false;
        }

        /// <summary>
        /// pan handler for tiles grid - in moving mode move tile to selected position
        /// </summary>
        public override bool Pan(Point from, Point to, bool done, Point startPoint)
        {
            var result = false;

            if (SelectionMode && _tilesCanvas.Bounds.Contains(to))
            {
                // в режиме перемещения плиток исключаем дребезг, срабатывание крохотного пана вместо клика
                if (Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2) <= 10)
                    return false;

                MoveTileTo(SelectedTile, to);
                result = true;
            }
            else
            {
                if (! GesturesEngine.IsHorizontal(from, to))
                {
                    FreezeUpdate(true);
                    result = base.Pan(from, to, done, startPoint);
                    if (done)
                        FreezeUpdate(false);
                }
            }

            return result;
        }

        /// <summary>
        /// return tile grid position for selected screen position
        /// </summary>
        private Point GetTileCell(Point location)
        {
            return new Point(
                (location.X - _tilesCanvas.Location.X) / (TileTheme.TileSize + TileTheme.TileSpacing),
                (location.Y - _tilesCanvas.Location.X - VerticalOffset) / (TileTheme.TileSize + TileTheme.TileSpacing)
                );
        }

        /// <summary>
        /// move selected tile to new location
        /// </summary>
        /// <param name="movingTile"> </param>
        /// <param name="location"> </param>
        private void MoveTileTo(TileWrapper movingTile, Point location)
        {
            var targetCell = GetTileCell(location);

            // if widget doesn't fit to new position by width, do not do anything
            if (targetCell.X + movingTile.GridSize.Width > _gridWidth)
                targetCell.X = _gridWidth - movingTile.GridSize.Width;
                //return;

            var targetCellIsEmpty = false;
            while (!targetCellIsEmpty)
            {
                object[,] cells = new object[_gridHeight, _gridWidth];
                foreach (TileWrapper wsInfo in _tiles)
                {
                    if (wsInfo != movingTile)
                        for (int y = 0; y < wsInfo.GridSize.Height; y++)
                            for (int x = 0; x < wsInfo.GridSize.Width; x++)
                                cells[wsInfo.GridPosition.Y + y, wsInfo.GridPosition.X + x] = wsInfo;
                }

                targetCellIsEmpty = true;
                for (int y = targetCell.Y; y < Math.Min(targetCell.Y + movingTile.GridSize.Height, 100); y++)
                    for (int x = targetCell.X; x < Math.Min(targetCell.X + movingTile.GridSize.Width, _gridWidth); x++)
                        if ((cells[y, x] != null) && (!cells[y, x].Equals(movingTile)))
                        {
                            targetCellIsEmpty = false;
                            break;
                        }

                if (!targetCellIsEmpty)
                {
                    foreach (TileWrapper wsInfo in _tiles)
                    {
                        if ((wsInfo.GridPosition.Y + wsInfo.GridSize.Height - 1) >= targetCell.Y)
                            wsInfo.GridPosition = new Point(wsInfo.GridPosition.X, wsInfo.GridPosition.Y + 1);
                    }
                }
            }

            movingTile.GridPosition = targetCell;

            RealignTiles();
        }

    }
}
