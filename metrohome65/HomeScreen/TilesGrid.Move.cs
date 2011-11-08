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
                    this.Active = (value == null);

                    /*!!
                    buttonSettings.Visible = (value != null);
                    buttonUnpin.Visible = (value != null);
                     */ 

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


        private void MoveWidgetTo(Point ALocation)
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

            RealignWidgets();
        }


    }
}
