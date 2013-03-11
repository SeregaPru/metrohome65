// MicronSys and Dev 2012 year
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
using Microsoft.WindowsMobile.PocketOutlook;

namespace MetroHome65.Widgets
{
    [TileInfo("WP Appointment")]
    public class WPAppointment : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;

        // ******************** КАЛЕНДАРЬ ******************
        private readonly Brush _brush;
        private readonly Font _fntDate;
        private readonly Font _fntTime;
        private readonly Font _fntWeek;
        private readonly Font _fntDay;
        private readonly Pen _pen;


        // *************************************************



        private static readonly List<String> _timeScreens = new List<String>() {"1","2","4","6","8","10","12","24","48","72" };
        private readonly List<object> _timeScreensData;

        
        private static readonly List<String> _fontSizes = new List<String>() {"6","7","8","9","10","12","14","16","18","20","22","24","26","28","30" };
        private readonly List<object> _fontSizesData;

        private static readonly List<String> _fontNames = new List<String>() {"Segoe WP","Segoe WP Light","Segoe WP SemiLight","Segoe WP Semibold" };
        private readonly List<object> _fontNamesData;

        private int _fontSize = int.Parse(_fontSizes[3]);
        private Color _fontColor = MetroTheme.TileTextStyle.Foreground;
        private string _fontName = _fontNames[3];

        private Boolean _isShowDate = true;
        private Boolean _isShowCalendar = true;
        private int _timeScreen = int.Parse(_timeScreens[3]); 

        private static OutlookSession _session = new OutlookSession();
        private static string _oldAppointments = "";

        public WPAppointment() 
        {
            _timeScreensData = new List<object>(); foreach (var time in _timeScreens) { _timeScreensData.Add(time); }
            _fontSizesData = new List<object>(); foreach (var size in _fontSizes){ _fontSizesData.Add(size); }
            _fontNamesData = new List<object>(); foreach (var name in _fontNames) { _fontNamesData.Add(name); }

            _brush = new SolidBrush(MetroTheme.TileTextStyle.Foreground);
            _fntDate = new Font(MetroTheme.PhoneFontFamilySemiBold, 9.ToLogic(), FontStyle.Regular);
            _fntTime = new Font(MetroTheme.PhoneFontFamilySemiBold, 14.ToLogic(), FontStyle.Regular);
            _fntWeek = new Font(MetroTheme.PhoneFontFamilySemiBold, 12.ToLogic(), FontStyle.Regular);
            _fntDay = new Font(MetroTheme.PhoneFontFamilySemiBold, 12.ToLogic(), FontStyle.Regular);
            _pen = new Pen(Color.WhiteSmoke);

            ApplySizeSettings();
        }

        private void ApplySizeSettings()
        {
            return ;
        }


        protected override Size[] GetSizes() { return new Size[] { new Size(4, 2) }; }

        protected override void GridSizeChanged() { ApplySizeSettings(); }


        /// <summary>
        /// Format for date
        /// </summary>



        [TileParameter] // Показывать дату
        public Boolean IsShowDate
        {
            get { return _isShowDate; }
            set { if (_isShowDate == value) return; _isShowDate = value; NotifyPropertyChanged("IsShowDate"); }
        }

        [TileParameter] // Показывать календарь
        public Boolean IsShowCalendar
        {
            get { return _isShowCalendar; }
            set { if (_isShowCalendar == value) return; _isShowCalendar = value; NotifyPropertyChanged("IsShowCalendar"); }
        }

        [TileParameter]
        public int TimeScreen { get { return _timeScreen; } set { _timeScreen = value; NotifyPropertyChanged("TimeScreen"); } }
        public int TimeScreenIndex
        {
            get { for (var i = 0; i < _timeScreens.Count; i++) if (_timeScreens[i] == Convert.ToString(TimeScreen)) return i; return 0; }
            set { if ((value >= 0) && (value < _timeScreens.Count)) TimeScreen = Convert.ToInt32(_timeScreens[value]); }
        }


        //******************** Настройки Шрифта
        [TileParameter]
        public String FontName { get { return _fontName; } set { _fontName = value; NotifyPropertyChanged("FontName"); } }

