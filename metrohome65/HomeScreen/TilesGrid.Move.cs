using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Core;
using Fleux.Animations;
using Fleux.UIElements.Panorama;
using WindowsPhone7Sample.Elements;

namespace MetroHome65.HomeScreen
{

    public partial class TilesGrid : ScrollViewer
    {

        private WidgetWrapper _MovingWidget = null;
        private Rectangle _MovingWidgetBounds;

        private bool MoveMode { get { return _MovingWidget != null; } }

        private WidgetWrapper MovingWidget
        {
            get { return _MovingWidget; }
            set
            {
                if (_MovingWidget != value)
                {
                    RealignSettingsButtons(value != null);

                    if (value == null)
                    {
                        _ResizeTimer.Dispose();
                        _ResizeTimer = null;

                        _MovingWidget.Moving = false;
                        // restore previous moving widget size
                        _MovingWidget.Location = _MovingWidgetBounds.Location;
                        _MovingWidget.Size = _MovingWidgetBounds.Size;
                        _MovingWidget.Update();

                        WriteSettings();
                    }
                    else
                    {
                        // restore previous movin widget size
                        if (_MovingWidget != null)
                        {
                            _MovingWidget.Location = _MovingWidgetBounds.Location;
                            _MovingWidget.Size = _MovingWidgetBounds.Size;
                        }

                        _MovingWidget = value;
                        _MovingWidgetBounds = _MovingWidget.Bounds;
                        _MovingWidget.Moving = true;

                        _ResizeTimer = new System.Threading.Timer(s => RepaintMovingWidget(), null, 0, 300);
                    }

                }
            }
        }

        private void RealignSettingsButtons(bool MoveMode)
        {
            if (MoveMode)
            {
                buttonSettings.Location = new Point(410, 170);
                buttonUnpin.Location = new Point(410, 100);
            }
            else
            {
                buttonSettings.Location = new Point(-100, -100);
                buttonUnpin.Location = new Point(-100, -100);
            }
        }

        private System.Threading.Timer _ResizeTimer;
        
        private int _deltaX = 0;
        private int _deltaY = -2;
        private int _deltaXInc = 2;
        private int _deltaYInc = -2;


        private void RepaintMovingWidget()
        {
            if (MovingWidget == null)
                return;

            Rectangle _MovingWidgetRect = _MovingWidgetBounds;
            _MovingWidgetRect.Inflate(_deltaX, _deltaY);

            if ((_deltaX >= 0) || (_deltaX <= -5))
                _deltaXInc = -_deltaXInc;
            _deltaX += _deltaXInc;
            if ((_deltaY >= 0) || (_deltaY <= -5))
                _deltaYInc = -_deltaYInc;
            _deltaY += _deltaYInc;

            // paint widget
            MovingWidget.Location = _MovingWidgetRect.Location;
            MovingWidget.Size = _MovingWidgetRect.Size;
            MovingWidget.Update();
        }

        /// <summary>
        /// click handler for tiles grid - enter to moving mode
        /// </summary>
        /// <param name="ALocation"></param>
        private void GridClickAt(Point ALocation)
        {
            if (MoveMode)
                MoveTileTo(ALocation);
        }

        /// <summary>
        /// move selected tile to new location
        /// </summary>
        /// <param name="ALocation"></param>
        private void MoveTileTo(Point ALocation)
        {
            Point TargetCell = new Point(
                (ALocation.X - tilesCanvas.Location.X + WidgetWrapper.CellSpacingHor) / (WidgetWrapper.CellWidth + WidgetWrapper.CellSpacingHor),
                (ALocation.Y - tilesCanvas.Location.X + WidgetWrapper.CellSpacingVer - this.VerticalOffset) / (WidgetWrapper.CellHeight + WidgetWrapper.CellSpacingVer)
                );

            // if widget doesn't fit to new position by width, do not do anything
            if (TargetCell.X + MovingWidget.GridSize.Width > 4)
                return;

            bool TargetCellIsEmpty = false;
            while (!TargetCellIsEmpty)
            {
                object[,] cells = new object[100, 4];
                foreach (WidgetWrapper wsInfo in _tiles)
                {
                    for (int y = 0; y < wsInfo.GridSize.Height; y++)
                        for (int x = 0; x < wsInfo.GridSize.Width; x++)
                            cells[wsInfo.GridPosition.Y + y, wsInfo.GridPosition.X + x] = wsInfo;
                }

                TargetCellIsEmpty = true;
                for (int y = TargetCell.Y; y < Math.Min(TargetCell.Y + MovingWidget.GridSize.Height, 100); y++)
                    for (int x = TargetCell.X; x < Math.Min(TargetCell.X + MovingWidget.GridSize.Width, 4); x++)
                        if ((cells[y, x] != null) && (!cells[y, x].Equals(MovingWidget)))
                        {
                            TargetCellIsEmpty = false;
                            break;
                        }

                if (!TargetCellIsEmpty)
                {
                    foreach (WidgetWrapper wsInfo in _tiles)
                    {
                        if ((wsInfo.GridPosition.Y + wsInfo.GridSize.Height - 1) >= TargetCell.Y)
                            wsInfo.GridPosition = new Point(wsInfo.GridPosition.X, wsInfo.GridPosition.Y + 1);
                    }
                }
            }

            MovingWidget.GridPosition = TargetCell;
            _MovingWidgetBounds = MovingWidget.Bounds;

            RealignWidgets();
        }


        /// <summary>
        /// Update may be change widgets screen positions
        /// </summary>
        private void RealignWidgets()
        {
            try
            {
                int MaxRow = 0;
                foreach (WidgetWrapper wsInfo in _tiles)
                    MaxRow = Math.Max(MaxRow, wsInfo.GridPosition.Y + wsInfo.GridSize.Height);

                object[,] cells = new object[MaxRow + 1, 4];
                foreach (WidgetWrapper wsInfo in _tiles)
                {
                    for (int y = 0; y < wsInfo.GridSize.Height; y++)
                        for (int x = 0; x < wsInfo.GridSize.Width; x++)
                            cells[wsInfo.GridPosition.Y + y, Math.Min(3, wsInfo.GridPosition.X + x)] = wsInfo;
                }

                // looking for empty rows and delete them - shift widgets 1 row top
                for (int row = MaxRow; row >= 0; row--)
                {
                    if ((cells[row, 0] == null) && (cells[row, 1] == null) &&
                        (cells[row, 2] == null) && (cells[row, 3] == null))
                    {
                        foreach (WidgetWrapper wsInfo in _tiles)
                            if (wsInfo.GridPosition.Y > row)
                                wsInfo.GridPosition = new Point(wsInfo.GridPosition.X, wsInfo.GridPosition.Y - 1);
                    }
                }


                // calc max image dimensions for widgets grid
                int WidgetsHeight = 0;
                int WidgetsWidth = 0;
                foreach (WidgetWrapper wsInfo in _tiles)
                {
                    WidgetsHeight = Math.Max(WidgetsHeight, wsInfo.Bounds.Bottom);
                    WidgetsWidth = Math.Max(WidgetsWidth, wsInfo.Bounds.Right);
                    wsInfo.Update();
                }
                WidgetsHeight += 50; // add padding at bottom and blank spaces at top and bottom

                tilesCanvas.Size = new Size(tilesCanvas.Size.Width, WidgetsHeight);
                tilesCanvas.Update();
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "ReadSettings")
            }
        }

    }
}
