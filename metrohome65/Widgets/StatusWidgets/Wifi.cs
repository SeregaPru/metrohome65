using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MetroHome65.Widgets.StatusWidgets
{
    public class WiFiStatus : CustomStatus
    {
        private bool _wiFiPowerOn;
        private bool _wiFiConnected;

        public WiFiStatus()
        {
            UpdateStatus();
        }

        /// <summary>
        /// Paint curreent WiFi status
        /// </summary>
        public override void PaintStatus(Graphics g, Rectangle rect)
        {
            DrawStatus drawStatus;
            if (_wiFiConnected && _wiFiPowerOn)
                drawStatus = DrawStatus.On;
            else
                if (!_wiFiConnected && !_wiFiPowerOn)
                    drawStatus = DrawStatus.Off;
                else 
                    drawStatus = DrawStatus.Error;

            PaintStatus(g, rect, drawStatus, "wifi", "");
        }

        /// <summary>
        /// Check if WiFi status was changed since last check
        /// </summary>
        /// <returns>True if status was changed</returns>
        public override bool UpdateStatus()
        {
            var currentPowerOn = Microsoft.WindowsMobile.Status.SystemState.WiFiStatePowerOn;
            var currentConnected = Microsoft.WindowsMobile.Status.SystemState.WiFiStateConnected;

            if ((currentPowerOn != _wiFiPowerOn) ||
                (currentConnected != _wiFiConnected))
            {
                _wiFiPowerOn = currentPowerOn;
                _wiFiConnected = currentConnected;
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Switch WiFi status to another - on / off
        /// </summary>
        public override void ChangeStatus()
        {
            if (_wiFiPowerOn)
            {
                // turn WiFi Off
                string driver = FindDriverKey(); 
                SetDevicePower(driver, PowerName, DevicePowerState.D4);
            }
            else
            {
                // turn WiFi On
                string driver = FindDriverKey(); 
                SetDevicePower(driver, PowerName, DevicePowerState.D0);
            }
        }

        [DllImport("coredll.dll", SetLastError = true)] 
        private static extern int SetDevicePower(string pvDevice, int dwDeviceFlags, DevicePowerState deviceState); 

        private enum DevicePowerState
        { 
            Unspecified = -1, 
            D0 = 0, // Full On: full power, full functionality 
            D1, // Low Power On: fully functional at low power/performance 
            D2, // Standby: partially powered with automatic wake 
            D3, // Sleep: partially powered with device initiated wake 
            D4, // Off: unpowered 
        }

        private const int PowerName = 0x00000001;

        // simply a function that returns the whole registry key name of the key containing the NDIS MINIPORT class GUID defined in the SDK’s pm.h:
        private static string FindDriverKey() 
        { 
            var ret = string.Empty; 
            const string wiFiDriverClass = "{98C5250D-C29A-4985-AE5F-AFE5367E5006}"; 

            foreach (var tmp in Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Power\\State", false).GetValueNames()) 
            { 
                if (tmp.Contains(wiFiDriverClass)) 
                { 
                    ret = tmp; 
                    break; 
                } 
            } 

            return ret; 
        }
    }
    
}
