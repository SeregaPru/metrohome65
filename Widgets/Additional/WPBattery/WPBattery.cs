// MicronSys and Dev 2012 year
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using System.Runtime.InteropServices;

namespace MetroHome65.Widgets
{
    [TileInfo("WP Battery")]
    public class WPBattery : ShortcutWidget, IActive
    {
        #region GetSystemPowerStatusEx2

        [DllImport("coredll")]
        static public extern uint GetSystemPowerStatusEx2(ref SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus, uint dwLen, bool fUpdate);

        public enum AcLineStatus : byte
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
            public AcLineStatus AcLineStatus;
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

        private string _sbACpower = "Unknown";
        private string _sbChargeStatus = "Unknown";
        private string _sbLifePercent = "0%";
        private string _sbIcons = "00.png";

        public WPBattery() 
        {
            UpdateBattery();
        }

        protected bool UpdateBattery()
        {
            var status = new SYSTEM_POWER_STATUS_EX2();
            GetSystemPowerStatusEx2(ref status, (uint)Marshal.SizeOf(status), false);
            _sbACpower = status.AcLineStatus.ToString();
            _sbChargeStatus = status.BatteryFlag.ToString();
            _sbLifePercent = status.BatteryLifePercent.ToString() + "%";

            _sbIcons = "00.png";
            var s = status.BatteryLifePercent;
            if (s < 20) _sbIcons = "20.png";
            if ((s > 20) && (s < 41))  _sbIcons = "40.png";
            if ((s > 40) && (s < 61))  _sbIcons = "60.png";
            if ((s > 60) && (s < 81))  _sbIcons = "80.png";
            if ((s > 80) && (s < 101)) _sbIcons = "100.png";

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
            SizeF sizeBox;

            (new AlphaImage(iconPath, this.GetType().Assembly)).PaintBackground(g, 
                new Rectangle(0, 0, rect.Width, rect.Height));
            
            if (_sbChargeStatus == "Charging")
            {
                iconPath = "WPBattery.Images.power.png";
                (new AlphaImage(iconPath, this.GetType().Assembly)).PaintBackground(g, 
                    new Rectangle(0, 0, rect.Width, rect.Height));
            }

            var captionFont = new Font(CaptionFont.FontFamily, CaptionFont.FontSize.ToLogic(), FontStyle.Regular);
            var captionBrush = new SolidBrush(CaptionFont.Foreground);

            // Caption 
            if (this.Caption != "") 
            {
                sizeBox = g.MeasureString(this.Caption, captionFont);
                x = (rect.Width - (int)(sizeBox.Width)) / 2; 
                y = -4;
                g.DrawString(this.Caption, captionFont, captionBrush, x, y);
            }

            // Charging status 
            sizeBox = g.MeasureString(_sbACpower, captionFont);
            x = CaptionLeftOffset;
            y = rect.Height - (int)sizeBox.Height - CaptionBottomOffset;
            g.DrawString(_sbACpower, captionFont, captionBrush, x, y);

            // Percents
            sizeBox = g.MeasureString(_sbLifePercent, captionFont);
            x = rect.Width - (int)(sizeBox.Width) - CaptionLeftOffset; 
            g.DrawString(_sbLifePercent, captionFont, captionBrush, x, y);
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
