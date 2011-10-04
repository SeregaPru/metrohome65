using System;
using System.Collections.Generic;
using System.Drawing;
using MetroHome65.Routines;
using MetroHome65.Widgets.StatusWidget;
using System.Windows.Forms;

namespace MetroHome65.Widgets.StatusWidget
{
    [WidgetInfo("Statuses")]
    public class StatusWidget : ShortcutWidget, IWidgetUpdatable
    {
        private System.Windows.Forms.Timer _Timer;

        List<CustomStatus> _Statuses = new List<CustomStatus>();

        /// <summary>
        /// minimal width for single status indicator for auto-layout
        /// </summary>
        private static int _MinWidth = ScreenRoutines.Scale(49);

        private Size _WidgetSize;


        public StatusWidget() : base()
        {
            _Statuses.Add(new BatteryStatus());
            _Statuses.Add(new BluetoothStatus());
            _Statuses.Add(new WiFiStatus());
            _Statuses.Add(new RotationStatus());
        }

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(4, 1) 
            };
            return sizes;
        }
        
        public override void Paint(Graphics g, Rectangle Rect)
        {
            _WidgetSize = Rect.Size;

            base.Paint(g, Rect);
            PaintStatuses(g, Rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle Rect) {}

        private enum StatusType {
            stBattery,
            stWiFi,
            stBluetooth
        };

        private void PaintStatuses(Graphics g, Rectangle Rect)
        {
            Pen pen = new Pen(Color.Gray);
            Rectangle StatusRect;

            for (int i = 0; i < _Statuses.Count; i++)
            {
                StatusRect = GetStatusRect(i);
                _Statuses[i].PaintStatus(g, StatusRect);
                if (i < _Statuses.Count - 1)
                    g.DrawLine(pen, StatusRect.Right, StatusRect.Top + 1, StatusRect.Right, StatusRect.Bottom - 1);
            }
        }

        private Rectangle GetStatusRect(int Position)
        {
            int StatusWidth = 0;
            int StatusCount = StatusesCount();
            while (StatusWidth < _MinWidth)
            {
                StatusWidth = _WidgetSize.Width / StatusCount;
                StatusCount--;
            }

            Rectangle StatusRect = new Rectangle(
                StatusWidth * Position + Position, 0, 
                StatusWidth, _WidgetSize.Height);
            return StatusRect;
        }

        /// <summary>
        /// Get count of registered statuses
        /// </summary>
        /// <returns></returns>
        private int StatusesCount()
        {
            return _Statuses.Count;
        }

        public void StartUpdate()
        {
            if (_Timer == null)
            {
                _Timer = new System.Windows.Forms.Timer();
                _Timer.Tick += new EventHandler(OnTimer);
            }
            _Timer.Interval = 3000;
            _Timer.Enabled = true;
        }

        public void StopUpdate()
        {
            if (_Timer != null)
                _Timer.Enabled = false;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            if (UpdateStatuses())
            {
                OnWidgetUpdate();
            }
        }

        private bool UpdateStatuses()
        {
            Boolean Result = false;
            foreach(CustomStatus Status in _Statuses)
            {
                Result = Result || Status.UpdateStatus();
                if (Result) break;
            }
            return Result;
        }

        public override void OnClick(Point Location)
        {
            for (int i = 0; i < _Statuses.Count; i++)
            {
                if (GetStatusRect(i).Contains(Location))
                {
                    _Statuses[i].ChangeStatus();
                    OnWidgetUpdate();
                    break;
                }
            }
        }


    }

}
