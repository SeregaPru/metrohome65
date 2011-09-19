using System;
using System.Drawing;

namespace MetroHome65.Widgets.StatusWidget
{
    public class BatteryStatus
    {
        private string _BatteryStatus = "";

        public BatteryStatus()
        {
            _BatteryStatus = GetBatteryStatus();
        }

        public void PaintStatus(Graphics g, Rectangle Rect)
        {
            int FontSize = 12;
            Font captionFont = new System.Drawing.Font("Segoe UI Light", FontSize, FontStyle.Bold);
            Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            g.DrawString(_BatteryStatus, captionFont, captionBrush,
                Rect.Left + 5, Rect.Top + 5);
        }

        public bool UpdateStatus()
        {
            string CurrentStatus = GetBatteryStatus();
            if (CurrentStatus != _BatteryStatus)
            {
                _BatteryStatus = CurrentStatus;
                return true;
            }

            return false;
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
