using System.Drawing;

namespace MetroHome65.Widgets.StatusWidgets
{
    public class BatteryStatus : CustomStatus
    {
        private string _batteryStatus = "";
        private Microsoft.WindowsMobile.Status.BatteryState _powerState;

        public BatteryStatus() : base()
        {
            UpdateStatus();
        }

        public override void PaintStatus(Graphics g, Rectangle rect)
        {
            DrawStatus drawStatus = (_powerState == Microsoft.WindowsMobile.Status.BatteryState.Critical) ? DrawStatus.Error : DrawStatus.On; 
            PaintStatus(g, rect, drawStatus, "battery_" + _batteryStatus, GetBatteryPercent() + "%");
        }

        public override bool UpdateStatus()
        {
            var currentStatus = GetBatteryStatus();
            if (currentStatus != _batteryStatus)
            {
                _batteryStatus = currentStatus;
                return true;
            }

            return false;
        }

        private string GetBatteryPercent()
        {
            var status = "";            
            switch (Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength)
            {
                case Microsoft.WindowsMobile.Status.BatteryLevel.VeryHigh: status = "100"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.High: status = "80"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.Medium: status = "60"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.Low: status = "40"; break;
                case Microsoft.WindowsMobile.Status.BatteryLevel.VeryLow: status = "20"; break;
                default: status = ""; break;
            }
            return status;
        }

        private string GetBatteryStatus()
        {
            var status = GetBatteryPercent();
            _powerState = Microsoft.WindowsMobile.Status.SystemState.PowerBatteryState;

            if (_powerState == Microsoft.WindowsMobile.Status.BatteryState.Charging)
                status += "_charge";

            return status;
        }

    }
}
