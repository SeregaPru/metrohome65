using TinyMessenger;

namespace MetroHome65.Widgets
{
    using Fleux.Controls;
    using Fleux.UIElements;
    using MetroHome65.Interfaces;
    using MetroHome65.Interfaces.Events;
    using MetroHome65.Routines;
    using Microsoft.WindowsMobile.Status;
    using System.Collections.Generic;
    using System.Drawing;
    using TinyIoC;

    [TileInfo("Wireless Control")]
    public class WirelessControlWidget : ShortcutWidget, IActive
    {
        private const string ResPatch = "WirelessControl.Images.";
        private string _blueCur = "bluetooth_off";
        private string _phoneCur = "phone";
        private string _wifiCur = "wifi_off";

        private string _oldBlueCur;
        private string _oldPhoneCur;
        private string _oldWifiCur;

        private ThreadTimer _updateTimer;


        public WirelessControlWidget()
        {
            UpdateStatus();
        }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);

            foreach (var element in controls)
            {
                if (element.Name.Contains("Icon"))
                {
                    controls.Remove(element);
                    break;
                }
            }

            foreach (var element in controls)
            {
                if (element.Name.Contains("Caption"))
                {
                    controls.Remove(element);
                    break;
                }
            }

            return controls;
        }

        protected override bool GetDoExitAnimation()
        {
            return true;
        }

        protected override Size[] GetSizes()
        {
            return new[] { new Size(2, 2) };
        }

        public override bool OnClick(Point location)
        {
            var page = new WirelessControlPage();
            TinyIoCContainer.Current.Resolve<ITinyMessengerHub>().Publish<ShowPageMessage>(new ShowPageMessage(page));
            return true;
        }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);

            var pos1 = (rect.Width - (2 * 87)) / 3;
            var pos2 = pos1 * 2 + 87;

            // TODO заменить нормальными картинками без масштаба
            new AlphaImage(ResPatch + "small_" + _phoneCur + ".png", GetType().Assembly).
                PaintIcon(g, _phoneCur == "phone" ?
                    new Rectangle(pos1, pos1, 87, 87) :
                    new Rectangle(pos2, pos2, 87, 87)
                );

            new AlphaImage(ResPatch + "small_" + _wifiCur + ".png", GetType().Assembly).
                PaintIcon(g, pos2, pos1);
            new AlphaImage(ResPatch + "small_" + _blueCur + ".png", GetType().Assembly).
                PaintIcon(g, pos1, pos2);
        }

        protected override void PaintCaption(Graphics g, Rectangle rect)
        {
        }

        protected override void PaintIcon(Graphics g, Rectangle rect)
        {
        }

        private bool UpdateStatus()
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

            if ((_oldBlueCur != _blueCur) || (_oldPhoneCur != _phoneCur) || (_oldWifiCur != _wifiCur))
            {
                _oldBlueCur = _blueCur;
                _oldWifiCur = _wifiCur;
                _oldPhoneCur = _phoneCur;

                return true;
            }
            return false;
        }

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(2000, () =>
                        {
                            if (UpdateStatus())
                                ForceUpdate();
                        });
                }
                else
                {
                    if (_updateTimer != null)
                        _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }
    }
}

