using MetroHome65.Routines.Screen;

namespace MetroHome65.Widgets
{
    using Fleux.Controls;
    using Fleux.Core;
    using Fleux.Styles;
    using Fleux.UIElements;
    using MetroHome65.Routines;
    using Microsoft.Win32;
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class HubPage : FleuxControlPage
    {
        public msysWirContWidget _main;
        private const int PowerName = 1;

        public HubPage() : base(false)
        {
            ScreenRoutines.CursorWait();
            try
            {
                this._main = new msysWirContWidget();
                base.theForm.set_Menu(null);
                base.theForm.set_ControlBox(false);
                base.Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.None;
                Canvas canvas = new Canvas {
                    Size = new Size(base.Size.get_Width(), base.Size.get_Height()),
                    Location = new Point(0, 0)
                };
                this.Content = canvas;
                base.Control.AddElement(this.Content);
                Canvas canvas2 = new Canvas {
                    Size = base.Size
                };
                this.Background = canvas2;
                base.Control.AddElement(this.Background);
                TextElement element5 = new TextElement("Wireless") {
                    Style = MetroTheme.PhoneTextPageTitle2Style,
                    Location = new Point(120, 20)
                };
                base.Control.AddElement(element5);
                TransparentImageElement element6 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource("back_dark.png")) {
                    TapHandler = new Func<Point, bool>(this, (IntPtr) this.TapOnArrow),
                    Location = new Point(10, 0x39)
                };
                base.Control.AddElement(element6);
                if (this._main._Phone_cur == "phone")
                {
                    TransparentImageElement element = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(this._main._res_patch + "big.button.png")) {
                        Location = new Point(60, 120),
                        Size = new Size(200, 200)
                    };
                    base.Control.AddElement(element);
                    TransparentImageElement element2 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(this._main._res_patch + "big.big_" + this._main._Phone_cur + ".png")) {
                        TapHandler = new Func<Point, bool>(this, (IntPtr) this.TapPhone),
                        Location = new Point(0x4a, 0x86),
                        Size = new Size(0xae, 0xae)
                    };
                    base.Control.AddElement(element2);
                }
                else
                {
                    TransparentImageElement element3 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(this._main._res_patch + "big.button.png")) {
                        Location = new Point(270, 330),
                        Size = new Size(200, 200)
                    };
                    base.Control.AddElement(element3);
                    TransparentImageElement element4 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(this._main._res_patch + "big.big_" + this._main._Phone_cur + ".png")) {
                        TapHandler = new Func<Point, bool>(this, (IntPtr) this.TapAir),
                        Location = new Point(0x11c, 0x158),
                        Size = new Size(0xae, 0xae)
                    };
                    base.Control.AddElement(element4);
                }
                TransparentImageElement element7 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(this._main._res_patch + "big.button.png")) {
                    Location = new Point(270, 120),
                    Size = new Size(200, 200)
                };
                base.Control.AddElement(element7);
                TransparentImageElement element8 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(this._main._res_patch + "big.big_" + this._main._Wifi_cur + ".png")) {
                    TapHandler = new Func<Point, bool>(this, (IntPtr) this.TapWifi),
                    Location = new Point(0x11c, 0x86),
                    Size = new Size(0xae, 0xae)
                };
                base.Control.AddElement(element8);
                TransparentImageElement element9 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(this._main._res_patch + "big.button.png")) {
                    Location = new Point(60, 330),
                    Size = new Size(200, 200)
                };
                base.Control.AddElement(element9);
                TransparentImageElement element10 = new TransparentImageElement(ResourceManager.Instance.GetIImageFromEmbeddedResource(this._main._res_patch + "big.big_" + this._main._Blue_cur + ".png")) {
                    TapHandler = new Func<Point, bool>(this, (IntPtr) this.TapBlue),
                    Location = new Point(0x4a, 0x158),
                    Size = new Size(0xae, 0xae)
                };
                base.Control.AddElement(element10);
                try
                {
                    this.ReadSettings();
                }
                catch (Exception)
                {
                }
            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        [DllImport("BthUtil.dll")]
        public static extern int BthGetMode(out RadioMode dwMode);
        [DllImport("BthUtil.dll")]
        public static extern int BthSetMode(RadioMode dwMode);
        private static string FindDriverKey()
        {
            string str = string.Empty;
            foreach (string str2 in Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Power\State", false).GetValueNames())
            {
                if (str2.Contains("{98C5250D-C29A-4985-AE5F-AFE5367E5006}"))
                {
                    return str2;
                }
            }
            return str;
        }

        private void ReadSettings()
        {
        }

        [DllImport("coredll.dll", SetLastError=true)]
        private static extern int SetDevicePower(string pvDevice, int dwDeviceFlags, DevicePowerState deviceState);
        public bool TapAir(Point p)
        {
            if (this._main._Phone_cur == "airplane")
            {
                PhoneRadio radio = new PhoneRadio();
                radio.SwitchOn();
                radio.Dispose();
            }
            Thread.Sleep(0x3e8);
            this.Close();
            return true;
        }

        public bool TapBlue(Point p)
        {
            if (this._main._Blue_cur == "bluetooth_off")
            {
                BthSetMode(RadioMode.Discoverable);
            }
            else
            {
                BthSetMode(RadioMode.Off);
            }
            Thread.Sleep(0x3e8);
            this.Close();
            return true;
        }

        public bool TapOnArrow(Point p)
        {
            this.Close();
            return true;
        }

        public bool TapPhone(Point p)
        {
            if (this._main._Phone_cur == "phone")
            {
                PhoneRadio radio = new PhoneRadio();
                radio.SwitchOff();
                radio.Dispose();
            }
            Thread.Sleep(0x3e8);
            this.Close();
            return true;
        }

        public bool TapWifi(Point p)
        {
            if (this._main._Wifi_cur == "wifi_on")
            {
                SetDevicePower(FindDriverKey(), 1, DevicePowerState.D4);
            }
            else
            {
                SetDevicePower(FindDriverKey(), 1, DevicePowerState.D0);
            }
            Thread.Sleep(0x3e8);
            this.Close();
            return true;
        }

        public Canvas Background { get; set; }

        public Canvas Content { get; set; }

        private enum DevicePowerState
        {
            D0 = 0,
            D1 = 1,
            D2 = 2,
            D3 = 3,
            D4 = 4,
            Unspecified = -1
        }

        public enum RadioMode
        {
            Off,
            Connectable,
            Discoverable
        }
    }
}

