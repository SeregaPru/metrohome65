using System;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;
using Microsoft.Win32;

namespace MetroHome65.Widgets
{
    [TileInfo("Digital clock")]
    public class DigitalClockWidget : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;

        private Boolean _showPoints = true;
        private Boolean _is24Hour = true;
        private Boolean _showAlarm = false;
        private string _dateFormat = _dateFormats[0];

        private int _paddingRight;

        private TextStyle _timeFont = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, 36, Color.White);
        private TextStyle _dateFont = new TextStyle(MetroTheme.PhoneFontFamilySemiLight, 12, Color.White);
        private TextStyle _alarmFont = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, 8, Color.White);


        public DigitalClockWidget() 
        {
            // fill date examples using current date and formats
            var currentDate = DateTime.Now;
            _dateSamples = new List<object>();
            foreach (var format in _dateFormats) 
            {
                _dateSamples.Add(currentDate.ToString(format));
            }

            ApplySizeSettings();
        }

        private void ApplySizeSettings()
        {
            if (this.GridSize.Width == 4)
            {
                _paddingRight = 20;
            }
            else
            {
                _paddingRight = 8;
            }
        }

        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(2, 1), 
                new Size(2, 2), 
                new Size(4, 2), 
            };
        }

        protected override void GridSizeChanged()
        {
            ApplySizeSettings();
        }


        /// <summary>
        /// Digital clock style - 12/24 hour
        /// </summary>
        [TileParameter]
        public Boolean Is24Hour
        {
            get { return _is24Hour; }
            set
            {
                if (_is24Hour == value) return;
                _is24Hour = value;
                NotifyPropertyChanged("Is24Hour");
            }
        }

        /// <summary>
        /// Show alarm clock
        /// </summary>
        [TileParameter]
        public Boolean ShowAlarm
        {
            get { return _showAlarm; }
            set
            {
                if (_showAlarm == value) return;
                _showAlarm = value;
                NotifyPropertyChanged("ShowAlarm");
            }
        }

        /// <summary>
        /// Format for date
        /// </summary>
        [TileParameter]
        public String DateFormat
        {
            get { return _dateFormat; }
            set
            {
                if (_dateFormat != value)
                {
                    _dateFormat = value;
                    NotifyPropertyChanged("DateFormat");
                }
            }
        }
        
        public int DateSampleIndex
        {
            get 
            { 
                for (var i = 0; i < _dateFormats.Count; i++)
                    if (_dateFormats[i] == DateFormat) return i;
                return 0;
            }
            set
            {
                if ((value >= 0) && (value < _dateFormats.Count))
                    DateFormat = _dateFormats[value];
            }
        }

        [TileParameter]
        public TextStyle TimeFont
        {
            get { return _timeFont; }
            set
            {
                if (_timeFont != value)
                {
                    _timeFont = value;
                    NotifyPropertyChanged("TimeFont");
                }
            }
        }

        [TileParameter]
        public TextStyle DateFont
        {
            get { return _dateFont; }
            set
            {
                if (_dateFont != value)
                {
                    _dateFont = value;
                    NotifyPropertyChanged("DateFont");
                }
            }
        }

        [TileParameter]
        public TextStyle AlarmFont
        {
            get { return _alarmFont; }
            set
            {
                if (_alarmFont != value)
                {
                    _alarmFont = value;
                    NotifyPropertyChanged("AlarmFont");
                }
            }
        }

        /// <summary>
        /// collection of formatted date with different foramts
        /// for display in date format selection form
        /// </summary>
        private readonly List<object> _dateSamples;

        private static readonly List<String> _dateFormats = new List<String>()
                                               {
                                                    "dddd, MMMM d", // monday, July 14
                                                    "dddd, d MMMM", // monday, 14 July
                                                    "MMMM d", // July 14
                                                    "d MMMM", // 14 July 
                                                    "MMMM d, dddd", // July 14, monday
                                                    "d MMMM, dddd", // 14 July, monday
                                                    "ddd, MMM d", // mon, Jul 14
                                                    "ddd, d MMM", // mon, 14 Jul
                                               };

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);

            int timeOffsetY = 0;
            // show alarm clock icon and time
            if (_showAlarm)
            {
                var key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Clock\0", false);
                if (key != null)
                {
                    var alarmFlagReg = (byte[])key.GetValue("AlarmFlags", new byte[] { 0 });
                    if (alarmFlagReg[0] != 1)
                    {
                        new AlphaImage("ClockWidgets.Images.AlarmOff.png", base.GetType().Assembly).PaintIcon(g,
                                        rect.Left + CaptionLeftOffset, rect.Top + CaptionLeftOffset);
                        timeOffsetY += 24;
                    }
                    else
                    {
                        var alarmTimeReg = (byte[])key.GetValue("AlarmTime", new byte[] { 0 });

                        var alarmTime = (alarmTimeReg[1] * 0x100) + alarmTimeReg[0];
                        var mins = alarmTime / 60;
                        var hours = alarmTime - (mins * 60);
                        var sAlarm = ((mins < 10)  ? "0" : "") + mins.ToString() + ":" +
                                     ((hours < 10) ? "0" : "") + hours.ToString();

                        var fntAlarm = new Font(_alarmFont.FontFamily, _alarmFont.FontSize.ToLogic(), FontStyle.Regular);
                        var brhAlarm = new SolidBrush(_alarmFont.Foreground);

                        var alarmBox = g.MeasureString(sAlarm, fntAlarm);
                        g.DrawString(sAlarm, fntAlarm, brhAlarm,
                                     rect.Left + CaptionLeftOffset + 24 + CaptionLeftOffset / 2, 
                                     rect.Top + CaptionBottomOffset);

                        new AlphaImage("ClockWidgets.Images.AlarmOn.png", base.GetType().Assembly).PaintIcon(g,
                                        rect.Left + CaptionLeftOffset,
                                        rect.Top + CaptionBottomOffset + (int)(alarmBox.Height / 2) - 8.ToLogic());

                        timeOffsetY += (int)alarmBox.Height;
                    }
                    key.Close();
                }
            }

            var fntTime = new Font(_timeFont.FontFamily, _timeFont.FontSize.ToLogic(), FontStyle.Regular);
            var fntDate = new Font(_dateFont.FontFamily, _dateFont.FontSize.ToLogic(), FontStyle.Regular);

            var sTimeHour = (Is24Hour) ? DateTime.Now.ToString("HH") : DateTime.Now.ToString("hh");
            var sTimeMins = DateTime.Now.ToString("mm");
            var sDate = DateTime.Now.ToString(DateFormat);

            var dots = ":";
            var timeBox = g.MeasureString("99", fntTime);
            var dotBox = g.MeasureString(dots, fntTime);
            var dateBox = (this.GridSize.Height == 2)
                              ? g.MeasureString(sDate, fntDate)
                              : new SizeF(0, 0);

            var brhTime = new SolidBrush(_timeFont.Foreground);
            var timePosY = rect.Top + timeOffsetY/2 + (rect.Height - timeBox.Height - dateBox.Height)/2;
            g.DrawString(sTimeMins, fntTime, brhTime,
                         rect.Right - timeBox.Width - _paddingRight, timePosY);
            if (_showPoints)
                g.DrawString(dots, fntTime, brhTime,
                             rect.Right - timeBox.Width - _paddingRight - dotBox.Width,
                             timePosY - dotBox.Height / 10);
            g.DrawString(sTimeHour, fntTime, brhTime,
                         rect.Right - timeBox.Width - _paddingRight - dotBox.Width - timeBox.Width, timePosY);

            if (this.GridSize.Height == 2)
            {
                var brhDate = new SolidBrush(_dateFont.Foreground);
                g.DrawString(sDate, fntDate, brhDate,
                             rect.Right - dateBox.Width - _paddingRight, 
                             rect.Bottom - dateBox.Height - 4.ToLogic());
            }
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
                                             _showPoints = !_showPoints;
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

        // overriding paint icon method - don't paint icon
        protected override void PaintIcon(Graphics g, Rectangle rect)
        { }

        // overriding paint caption method - don't paint caption
        protected override void PaintCaption(Graphics g, Rectangle rect)
        { }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            var is24HourControl = new FlagSettingsControl
                                  {
                                      Caption = "24-Hours".Localize(), 
                                  };
            controls.Add(is24HourControl);
            bindingManager.Bind(this, "Is24Hour", is24HourControl, "Value", true);

            var showAlarmControl = new FlagSettingsControl
            {
                Caption = "Show alarm".Localize(),
            };
            controls.Add(showAlarmControl);
            bindingManager.Bind(this, "ShowAlarm", showAlarmControl, "Value", true);

            var formatControl = new SelectSettingsControl
                                    {
                                        Caption = "Date format".Localize(),
                                        Items = _dateSamples,
                                    };
            controls.Add(formatControl);
            bindingManager.Bind(this, "DateSampleIndex", formatControl, "SelectedIndex", true);


            var timeFontControl = new FontSettingsControl
                                          {
                                              Caption = "Time Font".Localize(), 
                                          };
            controls.Add(timeFontControl);
            bindingManager.Bind(this, "TimeFont", timeFontControl, "Value", true);

            var dateFontControl = new FontSettingsControl
                                          {
                                              Caption = "Date Font".Localize(), 
                                          };
            controls.Add(dateFontControl);
            bindingManager.Bind(this, "DateFont", dateFontControl, "Value", true);

            var alarmFontControl = new FontSettingsControl
            {
                Caption = "Alarm Font".Localize(),
            };
            controls.Add(alarmFontControl);
            bindingManager.Bind(this, "AlarmFont", alarmFontControl, "Value", true);


            // hide control for icon / caption selection
            foreach (var control in controls)
                if (control.Name.Contains("Icon"))
                {
                    controls.Remove(control);
                    break;
                }
            foreach (var control in controls)
                if (control.Name.Contains("Caption"))
                {
                    controls.Remove(control);
                    break;
                }
            foreach (var control in controls)
                if (control.Name.Contains("CaptionFont"))
                {
                    controls.Remove(control);
                    break;
                }
           
            return controls;
        }

        
    }

}