        public int FontNameIndex
        {
            get { for (var i = 0; i < _fontNames.Count; i++) if (_fontNames[i] == FontName) return i; return 0; }
            set { if ((value >= 0) && (value < _fontNames.Count)) FontName = _fontNames[value]; }
        }
        [TileParameter]
        public int FontSize { get { return _fontSize; } set { _fontSize = value; NotifyPropertyChanged("FontSize"); } }
        public int FontSizeIndex
        {
            get { for (var i = 0; i < _fontSizes.Count; i++) if (_fontSizes[i] == Convert.ToString(FontSize)) return i; return 0; }
            set { if ((value >= 0) && (value < _fontSizes.Count)) FontSize = Convert.ToInt32(_fontSizes[value]); }
        }
        [TileParameter]
        public int FontColor { get { return _fontColor.ToArgb(); } set { _fontColor = Color.FromArgb(value); } }
        public Color FontColorIndex { get { return _fontColor; } set { _fontColor = value; NotifyPropertyChanged("FontColor"); } }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            var _fnt = new Font(_fontName, _fontSize.ToLogic(), FontStyle.Regular);
            var _brh = new SolidBrush(_fontColor);

            if (_oldAppointments != "")
            {
                if (_isShowDate)
                {
                    var _fntD = new Font(_fontName, 14.ToLogic(), FontStyle.Regular);
                    var _day = DateTime.Now.ToString("ddd d");
                    var _box = g.MeasureString(_day, _fntD);
                    var _rec = rect;

                    _rec.X += 10; _rec.Width -= 10;
                    _rec.Height -= (int)_box.Height;

                    g.DrawString(_oldAppointments, _fnt, _brh, _rec);

                    _rec = rect;
                    _rec.X = _rec.Width - (int)_box.Width - 10; _rec.Width = (int)_box.Width;
                    _rec.Y = _rec.Height - (int)_box.Height - 5; _rec.Height = (int)_box.Height;

                    g.DrawString(_day, _fntD, _brh, _rec);
                }
                else
                {
                    g.DrawString(_oldAppointments, _fnt, _brh, rect);
                }
            }
            else
            {
                if (_isShowCalendar)
                {

                    var _q = (int)(rect.Height * 0.35);
                    (new AlphaImage("WPAppointment.Images.calendar.png", this.GetType().Assembly)).PaintBackground(g, new Rectangle(1.ToLogic(), 1.ToLogic(), _q, _q));

                    var now = DateTime.Now; // текущее время
                    var currentWeekDay = (int)now.DayOfWeek;
                    if (currentWeekDay == 0) currentWeekDay = 7;
                    var currentWeekStart = now.Date.AddDays(1 - currentWeekDay);

                    var x = 0.ToLogic();
                    var y = 0.ToLogic();
                    //var i = 0;
                    var t = (int)(rect.Width / 8);

                    var _s = DateTime.Now.ToString("dddd, d MMMM");
                    var _box = g.MeasureString(_s, _fntDate);
                    g.DrawString(_s, _fntDate, _brush, (rect.Width - (int)(_box.Width) - 5.ToLogic()), -3.ToLogic());
                    y = y + (int)(_box.Height) - 8.ToLogic();

                    _s = DateTime.Now.ToString("HH:mm");
                    _box = g.MeasureString(_s, _fntTime);
                    g.DrawString(_s, _fntTime, _brush, (rect.Width - (int)(_box.Width) - 5.ToLogic()), y);
                    y = y + (int)(_box.Height) - 8.ToLogic();

                    _s = "Пн Вт Ср Чт Пт Сб Вс";
                    _box = g.MeasureString(_s, _fntWeek);

                    DrawWeek(g, 1, t, y, "Пн");
                    DrawWeek(g, 2, t, y, "Вт");
                    DrawWeek(g, 3, t, y, "Ср");
                    DrawWeek(g, 4, t, y, "Чт");
                    DrawWeek(g, 5, t, y, "Пт");
                    DrawWeek(g, 6, t, y, "Сб");
                    DrawWeek(g, 7, t, y, "Вс");

                    y = y + (int)(_box.Height) - 5.ToLogic();

                    for (int i = 0; i <= 6; i++)
                    {

                        _s = currentWeekStart.Day.ToString();
                        _box = g.MeasureString(_s, _fntWeek);
                        x = ((i + 1) * t) - ((int)(_box.Width) / 2);
                        g.DrawString(_s, _fntWeek, _brush, x, y);
                        currentWeekStart = currentWeekStart.AddDays(1);
                    }

#warning  Проблема с отображение в QVGA
                    g.DrawRectangle(_pen, (currentWeekDay * t) - (t / 2), y + 3, t - 1, (int)(_box.Height) - 1);

                
                
                }
                else
                {
                    g.DrawString("До " + DateTime.Now.AddHours(_timeScreen).ToShortTimeString() + " задачи не обнаружены", _fnt, _brh, rect);
                }
            }
        }
        
        public void DrawWeek(Graphics g, int i, int t, int y, string _s)
        {

            var _box = g.MeasureString(_s, _fntWeek);
            var x = (i * t) - ((int)(_box.Width) / 2);
            g.DrawString(_s, _fntWeek, _brush, x, y);
        }

        public bool UpdateStatus()
        {
            string _s = "";
            DateTime _start = DateTime.Now;
            DateTime _end = DateTime.Now.AddHours(_timeScreen);

            //По ДеМоргану: bool intersect = date_2_end >= date_1_start && date_2_start <= date_1_end;
            
            for (int i = 0; i < _session.Appointments.Items.Count; ++i)
            {
                Appointment app = _session.Appointments.Items[i];
                if ((app.End >= _start) && (app.Start <= _end))
                {
                    if (app.AllDayEvent)
                    {
                        _s += "Весь день " + app.Location + " " + app.Subject + " " + app.Body + "\n";
                    }
                    else
                    {
                        if (app.Start.Date == DateTime.Now.Date)
                        {

                            _s += app.Start.ToShortTimeString() + "-" + app.End.ToShortTimeString() + " " + app.Location + " " + app.Subject;
                            if (app.Body != "") _s += " " + app.Body; else _s += "\n"; 
                        }
                        else
                        {
                            _s += app.Start.ToShortDateString() + " " + app.Start.ToShortTimeString() + "-" + app.End.ToShortTimeString() + " " + app.Location + " " + app.Subject;
                            if (app.Body != "") _s += " " + app.Body; else _s += "\n"; 
                        }
                    }
                }
            }
            if (_oldAppointments != _s) { _oldAppointments = _s; return true; }
            return true;
        }


        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null) _updateTimer = new ThreadTimer(10000, () =>
                        {
                            if (UpdateStatus()) ForceUpdate(); // Не забудь НУЖНО ДРУГОЕ РЕШЕНИЕ !!!!!!!
                        });
                }
                else
                {
                    if (_updateTimer != null) _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        // overriding paint icon method - don't paint icon
        protected override void PaintIcon(Graphics g, Rectangle rect) { }

        // overriding paint caption method - don't paint caption
        //protected override void PaintCaption(Graphics g, Rectangle rect) { }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();


            var IsShowDateControl = new FlagSettingsControl { Caption = "Show Date", };
            controls.Add(IsShowDateControl); bindingManager.Bind(this, "IsShowDate", IsShowDateControl, "Value", true);

            var IsShowCalendarControl = new FlagSettingsControl { Caption = "Show Calendar", };
            controls.Add(IsShowCalendarControl); bindingManager.Bind(this, "IsShowCalendar", IsShowCalendarControl, "Value", true);

            var TimeScreenControl = new SelectSettingsControl { Caption = "Time Screen", Items = _timeScreensData, };
            controls.Add(TimeScreenControl);
            bindingManager.Bind(this, "TimeScreenIndex", TimeScreenControl, "SelectedIndex", true);
            
            
            var fontNameControl = new SelectSettingsControl {  Caption = "Font Name", Items = _fontNamesData, };
            controls.Add(fontNameControl);
            bindingManager.Bind(this, "FontNameIndex", fontNameControl, "SelectedIndex", true);

            var fontSizeControl = new SelectSettingsControl {  Caption = "Font Size", Items = _fontSizesData, };
            controls.Add(fontSizeControl);
            bindingManager.Bind(this, "FontSizeIndex", fontSizeControl, "SelectedIndex", true);

            var fontColorControl = new ColorSettingsControl(true) { Caption = "Font Color", };
            controls.Add(fontColorControl);
            bindingManager.Bind(this, "FontColorIndex", fontColorControl, "Value", true);

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
