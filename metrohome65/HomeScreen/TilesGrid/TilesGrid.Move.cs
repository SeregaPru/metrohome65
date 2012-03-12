using System;
using System.Drawing;
using System.Linq;
using Fleux.Controls.Gestures;
using Fleux.Core.GraphicsHelpers;
using Fleux.Core.Scaling;
using Fleux.UIElements;
using Fleux.Animations;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen.TilesGrid
{

    public partial class TilesGrid : ScrollViewer
    {

        private TileWrapper _movingTile;

        private bool MoveMode { get { return _movingTile != null; } }

        private TileWrapper MovingTile
        {
            get { return _movingTile; }
            set
            {
                if (_movingTile == value) return;

                RealignSettingsButtons(value != null);

                // всегда при смене пермещаемого виджета, выключаем текущий перемещаемый виджет
                if (_movingTile != null)
                {
                    _movingTile.Moving = false;
                    _movingTile = null;
                }

                if (value != null)
                {
                    // отключаем живые плитки
                    Active = false;
                    // но перерисовку оставляем включенной
                    FreezeUpdate(false);

                    _movingTile = value;
                    _movingTile.Moving = true;
                }
                else
                {
                    WriteSettings();
                    // включаем живые плитки
                    Active = true;
                }
            }
        }

        private void RealignSettingsButtons(bool moveMode)
        {
            if (moveMode)
            {
                _buttonSettings.Location = new Point(410, 170);
                _buttonUnpin.Location = new Point(410, 100);
            }
            else
            {
                _buttonSettings.Location = new Point(-100, -100);
                _buttonUnpin.Location = new Point(-100, -100);
            }
        }

        /*
        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            this.Content.Draw(drawingGraphics.CreateChild(new Point(this.HorizontalOffset, this.VerticalOffset)));
            //base.Draw(drawingGraphics);
        }
        */ 

        /// <summary>
        /// click handler for tiles grid - enter to moving mode
        /// </summary>
        private bool GridClickHandler(Point location)
        {
            if (MoveMode && _tilesCanvas.Bounds.Contains(location))
            {
                MoveTileTo(MovingTile, location);
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

            if (MoveMode && _tilesCanvas.Bounds.Contains(to))
            {
                // в режиме перемещения плиток исключаем дребезг, срабатывание крохотного пана вместо клика
                if (Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2) <= 10)
                    return false;

                MoveTileTo(MovingTile, to);
                result = true;
            }
            else
            {
                if (! GesturesEngine.IsHorizontal(from, to))
                {
                    FreezeUpdate(true);
                    result = base.Pan(from, to, done, startPoint);
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
                (location.X - _tilesCanvas.Location.X) / (TileWrapper.CellWidth + TileWrapper.CellSpacingHor),
                (location.Y - _tilesCanvas.Location.X - VerticalOffset) / (TileWrapper.CellHeight + TileWrapper.CellSpacingVer)
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
            if (targetCell.X + movingTile.GridSize.Width > 4)
                targetCell.X = 4 - movingTile.GridSize.Width;
                //return;

            var targetCellIsEmpty = false;
            while (!targetCellIsEmpty)
            {
                object[,] cells = new object[100, 4];
                foreach (TileWrapper wsInfo in _tiles)
                {
                    if (wsInfo != movingTile)
                        for (int y = 0; y < wsInfo.GridSize.Height; y++)
                            for (int x = 0; x < wsInfo.GridSize.Width; x++)
                                cells[wsInfo.GridPosition.Y + y, wsInfo.GridPosition.X + x] = wsInfo;
                }

                targetCellIsEmpty = true;
                for (int y = targetCell.Y; y < Math.Min(targetCell.Y + movingTile.GridSize.Height, 100); y++)
                    for (int x = targetCell.X; x < Math.Min(targetCell.X + movingTile.GridSize.Width, 4); x++)
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


        private int GetWidgetMaxRow()
        {
            var maxRow = 0;
            foreach (TileWrapper wsInfo in _tiles)
                maxRow = Math.Max(maxRow, wsInfo.GridPosition.Y + wsInfo.GridSize.Height);
            return maxRow;
        }

            /// <summary>
        /// Update may be change widgets screen positions
        /// </summary>
        private void RealignTiles()
        {
            try
            {
                var maxRow = GetWidgetMaxRow();

                var cells = new object[maxRow + 1, 4];
                foreach (var wsInfo in _tiles)
                {
                    for (var y = 0; y < wsInfo.GridSize.Height; y++)
                        for (var x = 0; x < wsInfo.GridSize.Width; x++)
                            cells[wsInfo.GridPosition.Y + y, Math.Min(3, wsInfo.GridPosition.X + x)] = wsInfo;
                }

                // looking for empty rows and delete them - shift widgets 1 row top
                for (var row = maxRow; row >= 0; row--)
                {
                    if ((cells[row, 0] == null) && (cells[row, 1] == null) &&
                        (cells[row, 2] == null) && (cells[row, 3] == null))
                    {
                        foreach (var wsInfo in _tiles.Where(wsInfo => wsInfo.GridPosition.Y > row))
                            wsInfo.GridPosition = new Point(wsInfo.GridPosition.X, wsInfo.GridPosition.Y - 1);
                    }
                }


                // calc max image dimensions for widgets grid
                var widgetsHeight = 0;
                var widgetsWidth = 0;
                foreach (var wsInfo in _tiles)
                {
                    widgetsHeight = Math.Max(widgetsHeight, wsInfo.Bounds.Bottom);
                    widgetsWidth = Math.Max(widgetsWidth, wsInfo.Bounds.Right);
                    //wsInfo.Update();
                }
                widgetsHeight += 50; // add padding at bottom and blank spaces at top and bottom

                _tilesCanvas.Size = new Size(_tilesCanvas.Size.Width, widgetsHeight);
                _tilesCanvas.Update();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "RealignTiles error");
            }
        }


        #region Animation

        private bool _launching = false;

        private void SetEntranceAnimation(TileWrapper target)
        {
            var random = new Random();
            var x = target.GetScreenRect().Left;
            target.EntranceAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                From = x - 1000 + random.Next(1000 - x - 173),
                To = x,
                OnAnimation = v => target.Location = new Point(v, target.Location.Y),
                OnAnimationStart = () => { target.Active = false; },
                OnAnimationStop = () => { target.Active = true; }
            };
        }

        private void SetExitAnimation(TileWrapper target)
        {
            var random = new Random();
            target.ExitAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                To = -target.Size.Width - random.Next(1000),
                From = target.GetScreenRect().Left,
                OnAnimation = v => target.Location = new Point(v, target.Location.Y),
                OnAnimationStart = () => { target.Active = false; },
                OnAnimationStop = () => { }
            };
        }

        #endregion


    }
}
