// MicronSys and Dev 2012 year
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using TinyIoC;
using TinyMessenger;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System;
using Metrohome65.Settings.Controls;
using System.Windows.Forms;

namespace MetroHome65.Widgets
{
    [TileInfo("WP Battery")]
    public class WPBattery : ShortcutWidget, IActive
    {
        #region GetSystemPowerStatusEx2

        [DllImport("coredll")]
        static public extern uint GetSystemPowerStatusEx2(ref SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus, uint dwLen, bool fUpdate);

        public enum ACLineStatus : byte
        {
            Offline = 0x00,
            Online = 0x01,
            BackupPower = 0x02,
            Unknown = 0xFF
        }


        public enum BatteryFlag : byte
        {
            High = 0x01,
            Low = 0x02,
            Critical = 0x04,
            Charging = 0x08,
            NoBattery = 0x80,
            Unknown = 0xFF
        }
        public enum BatteryChemistry : byte
        {
            Alkaline = 0x01,
            NiCd = 0x02,
            NiMh = 0x03,
            LiON = 0x04,
            LiPoly = 0x05,
            ZincAir = 0x06,
            Unknown = 0xFF
        }

        public struct SYSTEM_POWER_STATUS_EX2
        {
            public ACLineStatus ACLineStatus;
            public BatteryFlag BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
            public byte Reserved2;
            public BatteryFlag BackupBatteryFlag;
            public byte BackupBatteryLifePercent;
            public byte Reserved3;
            public int BackupBatteryLifeTime;
            public int BackupBatteryFullLifeTime;
            public int BatteryVoltage;
            public int BatteryCurrent;
            public int BatteryAverageCurrent;
            public int BatteryAverageInterval;
            public int BatterymAHourConsumed;
            public int BatteryTemperature;
            public int BackupBatteryVoltage;
            public BatteryChemistry BatteryChemistry;
        }


        #endregion

        private ThreadTimer _updateTimer;
        private readonly Brush _brush;
        private readonly Font _fnt;
        private readonly Font _fnt1;

        public string _sbACpower = "Unknown";
        public string _sbCharge_status = "Unknown";
        public string _sbLife_percent = "0%";
        public string _sbVoltage = "0.00V";
        public string _sbAmperagee  = "0 mA";
        public string _sbTemperature = "0.0 °C";
        public string _sbType_battery = "Unknown";

        public string _sbCaption = "";

        public string _sbIcons = "00.png";

        private int _startX = 87;
        private int _startY = 64;

        private Bitmap image;

        public WPBattery() 
        {
            image = new Bitmap(this.Size.Width, this.Size.Height);
            _brush = new SolidBrush(MetroTheme.TileTextStyle.Foreground);
            _fnt = new Font(MetroTheme.PhoneFontFamilySemiBold, 8.ToLogic(), FontStyle.Regular);
            _fnt1 = new Font(MetroTheme.PhoneFontFamilySemiBold, 10.ToLogic(), FontStyle.Regular);
            UpdateBattery();
            ForceUpdate();
            
            
        }

        protected bool UpdateBattery()
        {

            SYSTEM_POWER_STATUS_EX2 Status = new SYSTEM_POWER_STATUS_EX2();
            uint retBytesCount = GetSystemPowerStatusEx2(ref Status, (uint)Marshal.SizeOf(Status), false);
            _sbACpower = Status.ACLineStatus.ToString();
            _sbCharge_status = Status.BatteryFlag.ToString();
            _sbLife_percent = Status.BatteryLifePercent.ToString() + "%";
            _sbVoltage = ((float)(Status.BatteryVoltage) / 1000).ToString("0.##") + "V";
            _sbAmperagee = Status.BatteryCurrent.ToString() + "mA";
            _sbTemperature = ((float)(Status.BatteryTemperature) * 0.1).ToString("0.##") + " °C";
            _sbType_battery =  Status.BatteryChemistry.ToString();


            _sbIcons = "00.png";
            var _s = Status.BatteryLifePercent;
            if (_s < 20) _sbIcons = "20.png";
            if ((_s > 20) && (_s < 41))  _sbIcons = "40.png";
            if ((_s > 40) && (_s < 61))  _sbIcons = "60.png";
            if ((_s > 60) && (_s < 81))  _sbIcons = "80.png";
            if ((_s > 80) && (_s < 101)) _sbIcons = "100.png";
            _sbCaption = this.Caption; 

            return true;
        }
        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(2, 2)
            };
        }


        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            var iconPath = "WPBattery.Images." + _sbIcons;
            var x = 0;
            var y = 0;
            var _size_box = g.MeasureString(_sbLife_percent, _fnt);

            (new AlphaImage(iconPath, this.GetType().Assembly)).PaintBackground(g, new Rectangle(0, 0, rect.Width,rect.Height));
            
            if (_sbCharge_status == "Charging")
            {
                iconPath = "WPBattery.Images.power.png";
                (new AlphaImage(iconPath, this.GetType().Assembly)).PaintBackground(g, new Rectangle(0, 0, rect.Width, rect.Height));
            }

            /************************** Название ****************************/
            if (this.Caption != "") // Top <> Center
            {
                _sbCaption = this.Caption; 
                _size_box = g.MeasureString(this.Caption, _fnt1);
                x = (rect.Width - (int)(_size_box.Width)) / 2; y = - 7;
                g.DrawString(this.Caption, _fnt1, _brush, x, y);
            }
            /****************** Cостояния зарядки ****************************/
            _size_box = g.MeasureString(_sbACpower, _fnt);
            x = 5 ; y = rect.Height - (int)_size_box.Height - 0;
            g.DrawString(_sbACpower, _fnt, _brush, x, y);

            /****************** Проценты ****************************/
            _size_box = g.MeasureString(_sbLife_percent, _fnt);
            x = rect.Width - (int)(_size_box.Width) -  5; y = rect.Height - (int)_size_box.Height - 0;
            g.DrawString(_sbLife_percent, _fnt, _brush, x, y);
        }

        
        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(5000, () =>
                        {
                            UpdateBattery();
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

        protected override void PaintCaption(Graphics g, Rectangle rect)
        { }


        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            //var bindingManager = new BindingManager();

            
            foreach (var control in controls)
                if (control.Name.Contains("Icon"))
                {
                    controls.Remove(control);
                    break;
                }
            return controls;
        }

    }

}
