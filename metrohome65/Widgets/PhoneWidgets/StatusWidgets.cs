using System;
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

        BatteryStatus _BatteryStatus = null;
        BluetoothStatus _BluetoothStatus = null;
        WiFiStatus _WiFiStatus = null;

        /// <summary>
        /// minimal width for single status indicator for auto-layout
        /// </summary>
        private int _MinWidth = ScreenRoutines.Scale(70);

        private Size _WidgetSize;


        public StatusWidget() : base()
        {
            _BatteryStatus = new BatteryStatus();
            _BluetoothStatus = new BluetoothStatus();
            _WiFiStatus = new WiFiStatus();
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
            Pen pen = new Pen(Color.LightGray);
            Rectangle StatusRect;

            StatusRect = GetStatusRect(0);
            _BatteryStatus.PaintStatus(g, StatusRect);
            g.DrawLine(pen, StatusRect.Right, StatusRect.Top + 1, StatusRect.Right, StatusRect.Bottom - 9);

            StatusRect = GetStatusRect(1);
            _WiFiStatus.PaintStatus(g, StatusRect);
            g.DrawLine(pen, StatusRect.Right, StatusRect.Top + 1, StatusRect.Right, StatusRect.Bottom - 9);

            StatusRect = GetStatusRect(2);
            _BluetoothStatus.PaintStatus(g, StatusRect);
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
        /// count of displayed statuses
        /// </summary>
        /// <returns></returns>
        private int StatusesCount()
        {
            return 3;
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
            return _BatteryStatus.UpdateStatus() || 
                   _WiFiStatus.UpdateStatus() ||
                   _BluetoothStatus.UpdateStatus();
        }

        public override void OnClick(Point Location)
        {

            if (GetStatusRect(0).Contains(Location))
                MessageBox.Show("Click Battery");
            else
                if (GetStatusRect(1).Contains(Location))
                    _WiFiStatus.ChangeStatus();
                else
                    if (GetStatusRect(2).Contains(Location))
                        _BluetoothStatus.ChangeStatus();

            OnWidgetUpdate();
        }

    }

}
