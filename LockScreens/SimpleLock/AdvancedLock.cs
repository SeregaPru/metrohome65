using System;
using System.Collections.Generic;
using System.Drawing;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;
using MetroHome65.Routines.Settings;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;

namespace MetroHome65.SimpleLock
{
    public class AdvancedLockSettings : CustomSettings, ILockScreenSettings
    {
        [LockScreenParameter]
        public string Background { get; set; }

        [LockScreenParameter]
        public Boolean Is24Hour { get; set; }

        public string TimeFormat { get { return (Is24Hour ? "HH" : "hh") + ":mm"; } }

        [LockScreenParameter]
        public string DateFormat { get; set; }

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

        public AdvancedLockSettings()
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

            return controls;
        }
    }

    
    /// <summary>
    /// Lock screen with background and clock with configured date format
    /// </summary>
    [LockScreenInfo("Advanced lock screen")]
    public class AdvancedLock : Canvas, IActive, ILockScreen
    {
        private TextElement _lblClock;

        private ScaledBackground _background;

        private ThreadTimer _updateTimer;

        private readonly TextStyle _style = new TextStyle(
            MetroTheme.PhoneFontFamilyNormal,
            MetroTheme.PhoneFontSizeExtraLarge,
            MetroTheme.PhoneForegroundBrush);


        [LockScreenSettings]
        public AdvancedLockSettings Settings { get; set; }


        public AdvancedLock()
        {
            CreateSettings();
            CreateVisual();
        }

        private void CreateSettings()
        {
            Settings = new AdvancedLockSettings();
        }

        private void CreateVisual()
        {
            _background = new ScaledBackground(Settings.Background);
            AddElement(_background);

            const int leftOffset = 20;
            const int rightOffset = 10;

            var lineHeight = FleuxApplication.DummyDrawingGraphics.Style(_style).CalculateMultilineTextHeight("0", 100);

            _lblClock = new TextElement(GetText())
                            {
                                Style = _style,
                                AutoSizeMode = TextElement.AutoSizeModeOptions.None,
                                Size = new Size(ScreenConsts.ScreenWidth.ToLogic() - leftOffset - rightOffset, lineHeight * 4),
                                Location = new Point(leftOffset, ScreenConsts.ScreenHeight.ToLogic() - lineHeight * 4),
                            };
            AddElement(_lblClock);

            this.TapHandler = OnTap;
        }

        private void UpdateTime()
        {
            _lblClock.Text = GetText();
            // do not call update because change text property calls update inside
        }

        private string GetText()
        {
            return DateTime.Now.ToString(Settings.TimeFormat + "\n" + 
                Settings.DateFormat.Replace(", ", "\n"));
        }

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(2000, UpdateTime);
                }
                else
                {
                    if (_updateTimer != null)
                        _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }


        /// <summary>
        /// when tap to lockscreen, plays animation
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool OnTap(Point arg)
        {
            var screenAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = 1000,
                From = 100,
                To = 0,
                OnAnimation = v =>
                {
                    Location = new Point(-(int)(Math.Round(Math.Abs(Math.Sin(v / 10.0) * v))), 0);
                    Update();
                },
                OnAnimationStop = () => { Location = new Point(0, 0); },
            };
            StoryBoard.BeginPlay(screenAnimation);

            return true;
        }


        public void ApplySettings(ILockScreenSettings settings)
        {
            Settings = settings as AdvancedLockSettings;
            _background.Image = Settings.Background;
            UpdateTime();
        }
    }
}