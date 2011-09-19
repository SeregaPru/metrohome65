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
            base.Paint(g, Rect);
            PaintStatuses(g, Rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle Rect) {}

        private enum StatusType {
            stBattery,
            stWiFi,
            stBluetooth
        };

        private Rectangle GetStatusRect(StatusType AStatusType)
        {
            Rectangle Rect;
            switch (AStatusType)
            {
                case StatusType.stWiFi:
                        Rect = new Rectangle(ScreenRoutines.Scale(81) + 1, 0,
                            ScreenRoutines.Scale(81), ScreenRoutines.Scale(81));
                        break;
                case StatusType.stBluetooth:
                        Rect = new Rectangle(ScreenRoutines.Scale(81) * 2 + 1, 0,
                            ScreenRoutines.Scale(81), ScreenRoutines.Scale(81));
                        break;
                default:
                        Rect = new Rectangle(0, 0, 
                            ScreenRoutines.Scale(81), ScreenRoutines.Scale(81));
                        break;
            }
            return Rect;
        }

        private void PaintStatuses(Graphics g, Rectangle Rect)
        {
            _BatteryStatus.PaintStatus(g, GetStatusRect(StatusType.stBattery));
            _WiFiStatus.PaintStatus(g, GetStatusRect(StatusType.stWiFi));
            _BluetoothStatus.PaintStatus(g, GetStatusRect(StatusType.stBluetooth));
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

            if (GetStatusRect(StatusType.stBattery).Contains(Location))
                MessageBox.Show("Click Battery");
            else
                if (GetStatusRect(StatusType.stWiFi).Contains(Location))
                    _WiFiStatus.ChangeStatus();
                else
                    if (GetStatusRect(StatusType.stBluetooth).Contains(Location))
                        _BluetoothStatus.ChangeStatus();

            OnWidgetUpdate();
        }

    }

}
