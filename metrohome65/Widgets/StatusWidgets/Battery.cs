using System;
using System.Drawing;

namespace MetroHome65.Widgets.StatusWidget
{
    public class BatteryStatus : CustomStatus
    {
        private string _BatteryStatus = "";
        private Microsoft.WindowsMobile.Status.BatteryState _PowerState;

        public BatteryStatus() : base()
        {
            UpdateStatus();
        }

        public override void PaintStatus(Graphics g, Rectangle Rect)
        {
            DrawStatus DrawStatus = (_PowerState == Microsoft.WindowsMobile.Status.BatteryState.Critical) ? DrawStatus.dsError : DrawStatus.dsOn; 
            PaintStatus(g, Rect, DrawStatus, "battery_" + _BatteryStatus, GetBatteryPercent() + "%");
        }

        public override bool UpdateStatus()
        {
            string CurrentStatus = GetBatteryStatus();
            if (CurrentStatus != _BatteryStatus)
            {
                _BatteryStatus = CurrentStatus;
                return true;
            }

            return false;
        }

        private string GetBatteryPercent()
        {
            String Status = "";            
            switch (Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength)
            {
                case Microsoft.WindowsMobile.Status.BatteryLevel.VeryHigh: Status = "100"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.High: Status = "80"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.Medium: Status = "60"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.Low: Status = "40"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.VeryLow: Status = "20"; break;
                default: Status = ""; break;
            }
            return Status;
        }

        private string GetBatteryStatus()
        {
            String Status = GetBatteryPercent();
            _PowerState = Microsoft.WindowsMobile.Status.SystemState.PowerBatteryState;

            if (_PowerState == Microsoft.WindowsMobile.Status.BatteryState.Charging)
                Status += "_charge";

            return Status;
        }

    }
}
