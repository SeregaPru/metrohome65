using System.Drawing;
//using System.Runtime.InteropServices;

namespace MetroHome65.Widgets.StatusWidgets
{
    public class BatteryStatus : CustomStatus
    {
        /*
        [DllImport("coredll")]
        static public extern uint GetSystemPowerStatusEx2(SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus, int dwLen, bool fUpdate);

        public class SYSTEM_POWER_STATUS_EX2
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public uint BatteryLifeTime;
            public uint BatteryFullLifeTime;
            public byte Reserved2;
            public byte BackupBatteryFlag;
            public byte BackupBatteryLifePercent;
            public byte Reserved3;
            public uint BackupBatteryLifeTime;
            public uint BackupBatteryFullLifeTime;
            public uint BatteryVoltage;
            public uint BatteryCurrent;
            public uint BatteryAverageCurrent;
            public uint BatteryAverageInterval;
            public uint BatterymAHourConsumed;
            public uint BatteryTemperature;
            public uint BackupBatteryVoltage;
            public byte BatteryChemistry;
        }
        */

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

        /*
        private string GetBatteryPercentExt()
        {
            SYSTEM_POWER_STATUS_EX2 Status = new SYSTEM_POWER_STATUS_EX2();
            GetSystemPowerStatusEx2(Status, Marshal.SizeOf(Status), false);
            return (Status.BatteryLifePercent.ToString());
        }
        */
    }
}
