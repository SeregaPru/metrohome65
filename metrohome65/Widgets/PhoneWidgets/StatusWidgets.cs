using System;
using System.Drawing;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [WidgetInfo("Battery status")]
    public class BatteryStatusWidget : ShortcutWidget, IWidgetUpdatable
    {
        private System.Windows.Forms.Timer _Timer;
        private string _BatteryStatus = "";

        public BatteryStatusWidget() : base()
        {
            _BatteryStatus = GetBatteryStatus();
        }

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(1, 1),
                new Size(2, 2) 
            };
            return sizes;
        }
        
        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);
            PaintStatus(g, Rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle Rect)
        {
            //
        }

        private void PaintStatus(Graphics g, Rectangle Rect)
        {
            int FontSize = 12 * this._Size.Width;
            Font captionFont = new System.Drawing.Font("Segoe UI Light", FontSize, FontStyle.Bold);
            Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            g.DrawString(_BatteryStatus, captionFont, captionBrush,
                Rect.Left + 5, Rect.Top + 5);
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
            string CurrentStatus = GetBatteryStatus();
            if (CurrentStatus != _BatteryStatus)
            {
                _BatteryStatus = CurrentStatus;
                OnWidgetUpdate();
            }
        }

        private string GetBatteryStatus()
        {
            switch (Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength)
            {
                case Microsoft.WindowsMobile.Status.BatteryLevel.VeryHigh: return "100%"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.High: return "80%"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.Medium: return "60%"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.Low: return "40%"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.VeryLow: return "20%"; break;
                default: return "";
            }
        }

    }


}
