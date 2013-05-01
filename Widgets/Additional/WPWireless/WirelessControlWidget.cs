namespace MetroHome65.Widgets
{
    using Fleux.Controls;
    using Fleux.UIElements;
    using MetroHome65.Interfaces;
    using MetroHome65.Interfaces.Events;
    using MetroHome65.Routines;
    using Microsoft.WindowsMobile.Status;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using TinyIoC;

    [TileInfo("WirContWidget")]
    public class msysWirContWidget : ShortcutWidget, IActive
    {
        public string _Blue_cur = "bluetooth_off";
        public string _Blue_old = "";
        private readonly Brush _brush = new SolidBrush(MetroTheme.TileTextStyle.Foreground);
        private readonly Font _fnt = new Font(MetroTheme.PhoneFontFamilySemiBold, (float) 10.ToLogic(), 0);
        public string _Phone_cur = "phone";
        public string _Phone_old = "";
        public string _res_patch = "msysWirContWidget.Images.";
        public ThreadTimer _updateTimer;
        public string _Wifi_cur = "wifi_off";
        public string _Wifi_old = "";

        public msysWirContWidget()
        {
            this.UpdateStatus();
            this.ForceUpdate();
        }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            ICollection<UIElement> is2 = base.EditControls(settingsPage);
            BindingManager manager = new BindingManager();
            foreach (UIElement element in is2)
            {
                if (element.Name.Contains("Icon"))
                {
                    is2.Remove(element);
                    break;
                }
            }
            foreach (UIElement element in is2)
            {
                if (element.Name.Contains("Caption"))
                {
                    is2.Remove(element);
                    return is2;
                }
            }
            return is2;
        }

        protected override bool GetDoExitAnimation()
        {
            return true;
        }

        protected override Size[] GetSizes()
        {
            return new Size[] { new Size(2, 2) };
        }

        public override bool OnClick(Point location)
        {
            HubPage page = new HubPage();
            TinyIoCContainer.get_Current().Resolve<ITinyMessengerHub>().Publish<ShowPageMessage>(new ShowPageMessage(page));
            return true;
        }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            if (this._Phone_cur == "phone")
            {
                new AlphaImage(this._res_patch + "big.big_" + this._Phone_cur + ".png", base.GetType().Assembly).PaintBackground(g, new Rectangle(0, 0, 0x57, 0x57));
            }
            else
            {
                new AlphaImage(this._res_patch + "big.big_" + this._Phone_cur + ".png", base.GetType().Assembly).PaintBackground(g, new Rectangle(0x57, 0x57, 0x57, 0x57));
            }
            new AlphaImage(this._res_patch + "big.big_" + this._Wifi_cur + ".png", base.GetType().Assembly).PaintBackground(g, new Rectangle(0x57, 0, 0x57, 0x57));
            new AlphaImage(this._res_patch + "big.big_" + this._Blue_cur + ".png", base.GetType().Assembly).PaintBackground(g, new Rectangle(0, 0x57, 0x57, 0x57));
        }

        protected override void PaintCaption(Graphics g, Rectangle rect)
        {
        }

        protected override void PaintIcon(Graphics g, Rectangle rect)
        {
        }

        public bool UpdateStatus()
        {
            bool flag = SystemState.get_BluetoothStatePowerOn();
            bool flag2 = SystemState.get_BluetoothStateA2DPConnected();
            bool flag3 = SystemState.get_BluetoothStateDiscoverable();
            if (flag)
            {
                if (flag3)
                {
                    this._Blue_cur = "bluetooth_on_invisible";
                }
                else
                {
                    this._Blue_cur = "bluetooth_on_visible";
                }
                if (flag2)
                {
                    this._Blue_cur = "big_bluetooth_ad2p";
                }
            }
            else
            {
                this._Blue_cur = "bluetooth_off";
            }
            if (SystemState.get_WiFiStatePowerOn())
            {
                this._Wifi_cur = "wifi_on";
            }
            else
            {
                this._Wifi_cur = "wifi_off";
            }
            if (SystemState.get_PhoneRadioOff())
            {
                this._Phone_cur = "airplane";
            }
            else
            {
                this._Phone_cur = "phone";
            }
            return true;
        }

        public bool Active
        {
            get
            {
                return (this._updateTimer != null);
            }
            set
            {
                ThreadTimerProc proc = null;
                if (value)
                {
                    if (this._updateTimer == null)
                    {
                        if (proc == null)
                        {
                            proc = new ThreadTimerProc(this, (IntPtr) this.<set_Active>b__0);
                        }
                        this._updateTimer = new ThreadTimer(0x7d0, proc);
                    }
                }
                else
                {
                    if (this._updateTimer != null)
                    {
                        this._updateTimer.Stop();
                    }
                    this._updateTimer = null;
                }
            }
        }
    }
}

