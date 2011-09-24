using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MetroHome65.Widgets.StatusWidget
{
    public class BluetoothStatus : CustomStatus
    {
        private bool _BluetoothPowerOn = false;
        private bool _BluetoothConnected = false;

        public BluetoothStatus() : base()
        {
            UpdateStatus();
        }

        public override void PaintStatus(Graphics g, Rectangle Rect)
        {
            DrawStatus DrawStatus;
            if (_BluetoothConnected && _BluetoothPowerOn)
                DrawStatus = DrawStatus.dsOn;
            else
            if (! _BluetoothConnected && ! _BluetoothPowerOn)
                DrawStatus = DrawStatus.dsOff;
            else
                DrawStatus = DrawStatus.dsError;

            PaintStatus(g, Rect, DrawStatus, "BT", "");
        }

        /// <summary>
        /// Check if Bluetooth status was changed since last check
        /// </summary>
        /// <returns>True if status was changed</returns>
        public override bool UpdateStatus()
        {
            bool CurrentPowerOn = Microsoft.WindowsMobile.Status.SystemState.BluetoothStatePowerOn;
            bool CurrentConnected = Microsoft.WindowsMobile.Status.SystemState.BluetoothStateA2DPConnected;

            if ((CurrentPowerOn != _BluetoothPowerOn) ||
                (CurrentConnected != _BluetoothConnected))
            {
                _BluetoothPowerOn = CurrentPowerOn;
                _BluetoothConnected = CurrentConnected;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Switch Bluetooth status to another - on <--> off
        /// </summary>
        public override void ChangeStatus()
        {
            int ret;
            if (_BluetoothPowerOn)
            {
                // turn Bluetooth Off
                ret = BthSetMode(RadioMode.Off);
            }
            else
            {
                // turn Bluetooth On
                ret = BthSetMode(RadioMode.Discoverable);
            }
        }


        public enum RadioMode
        {
            Off = 0,
            Connectable = 1,
            Discoverable = 2
        }

        [DllImport("BthUtil.dll")]
        public static extern int BthGetMode(out RadioMode dwMode);

        [DllImport("BthUtil.dll")]
        public static extern int BthSetMode(RadioMode dwMode);

    }

}
