using System;
using System.Collections.Generic;
using System.Drawing;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.Settings;
using Metrohome65.Settings.Controls;

namespace MetroHome65.WPLock
{
    public class WPLockSettings : CustomSettings, ILockScreenSettings
    {
        [LockScreenParameter]
        public string Background { get; set; }

        [LockScreenParameter]
        public Boolean Is24Hour { get; set; }

        public string TimeFormat { get { return (Is24Hour ? "HH" : "hh") + ":mm"; } }

        [LockScreenParameter]
        public string DateFormat { get; set; }

        // MicronSys and Dev
        // Top Bar 
        [LockScreenParameter] 
        public Boolean TopBarWhiteIcon { get ; set; }
        [LockScreenParameter]
        public Boolean TopBarColorBattery { get; set; }
        [LockScreenParameter]
        public int TopBarFontColor { get ; set; }
        public Color TopBarFontColorIndex { get { return Color.FromArgb(TopBarFontColor); } set { TopBarFontColor =  value.ToArgb(); } }
        
        // Media Player
        [LockScreenParameter]
        public Boolean PlayerWhiteIcon { get; set;}
        [LockScreenParameter]
        public int PlayerFontColor { get; set; }
        public Color PlayerFontColorIndex { get { return Color.FromArgb(PlayerFontColor); } set { PlayerFontColor = value.ToArgb(); } }
        
        // Date Time
        [LockScreenParameter]
        public Boolean DateTimeWhiteIcon { get; set; }
        [LockScreenParameter]
        public int DateTimeFontColor { get; set; }
        public Color DateTimeFontColorIndex { get { return Color.FromArgb(DateTimeFontColor); } set { DateTimeFontColor = value.ToArgb(); } }
        
        // Appointment
        [LockScreenParameter]
        public Boolean AppointmentWhiteIcon { get; set; }
        [LockScreenParameter]
        public int AppointmentFontColor { get; set; }
        public Color AppointmentFontColorIndex { get { return Color.FromArgb(AppointmentFontColor); } set { AppointmentFontColor = value.ToArgb(); } }

        // Status
        [LockScreenParameter]
        public Boolean StatusWhiteIcon { get; set; }
        [LockScreenParameter]
        public int StatusFontColor { get; set; }
        public Color StatusFontColorIndex { get { return Color.FromArgb(StatusFontColor); } set { StatusFontColor = value.ToArgb(); } }


        // internal wrapper for date format selection
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

        public WPLockSettings()
        {
            // fill date examples using current date and formats
            var currentDate = DateTime.Now;
            _dateSamples = new List<object>();
            foreach (var format in _dateFormats)
            {
                _dateSamples.Add(currentDate.ToString(format));
            }

            DateFormat = _dateFormats[0];

        }

        public virtual ICollection<UIElement> EditControls(FleuxControlPage settingsPage, BindingManager bindingManager)
        {
            var controls = new List<UIElement>();

            var ctrLockScreenImage = new ImageSettingsControl
                                         {
                                             Caption = "Lock screen background",
                                         };
            controls.Add(ctrLockScreenImage);
            bindingManager.Bind(this, "Background", ctrLockScreenImage, "Value", true);

            var formatControl = new SelectSettingsControl
                                    {
                                        Caption = "Date format",
                                        Items = _dateSamples,
                                    };
            controls.Add(formatControl);
            bindingManager.Bind(this, "DateSampleIndex", formatControl, "SelectedIndex", true);

            var flagControl = new FlagSettingsControl
                                  {
                                      Caption = "24-Hours",
                                  };
            controls.Add(flagControl);
            bindingManager.Bind(this, "Is24Hour", flagControl, "Value", true);
            /*****************************************************************************************************/
            // Top Bar
            /* Блокируем нет пока иконок черного цвета
            var TopBarWhiteIconflagControl = new FlagSettingsControl { Caption = "TopBar white Icon", };
            controls.Add(TopBarWhiteIconflagControl);
            bindingManager.Bind(this, "TopBarWhiteIcon", TopBarWhiteIconflagControl, "Value", true);*/ 

            var TopBarColorBatteryflagControl = new FlagSettingsControl { Caption = "TopBar color battery", };
            controls.Add(TopBarColorBatteryflagControl);
            bindingManager.Bind(this, "TopBarColorBattery", TopBarColorBatteryflagControl, "Value", true);

            var TopBarFontColorControl = new ColorSettingsControl(true) { Caption = "TopBar time color", };
            controls.Add(TopBarFontColorControl);
            bindingManager.Bind(this, "TopBarFontColorIndex", TopBarFontColorControl, "Value", true);
            /*****************************************************************************************************/
            // Media Player
            var PlayerWhiteIconflagControl = new FlagSettingsControl { Caption = "Player white icon", };
            controls.Add(PlayerWhiteIconflagControl);
            bindingManager.Bind(this, "PlayerWhiteIcon", PlayerWhiteIconflagControl, "Value", true);

            var PlayerFontColorControl = new ColorSettingsControl(true) { Caption = "Player text color", };
            controls.Add(PlayerFontColorControl);
            bindingManager.Bind(this, "PlayerFontColorIndex", PlayerFontColorControl, "Value", true);
            /*****************************************************************************************************/
            // Date Time
            /* Пока не сделал отображение будильника
            var DateTimeWhiteIconflagControl = new FlagSettingsControl { Caption = "Alarm white icon", };
            controls.Add(DateTimeWhiteIconflagControl);
            bindingManager.Bind(this, "DateTimeWhiteIcon", DateTimeWhiteIconflagControl, "Value", true);*/

            var DateTimeFontColorControl = new ColorSettingsControl(true) { Caption = "Date text color", };
            controls.Add(DateTimeFontColorControl);
            bindingManager.Bind(this, "DateTimeFontColorIndex", DateTimeFontColorControl, "Value", true);
            /*****************************************************************************************************/
            // Appointment
            var AppointmentWhiteIconflagControl = new FlagSettingsControl { Caption = "Appointment white icon", };
            controls.Add(AppointmentWhiteIconflagControl);
            bindingManager.Bind(this, "AppointmentWhiteIcon", AppointmentWhiteIconflagControl, "Value", true);

            var AppointmentFontColorControl = new ColorSettingsControl(true) { Caption = "Appointment text color", };
            controls.Add(AppointmentFontColorControl);
            bindingManager.Bind(this, "AppointmentFontColorIndex", AppointmentFontColorControl, "Value", true);
            /*****************************************************************************************************/
            // Status
            var StatusWhiteIconflagControl = new FlagSettingsControl { Caption = "Status white icon", };
            controls.Add(StatusWhiteIconflagControl);
            bindingManager.Bind(this, "StatusWhiteIcon", StatusWhiteIconflagControl, "Value", true);

            var StatusFontColorControl = new ColorSettingsControl(true) { Caption = "Status text color", };
            controls.Add(StatusFontColorControl);
            bindingManager.Bind(this, "StatusFontColorIndex", StatusFontColorControl, "Value", true);

            return controls;
        }
    }
}