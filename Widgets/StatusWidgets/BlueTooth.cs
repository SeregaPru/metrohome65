using System.Drawing;
using System.Runtime.InteropServices;

namespace MetroHome65.Widgets.StatusWidgets
{
    public class BluetoothStatus : CustomStatus
    {
        private bool _bluetoothPowerOn;
        private bool _bluetoothConnected;

        public BluetoothStatus()
        {
            UpdateStatus();
        }

        public override void PaintStatus(Graphics g, Rectangle rect)
        {
            DrawStatus drawStatus;
            if (_bluetoothConnected && _bluetoothPowerOn)
                drawStatus = DrawStatus.On;
            else
            if (! _bluetoothConnected && ! _bluetoothPowerOn)
                drawStatus = DrawStatus.Off;
            else
                drawStatus = DrawStatus.Error;

            PaintStatus(g, rect, drawStatus, "bluetooth", "");
        }

        /// <summary>
        /// Check if Bluetooth status was changed since last check
        /// </summary>
        /// <returns>True if status was changed</returns>
        public override bool UpdateStatus()
        {
            var currentPowerOn = Microsoft.WindowsMobile.Status.SystemState.BluetoothStatePowerOn;
            var currentConnected = Microsoft.WindowsMobile.Status.SystemState.BluetoothStateA2DPConnected;

            if ((currentPowerOn != _bluetoothPowerOn) ||
                (currentConnected != _bluetoothConnected))
            {
                _bluetoothPowerOn = currentPowerOn;
                _bluetoothConnected = currentConnected;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Switch Bluetooth status to another - on / off
        /// </summary>
        public override void ChangeStatus()
        {
            BthSetMode(_bluetoothPowerOn ? RadioMode.Off : RadioMode.Discoverable);
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
