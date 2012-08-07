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

        private static readonly List<String> _TimefontSizes = new List<String>() { "20", "22", "24", "26", "28", "30", "32", "34", "36", "38", "40", "42", "44", "46", "48", "50", };
        private readonly List<object> _TimefontSizesData;
        private static readonly List<String> _TimefontNames = new List<String>() { "Segoe WP", "Segoe WP Light", "Segoe WP SemiLight", "Segoe WP Semibold" };
        private readonly List<object> _TimefontNamesData;

        private int _TimefontSize = Int16.Parse(_TimefontSizes[8]);
        private Color _TimefontColor = MetroTheme.TileTextStyle.Foreground;
        private string _TimefontName = _TimefontNames[3];


        private static readonly List<String> _DatefontSizes = new List<String>() { "6", "7", "8", "9", "10", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30" };
        private readonly List<object> _DatefontSizesData;
        private static readonly List<String> _DatefontNames = new List<String>() { "Segoe WP", "Segoe WP Light", "Segoe WP SemiLight", "Segoe WP Semibold" };
        private readonly List<object> _DatefontNamesData;

        private int _DatefontSize = Int16.Parse(_DatefontSizes[4]);
        private Color _DatefontColor = MetroTheme.TileTextStyle.Foreground;
        private string _DatefontName = _DatefontNames[3];

        private readonly Brush _brushCaption;
        private readonly Font _fntDate;
        private Font _fntTime;
        private Boolean _showPoints = true;
        private Boolean _is24Hour = true;
        private string _dateFormat = _dateFormats[0];

        private int _paddingRight;
        private int _dotWidth;
        private int _dotPaddingRight;
        private int _dotPaddingLeft;

        public DigitalClockWidget() 
        {
            _brushCaption = new SolidBrush(MetroTheme.TileTextStyle.Foreground);

            // fill date examples using current date and formats
            var currentDate = DateTime.Now;
            _dateSamples = new List<object>();
            foreach (var format in _dateFormats) 
            {
                _dateSamples.Add(currentDate.ToString(format));
            }

            //_fntDate = new Font(MetroTheme.PhoneFontFamilySemiBold, 10.ToLogic(), FontStyle.Regular);
            _TimefontSizesData = new List<object>();
            foreach (string size in _TimefontSizes)
            {
                _TimefontSizesData.Add(size);
            }
            _TimefontNamesData = new List<object>();
            foreach (string name in _TimefontNames)
            {
                _TimefontNamesData.Add(name);
            }

            _DatefontSizesData = new List<object>();
            foreach (string size in _DatefontSizes)
            {
                _DatefontSizesData.Add(size);
            }
            _DatefontNamesData = new List<object>();
            foreach (string name in _DatefontNames)
            {
                _DatefontNamesData.Add(name);
            }

            ApplySizeSettings();
        }

        private void ApplySizeSettings()
        {
            if (this.GridSize.Width == 4)
            {
                _TimefontSize = 36;
                _DatefontSize = 10;
                //_fntTime = new Font(MetroTheme.PhoneFontFamilySemiBold, 36.ToLogic(), FontStyle.Regular);
                _paddingRight = 20;
                _dotWidth = 20;
                _dotPaddingRight = 8;
                _dotPaddingLeft = 5;
            }
            else
            {
                _TimefontSize = 22;
                _DatefontSize = 8;
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

        // Time Font - [Size] [Color] [Name]
        [TileParameter]
        public String TimeFontName
        {
            get { return _TimefontName; }
            set
            {
                _TimefontName = value;
                NotifyPropertyChanged("TimeFontName");
            }
        }

        public int TimeFontNameIndex
        {
            get
            {
                for (int i = 0; i < _TimefontNames.Count; i++) 
                    if (_TimefontNames[i] == TimeFontName) 
                        return i;
                return 0;
            }
            set { 
                if ((value >= 0) && (value < _TimefontNames.Count)) 
                    TimeFontName = _TimefontNames[value]; 
            }
        }

        [TileParameter]
        public int TimeFontSize
        {
            get { return _TimefontSize; }
            set
            {
                _TimefontSize = value;
                NotifyPropertyChanged("TimeFontSize");
            }
        }

        public int TimeFontSizeIndex
        {
            get
            {
                for (int i = 0; i < _TimefontSizes.Count; i++)
                    if (_TimefontSizes[i] == Convert.ToString(TimeFontSize)) return i;
                return 0;
            }
            set
            {
                if ((value >= 0) && (value < _TimefontSizes.Count)) 
                    TimeFontSize = Convert.ToInt32(_TimefontSizes[value]);
            }
        }

        [TileParameter]
        public int TimeFontColor
        {
            get { return _TimefontColor.ToArgb(); }
            set { _TimefontColor = Color.FromArgb(value); }
        }

        public Color TimeFontColorIndex
        {
            get { return _TimefontColor; }
            set
            {
                _TimefontColor = value;
                NotifyPropertyChanged("TimeFontColor");
            }
        }

        // Date Font - [Size] [Color] [Name]
        [TileParameter]
        public String DateFontName
        {
            get { return _DatefontName; }
            set
            {
                _DatefontName = value;
                NotifyPropertyChanged("DateFontName");
            }
        }

        public int DateFontNameIndex
        {
            get
            {
                for (int i = 0; i < _DatefontNames.Count; i++) 
                    if (_DatefontNames[i] == DateFontName) 
                        return i;
                return 0;
            }
            set
            {
                if ((value >= 0) && (value < _DatefontNames.Count)) 
                    DateFontName = _DatefontNames[value];
            }
        }

        [TileParameter]
        public int DateFontSize
        {
            get { return _DatefontSize; }
            set
            {
                _DatefontSize = value;
                NotifyPropertyChanged("DateFontSize");
            }
        }

        public int DateFontSizeIndex
        {
            get
            {
                for (int i = 0; i < _DatefontSizes.Count; i++)
                    if (_DatefontSizes[i] == Convert.ToString(DateFontSize)) return i;
                return 0;
            }
            set { 
                if ((value >= 0) && (value < _DatefontSizes.Count)) 
                    DateFontSize = Convert.ToInt32(_DatefontSizes[value]); 
            }
        }

        [TileParameter]
        public int DateFontColor
        {
            get { return _DatefontColor.ToArgb(); }
            set { _DatefontColor = Color.FromArgb(value); }
        }

        public Color DateFontColorIndex
        {
            get { return _DatefontColor; }
            set
            {
                _DatefontColor = value;
                NotifyPropertyChanged("DateFontColor");
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

            var _fntTime = new Font(_TimefontName, _TimefontSize.ToLogic(), FontStyle.Regular);
            var _fntDate = new Font(_DatefontName, _DatefontSize.ToLogic(), FontStyle.Regular);

            var _brhTime = new SolidBrush(_TimefontColor);
            var _brhDate = new SolidBrush(_DatefontColor);

            var sTimeHour = (Is24Hour) ? DateTime.Now.ToString("HH") : DateTime.Now.ToString("hh");
            var sTimeMins = DateTime.Now.ToString("mm");
            var sDate = DateTime.Now.ToString(DateFormat);

            var timeBox = g.MeasureString("99", _fntTime);
            var dateBox = g.MeasureString(sDate, _fntDate);

            //g.DrawString(sTimeMins, _fntTime, _brushCaption,
            g.DrawString(sTimeMins, _fntTime, _brhTime,
                rect.Right - timeBox.Width - _paddingRight,
                rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2);
            if (_showPoints)
                //g.DrawString(":", _fntTime, _brushCaption,
                g.DrawString(":", _fntTime, _brhTime,
                    rect.Right - timeBox.Width - _paddingRight - _dotWidth - _dotPaddingRight,
                    rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2 - ScreenRoutines.Scale(5));
            //g.DrawString(sTimeHour, _fntTime, _brushCaption,
            g.DrawString(sTimeHour, _fntTime, _brhTime,
                rect.Right - timeBox.Width - _paddingRight - _dotWidth - _dotPaddingRight - _dotPaddingLeft - timeBox.Width,
                rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2);

            //g.DrawString(sDate, _fntDate, _brushCaption,
            g.DrawString(sDate, _fntDate, _brhDate,
                         rect.Right - dateBox.Width - _paddingRight,
                         rect.Bottom - dateBox.Height);
            //rect.Bottom - (rect.Height - timeBox.Height - dateBox.Height) / 2 - dateBox.Height);
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

            /*
            var formatControl = new SelectSettingsControl
                                    {
                                        Caption = "Date format",
                                        Items = _dateSamples,
                                    };
            controls.Add(formatControl);
            bindingManager.Bind(this, "DateSampleIndex", formatControl, "SelectedIndex", true);
            */

            var timeFontNameControl = new SelectSettingsControl
                                          {
                                              Caption = "Time Font Name", 
                                              Items = _TimefontNamesData,
                                          };
            controls.Add(timeFontNameControl);
            bindingManager.Bind(this, "TimeFontNameIndex", timeFontNameControl, "SelectedIndex", true);

            var timeFontSizeControl = new SelectSettingsControl
                                          {
                                              Caption = "Time Font Size", 
                                              Items = _TimefontSizesData,
                                          };
            controls.Add(timeFontSizeControl);
            bindingManager.Bind(this, "TimeFontSizeIndex", timeFontSizeControl, "SelectedIndex", true);

            var timeFontColorControl = new ColorSettingsControl(true)
                                           {
                                               Caption = "Time Font Color",
                                           };
            controls.Add(timeFontColorControl);
            bindingManager.Bind(this, "TimeFontColorIndex", timeFontColorControl, "Value", true);


            var dateFontNameControl = new SelectSettingsControl
                                          {
                                              Caption = "Date Font Name", 
                                              Items = _DatefontNamesData,
                                          };
            controls.Add(dateFontNameControl);
            bindingManager.Bind(this, "DateFontNameIndex", dateFontNameControl, "SelectedIndex", true);

            var dateFontSizeControl = new SelectSettingsControl
                                          {
                                              Caption = "Date Font Size", 
                                              Items = _DatefontSizesData,
                                          };
            controls.Add(dateFontSizeControl);
            bindingManager.Bind(this, "DateFontSizeIndex", dateFontSizeControl, "SelectedIndex", true);

            var dateFontColorControl = new ColorSettingsControl(true)
                                           {
                                               Caption = "DateFont Color",
                                           };
            controls.Add(dateFontColorControl);
            bindingManager.Bind(this, "DateFontColorIndex", dateFontColorControl, "Value", true);


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
