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

        private readonly Pen _pen;

        private Boolean _isShowDate = true;
        private Boolean _isShowCalendar = true;

        private static readonly OutlookSession Session = new OutlookSession();
        private static string _oldAppointments = "";


        protected override Size[] GetSizes()
        {
            return new[] { new Size(4, 2) };
        }


        private TextStyle _weekFont = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, 11, Color.White);

        [TileParameter]
        public TextStyle WeekFont
        {
            get { return _weekFont; }
            set
            {
                if (_weekFont != value)
                {
                    _weekFont = value;
                    NotifyPropertyChanged("WeekFont");
                }
            }
        }

        private TextStyle _dateFont = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, 9, Color.White);

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

        private TextStyle _timeFont = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, 14, Color.White);

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


        [TileParameter] // Показывать дату
        public Boolean IsShowDate
        {
            get { return _isShowDate; }
            set
            {
                if (_isShowDate == value) 
                    return; 
                _isShowDate = value; 
                NotifyPropertyChanged("IsShowDate");
            }
        }

        // Показывать календарь
        [TileParameter] 
        public Boolean IsShowCalendar
        {
            get { return _isShowCalendar; }
            set
            {
                if (_isShowCalendar == value) 
                    return; 
                _isShowCalendar = value; 
                NotifyPropertyChanged("IsShowCalendar");
            }
        }


        private static readonly List<String> TimeScreens = new List<String>() { "1", "2", "4", "6", "8", "10", "12", "24", "48", "72" };
        private readonly List<object> _timeScreensData;
        private int _timeScreen = int.Parse(TimeScreens[3]); 

        [TileParameter]
        public int TimeScreen { 
            get { return _timeScreen; } 
            set
            {
                _timeScreen = value; 
                NotifyPropertyChanged("TimeScreen");
            } 
        }

        public int TimeScreenIndex
        {
            get
            {
                for (var i = 0; i < TimeScreens.Count; i++) 
                    if (TimeScreens[i] == Convert.ToString(TimeScreen)) 
                        return i; 
                return 0;
            }
            set
            {
                if ((value >= 0) && (value < TimeScreens.Count)) 
                    TimeScreen = Convert.ToInt32(TimeScreens[value]);
            }
        }


        public WPAppointment()
        {
            _timeScreensData = new List<object>(); foreach (var time in TimeScreens) { _timeScreensData.Add(time); }

            _pen = new Pen(Color.WhiteSmoke);
        }


        // overriding paint icon method - don't paint icon
        protected override void PaintIcon(Graphics g, Rectangle rect) { }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);

            var dayFont = new Font(_dateFont.FontFamily, _dateFont.FontSize.ToLogic(), FontStyle.Regular);
            var dayBrush = new SolidBrush(_dateFont.Foreground);

            // if there is appointments, write them
            if (_oldAppointments != "")
            {
                var aptRect = rect;

                if (_isShowDate)
                {
                    // write date
                    var day = DateTime.Now.ToString("ddd d"); // three-letter day of the WEEK & day of the MONTH
                    var box = g.MeasureString(day, dayFont);

                    var rec = rect;
                    rec.X += 10; 
                    rec.Width -= 10;
                    rec.Height -= (int)box.Height;
                    aptRect = rec;

                    rec = rect;
                    rec.X = rec.Width - (int)box.Width - 10; 
                    rec.Width = (int)box.Width;
                    rec.Y = rec.Height - (int)box.Height - 5; 
                    rec.Height = (int)box.Height;

                    g.DrawString(day, dayFont, dayBrush, rec);
                }

                // write appointment
                var aptFont = new Font(CaptionFont.FontFamily, CaptionFont.FontSize.ToLogic(), FontStyle.Regular);
                g.DrawString(_oldAppointments, aptFont, new SolidBrush(CaptionFont.Foreground), aptRect);
            }
            else

            // if there is NO appointments, write calendar, or nothing
            {
                if (_isShowCalendar)
                {
                    //var q = (int)(rect.Height * 0.35);
                    (new AlphaImage("WPAppointment.Images.calendar.png", this.GetType().Assembly)).
                        PaintIcon(g, CaptionLeftOffset.ToLogic(), CaptionBottomOffset.ToLogic());
                        //PaintBackground(g, new Rectangle(1.ToLogic(), 1.ToLogic(), q, q));


                    var x = 0;
                    var y = 0;

                    var s = DateTime.Now.ToString("dddd, d MMMM"); // full day of the WEEK & day of the MONTH & full month string
                    var dayBox = g.MeasureString(s, dayFont);
                    g.DrawString(s, dayFont, dayBrush, 
                        (rect.Width - (int)(dayBox.Width) - CaptionLeftOffset.ToLogic()), -CaptionBottomOffset.ToLogic());
                    y = y + (int)(dayBox.Height) - 8.ToLogic();

                    s = DateTime.Now.ToString("HH:mm");
                    var timeFont = new Font(TimeFont.FontFamily, TimeFont.FontSize.ToLogic(), FontStyle.Regular);
                    var timeBox = g.MeasureString(s, timeFont);
                    g.DrawString(s, timeFont, new SolidBrush(TimeFont.Foreground),
                        (rect.Width - (int)(timeBox.Width) - CaptionLeftOffset.ToLogic()), y);
                    y = y + (int)(timeBox.Height) - 8.ToLogic();

                    var weekFont = new Font(WeekFont.FontFamily, WeekFont.FontSize.ToLogic(), FontStyle.Regular);
                    var weekBrush = new SolidBrush(WeekFont.Foreground);

                    var dayWidth = (int)(rect.Width / 8);
                    var dayHeight = 0;
                    foreach (var dayName in new[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" })
                    {
                        var weekdayBox = g.MeasureString(dayName, weekFont);
                        dayHeight = (int) Math.Max(dayHeight, weekdayBox.Height);
                    }
                    var numHeight = (int) g.MeasureString("0", weekFont).Height;

                    // текущее время
                    var currentWeekDay = (int)DateTime.Now.DayOfWeek;
                    if (currentWeekDay == 0) currentWeekDay = 7;
                    var dayNum = 1;

                    foreach (var dayName in new[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" })
                    {
                        // день недели
                        var weekdayBox = g.MeasureString(dayName, weekFont);
                        var dayX = (dayNum * dayWidth) - ((int)(weekdayBox.Width) / 2);
                        g.DrawString(dayName, weekFont, weekBrush, dayX, y);

                        // число месяца
                        var currentWeekStart = DateTime.Now.Date.AddDays(1 - currentWeekDay + dayNum);
                        s = currentWeekStart.Day.ToString();
                        g.DrawString(s, weekFont, weekBrush, dayX, y + dayHeight - 5);
                       
                        dayNum++;
                    }

                    #warning  Проблема с отображение в QVGA
                    g.DrawRectangle(_pen, (currentWeekDay * dayWidth) - (dayWidth / 2),
                        y + dayHeight - 1, dayWidth - 1, numHeight - 4);
                }
                else
                {
                    var aptFont = new Font(CaptionFont.FontFamily, CaptionFont.FontSize.ToLogic(), FontStyle.Regular);
                    var noApts = string.Format("До {0}\nзадачи не обнаружены", DateTime.Now.AddHours(_timeScreen).ToShortTimeString());
                    g.DrawString(noApts, aptFont, new SolidBrush(CaptionFont.Foreground), CaptionLeftOffset, CaptionBottomOffset);
                }
            }
        }

        
        public bool UpdateStatus()
        {
            var s = "";
            var start = DateTime.Now;
            var end = DateTime.Now.AddHours(_timeScreen);
          
            for (var i = 0; i < Session.Appointments.Items.Count; ++i)
            {
                var app = Session.Appointments.Items[i];

                if ((app.End < start) || (app.Start > end)) continue;

                if (app.AllDayEvent)
                {
                    s += "Весь день".Localize() + " " + app.Location + " " + app.Subject + " " + app.Body + "\n";
                }
                else
                {
                    if (app.Start.Date == DateTime.Now.Date)
                    {
                        s += app.Start.ToShortTimeString() + "-" + app.End.ToShortTimeString() + " " + app.Location + " " + app.Subject;
                    }
                    else
                    {
                        s += app.Start.ToShortDateString() + " " + app.Start.ToShortTimeString() + "-" + app.End.ToShortTimeString() + " " + app.Location + " " + app.Subject;
                    }
                    s += (app.Body != "") ? " " + app.Body : "\n";
                }
            }

            if (_oldAppointments != s)
            {
                _oldAppointments = s; 
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

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();
            
            var showDateControl = new FlagSettingsControl { Caption = "Show Date", };
            controls.Add(showDateControl); bindingManager.Bind(this, "IsShowDate", showDateControl, "Value", true);

            var showCalendarControl = new FlagSettingsControl { Caption = "Show Calendar", };
            controls.Add(showCalendarControl); bindingManager.Bind(this, "IsShowCalendar", showCalendarControl, "Value", true);

            var timeScreenControl = new SelectSettingsControl { Caption = "Time Screen", Items = _timeScreensData, };
            controls.Add(timeScreenControl);
            bindingManager.Bind(this, "TimeScreenIndex", timeScreenControl, "SelectedIndex", true);


            var dateFontControl = new FontSettingsControl
            {
                Caption = "Date Font".Localize(),
                Name = "DateFont",
            };
            controls.Add(dateFontControl);
            bindingManager.Bind(this, "DateFont", dateFontControl, "Value", true);


            var timeFontControl = new FontSettingsControl
            {
                Caption = "Time Font".Localize(),
                Name = "TimeFont",
            };
            controls.Add(timeFontControl);
            bindingManager.Bind(this, "TimeFont", timeFontControl, "Value", true);


            var weekFontControl = new FontSettingsControl
            {
                Caption = "Week Font".Localize(),
                Name = "WeekFont",
            };
            controls.Add(weekFontControl);
            bindingManager.Bind(this, "WeekFont", weekFontControl, "Value", true);


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
