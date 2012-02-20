using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MetroHome65.Widgets.StatusWidget
{
    public class WiFiStatus : CustomStatus
    {
        private bool _WiFiPowerOn = false;
        private bool _WiFiConnected = false;

        public WiFiStatus() : base()
        {
            UpdateStatus();
        }

        /// <summary>
        /// Paint curreent WiFi status
        /// </summary>
        public override void PaintStatus(Graphics g, Rectangle rect)
        {
            DrawStatus DrawStatus;
            if (_WiFiConnected && _WiFiPowerOn)
                DrawStatus = DrawStatus.dsOn;
            else
                if (!_WiFiConnected && !_WiFiPowerOn)
                    DrawStatus = DrawStatus.dsOff;
                else 
                    DrawStatus = DrawStatus.dsError;

            PaintStatus(g, rect, DrawStatus, "wifi", "");
        }

        /// <summary>
        /// Check if WiFi status was changed since last check
        /// </summary>
        /// <returns>True if status was changed</returns>
        public override bool UpdateStatus()
        {
            bool CurrentPowerOn = Microsoft.WindowsMobile.Status.SystemState.WiFiStatePowerOn;
            bool CurrentConnected = Microsoft.WindowsMobile.Status.SystemState.WiFiStateConnected;

            if ((CurrentPowerOn != _WiFiPowerOn) ||
                (CurrentConnected != _WiFiConnected))
            {
                _WiFiPowerOn = CurrentPowerOn;
                _WiFiConnected = CurrentConnected;
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Switch WiFi status to another - on <--> off
        /// </summary>
        public override void ChangeStatus()
        {
            if (_WiFiPowerOn)
            {
                // turn WiFi Off
                string driver = FindDriverKey(); 
                SetDevicePower(driver, POWER_NAME, DevicePowerState.D4);
            }
            else
            {
                // turn WiFi On
                string driver = FindDriverKey(); 
                SetDevicePower(driver, POWER_NAME, DevicePowerState.D0);
            }
        }

        [DllImport("coredll.dll", SetLastError = true)] 
        private static extern int SetDevicePower(string pvDevice, int dwDeviceFlags, DevicePowerState DeviceState); 

        private enum DevicePowerState : int 
        { 
            Unspecified = -1, 
            D0 = 0, // Full On: full power, full functionality 
            D1, // Low Power On: fully functional at low power/performance 
            D2, // Standby: partially powered with automatic wake 
            D3, // Sleep: partially powered with device initiated wake 
            D4, // Off: unpowered 
        }

        private const int POWER_NAME = 0x00000001;

        // simply a function that returns the whole registry key name of the key containing the NDIS MINIPORT class GUID defined in the SDK’s pm.h:
        private static string FindDriverKey() 
        { 
            string ret = string.Empty; 
            string WiFiDriverClass= "{98C5250D-C29A-4985-AE5F-AFE5367E5006}"; 

            foreach (string tmp in Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Power\\State", false).GetValueNames()) 
            { 
                if (tmp.Contains(WiFiDriverClass)) 
                { 
                    ret = tmp; 
                    break; 
                } 
            } 

            return ret; 
        }
    }
    
}
