using Fleux.Animations;
using Fleux.Templates;
using MetroHome65.Routines.Screen;
using Microsoft.WindowsMobile.Status;

namespace MetroHome65.Widgets
{
    using Fleux.Controls;
    using Fleux.Core;
    using Fleux.Styles;
    using Fleux.UIElements;
    using Microsoft.Win32;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class WirelessControlPage : WindowsPhone7Page
    {
        private enum DevicePowerState
        {
            D0 = 0,
            D4 = 4
        }

        public enum RadioMode
        {
            Off,
            Connectable,
            Discoverable
        }

        private const string ResPatch = "WirelessControl.Images.";
        private string _blueCur = "bluetooth_off";
        private string _phoneCur = "phone";
        private string _wifiCur = "wifi_off";


        public WirelessControlPage()
            : base("WIRELESS CONTROL", "", false)
        {
            ScreenRoutines.CursorWait();

            try
            {
                theForm.Menu = null;
                theForm.ControlBox = false;
                Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.FromRight;

                Content.Location = new Point(0, 80);
                Content.Size = new Size(Size.Width, Size.Height - 80);

                var appBar = new ApplicationBar
                {
                    Size = new Size(Content.Size.Width, 48 + 2 * 10),
                    Location = new Point(0, Content.Size.Height - 48 - 2 * 10)
                };
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource(ResPatch + 
                    ((MetroTheme.PhoneBackgroundBrush == Color.White) ?
                        "back-light.bmp" : "back-dark.bmp"
                    )));
                appBar.ButtonTap += (sender, args) => Close();
                Content.AddElement(appBar.AnimateHorizontalEntrance(false));


                UpdateStatus();

                if (_phoneCur == "phone")
                {
                    CreateButton(_phoneCur, 27, 0, TapPhone);
                }
                else
                {
                    CreateButton(_phoneCur, 254, 227, TapAir);
                }

                CreateButton(_wifiCur, 254, 0, TapWifi);
                CreateButton(_blueCur, 27, 227, TapBlue);
            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        private void CreateButton(string imageName, int x, int y, System.Func<Point, bool> handler)
        {
            var border = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                ResPatch + "big_button.png"))
                {
                    Location = new Point(x, y),
                    Size = new Size(200, 200)
                };
            Content.AddElement(border);

            var icon = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                ResPatch + "big_" + imageName + ".png"))
                {
                    TapHandler = handler,
                    Location = new Point(x+14, y+14),
                    Size = new Size(174, 174)
                };
            Content.AddElement(icon);
        }

        private void UpdateStatus()
        {
            if (SystemState.BluetoothStatePowerOn)
            {
                _blueCur = SystemState.BluetoothStateDiscoverable ? "bluetooth_on_invisible" : "bluetooth_on_visible";
                if (SystemState.BluetoothStateA2DPConnected)
                {
                    _blueCur = "bluetooth_ad2p";
                }
            }
            else
            {
                _blueCur = "bluetooth_off";
            }

            _wifiCur = SystemState.WiFiStatePowerOn ? "wifi_on" : "wifi_off";

            _phoneCur = SystemState.PhoneRadioOff ? "airplane" : "phone";
        }

        public bool TapAir(Point p)
        {
            if (_phoneCur == "airplane")
            {
                var radio = new PhoneRadio();
                radio.SwitchOn();
                radio.Dispose();
            }
            Thread.Sleep(1000);
            Close();
            return true;
        }

        public bool TapBlue(Point p)
        {
            BthSetMode(_blueCur == "bluetooth_off" ? RadioMode.Discoverable : RadioMode.Off);
            Thread.Sleep(1000);
            Close();
            return true;
        }

        public bool TapPhone(Point p)
        {
            if (_phoneCur == "phone")
            {
                var radio = new PhoneRadio();
                radio.SwitchOff();
                radio.Dispose();
            }
            Thread.Sleep(1000);
            Close();
            return true;
        }

        public bool TapWifi(Point p)
        {
            SetDevicePower(FindDriverKey(), 1, 
                _wifiCur == "wifi_on" ? DevicePowerState.D4 : DevicePowerState.D0);
            Thread.Sleep(1000);
            Close();
            return true;
        }

        private static string FindDriverKey()
        {
            foreach (var strState in Registry.LocalMachine.
                OpenSubKey(@"System\CurrentControlSet\Control\Power\State", false).GetValueNames())
            {
                if (strState.Contains("{98C5250D-C29A-4985-AE5F-AFE5367E5006}"))
                {
                    return strState;
                }
            }
            return string.Empty;
        }

        [DllImport("BthUtil.dll")]
        public static extern int BthGetMode(out RadioMode dwMode);

        [DllImport("BthUtil.dll")]
        public static extern int BthSetMode(RadioMode dwMode);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int SetDevicePower(string pvDevice, int dwDeviceFlags, DevicePowerState deviceState);

    }
}

