using System;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;
using Metrohome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    [TileInfo("Digital clock")]
    public class DigitalClockWidget : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;

        private Boolean _showPoints = true;
        private Boolean _is24Hour = true;
        private string _dateFormat = _dateFormats[0];

        private int _paddingRight;
        private int _dotWidth;
        private int _dotPaddingRight;
        private int _dotPaddingLeft;

        private TextStyle _timeFont = new TextStyle(MetroTheme.PhoneFontFamilySemiLight, 36, MetroTheme.PhoneForegroundBrush);
        private TextStyle _dateFont = new TextStyle(MetroTheme.PhoneFontFamilySemiLight, 10, MetroTheme.PhoneForegroundBrush);


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
                _dotWidth = 20;
                _dotPaddingRight = 8;
                _dotPaddingLeft = 5;
            }
            else
            {
                _timeFont.FontSize = 22;
                _dateFont.FontSize = 8;
                //_fntTime = new Font(MetroTheme.PhoneFontFamilySemiBold, 24.ToLogic(), FontStyle.Regular);
                _paddingRight = 8;
                _dotWidth = 10;
                _dotPaddingRight = 4;
                _dotPaddingLeft = 0;
            }
        }

        protected override Size[] GetSizes()
        {
            return new Size[] { 
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

            var fntTime = new Font(_timeFont.FontFamily, _timeFont.FontSize.ToLogic(), FontStyle.Regular);
            var fntDate = new Font(_dateFont.FontFamily, _dateFont.FontSize.ToLogic(), FontStyle.Regular);

            var brhTime = new SolidBrush(_timeFont.Foreground);
            var brhDate = new SolidBrush(_dateFont.Foreground);

            var sTimeHour = (Is24Hour) ? DateTime.Now.ToString("HH") : DateTime.Now.ToString("hh");
            var sTimeMins = DateTime.Now.ToString("mm");
            var sDate = DateTime.Now.ToString(DateFormat);

            var timeBox = g.MeasureString("99", fntTime);
            var dateBox = g.MeasureString(sDate, fntDate);

            g.DrawString(sTimeMins, fntTime, brhTime,
                rect.Right - timeBox.Width - _paddingRight,
                rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2);
            if (_showPoints)
                g.DrawString(":", fntTime, brhTime,
                    rect.Right - timeBox.Width - _paddingRight - _dotWidth - _dotPaddingRight,
                    rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2 - ScreenRoutines.Scale(5));
            g.DrawString(sTimeHour, fntTime, brhTime,
                rect.Right - timeBox.Width - _paddingRight - _dotWidth - _dotPaddingRight - _dotPaddingLeft - timeBox.Width,
                rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2);

            g.DrawString(sDate, fntDate, brhDate,
                         rect.Right - dateBox.Width - _paddingRight,
                         rect.Bottom - dateBox.Height);
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

            var flagControl = new FlagSettingsControl
                                  {
                                      Caption = "24-Hours", 
                                  };
            controls.Add(flagControl);
            bindingManager.Bind(this, "Is24Hour", flagControl, "Value", true);

            var formatControl = new SelectSettingsControl
                                    {
                                        Caption = "Date format",
                                        Items = _dateSamples,
                                    };
            controls.Add(formatControl);
            bindingManager.Bind(this, "DateSampleIndex", formatControl, "SelectedIndex", true);


            var timeFontControl = new FontSettingsControl
                                          {
                                              Caption = "Time Font", 
                                          };
            controls.Add(timeFontControl);
            bindingManager.Bind(this, "TimeFont", timeFontControl, "Value", true);

            var dateFontControl = new FontSettingsControl
                                          {
                                              Caption = "Date Font", 
                                          };
            controls.Add(dateFontControl);
            bindingManager.Bind(this, "DateFont", dateFontControl, "Value", true);


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
           
            return controls;
        }

        
    }

}
