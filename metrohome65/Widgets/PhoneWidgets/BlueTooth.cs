using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MetroHome65.Widgets.StatusWidget
{
    public class BluetoothStatus
    {
        private bool _BluetoothPowerOn = false;
        private bool _BluetoothConnected = false;

        public BluetoothStatus() : base()
        {
            UpdateStatus();
        }

        public void PaintStatus(Graphics g, Rectangle Rect)
        {
            SolidBrush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

            Font captionFont = new System.Drawing.Font("Segoe UI Light", 11, FontStyle.Bold);
            captionBrush.Color = (_BluetoothPowerOn) ? System.Drawing.Color.LightGreen : System.Drawing.Color.DarkGray;
            g.DrawString("BT", captionFont, captionBrush, Rect.Left + 5, Rect.Top + 10);

            captionFont = new System.Drawing.Font("Segoe UI Light", 9, FontStyle.Bold);
            captionBrush.Color = (_BluetoothConnected) ? System.Drawing.Color.LightGreen : System.Drawing.Color.DarkGray;
            g.DrawString("A2DP", captionFont, captionBrush, Rect.Left + 5, Rect.Top + 40);
        }

        /// <summary>
        /// Check if Bluetooth status was changed since last check
        /// </summary>
        /// <returns>True if status was changed</returns>
        public bool UpdateStatus()
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
        public void ChangeStatus()
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
