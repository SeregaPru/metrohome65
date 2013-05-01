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

    public class WirelessControlPage : FleuxControlPage
    {
        public Canvas Background;

        public Canvas Content;

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


        public WirelessControlPage() : base(false)
        {
            ScreenRoutines.CursorWait();

            try
            {
                theForm.Menu = null;
                theForm.ControlBox = false;
                Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.None;

                var canvas = new Canvas {
                    Size = Size,
                    Location = new Point(0, 0)
                };
                Content = canvas;
                Control.AddElement(Content);

                var canvasBg = new Canvas { Size = Size };
                Background = canvasBg;
                Control.AddElement(Background);

                UpdateStatus();

                var element5 = new TextElement("Wireless") {
                    Style = MetroTheme.PhoneTextPageTitle2Style,
                    Location = new Point(120, 20)
                };
                Control.AddElement(element5);

                var element6 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                    ResPatch + "back-dark.bmp")) {
                    TapHandler = TapOnArrow,
                    Location = new Point(10, 57)
                };
                Control.AddElement(element6);

                if (_phoneCur == "phone")
                {
                    var element = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                        ResPatch + "big.button.png")) {
                        Location = new Point(60, 120),
                        Size = new Size(200, 200)
                    };
                    Control.AddElement(element);
                    var element2 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                        ResPatch + "big.big_" + _phoneCur + ".png")) {
                        TapHandler = TapPhone,
                        Location = new Point(74, 134),
                        Size = new Size(174, 174)
                    };
                    Control.AddElement(element2);
                }
                else
                {
                    var element3 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                        ResPatch + "big.button.png")) {
                        Location = new Point(270, 330),
                        Size = new Size(200, 200)
                    };
                    Control.AddElement(element3);
                    var element4 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                        ResPatch + "big.big_" + _phoneCur + ".png")) {
                        TapHandler = TapAir,
                        Location = new Point(284, 344),
                        Size = new Size(174, 174)
                    };
                    Control.AddElement(element4);
                }

                var element7 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                    ResPatch + "big.button.png")) {
                    Location = new Point(270, 120),
                    Size = new Size(200, 200)
                };
                Control.AddElement(element7);

                var element8 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                    ResPatch + "big.big_" + _wifiCur + ".png")) {
                    TapHandler = TapWifi,
                    Location = new Point(0x11c, 0x86),
                    Size = new Size(0xae, 0xae)
                };
                Control.AddElement(element8);

                var element9 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                    ResPatch + "big.button.png")) {
                    Location = new Point(60, 330),
                    Size = new Size(200, 200)
                };
                Control.AddElement(element9);

                var element10 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(
                    ResPatch + "big.big_" + _blueCur + ".png")) {
                    TapHandler = TapBlue,
                    Location = new Point(0x4a, 0x158),
                    Size = new Size(0xae, 0xae)
                };
                Control.AddElement(element10);

            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        private void UpdateStatus()
        {
            if (SystemState.BluetoothStatePowerOn)
            {
                _blueCur = SystemState.BluetoothStateDiscoverable ? "bluetooth_on_invisible" : "bluetooth_on_visible";
                if (SystemState.BluetoothStateA2DPConnected)
                {
                    _blueCur = "big_bluetooth_ad2p";
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

        public bool TapOnArrow(Point p)
        {
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

