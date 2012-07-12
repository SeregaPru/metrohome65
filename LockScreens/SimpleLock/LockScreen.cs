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
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;
using MetroHome65.Routines.Settings;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.SimpleLock
{
    public class SimpleLockSettings : CustomSettings, ILockScreenSettings
    {
        [LockScreenParameter]
        public string Background { get; set; }

        public virtual ICollection<UIElement> EditControls(FleuxControlPage settingsPage, BindingManager bindingManager)
        {
            var controls = new List<UIElement>();

            var ctrLockScreenImage = new ImageSettingsControl
            {
                Caption = "Lock screen background",
                //!!Value = Settings.LockScreenImage,
            };
            controls.Add(ctrLockScreenImage);
            bindingManager.Bind(this, "Background", ctrLockScreenImage, "Value");

            return controls;
        }
    }


    [LockScreenInfo("Simple lock screen")]
    public class SimpleLock : Canvas, IActive, ILockScreen
    {
        private const string DateFormat = "HH:mm\ndddd\nMMMM d";

        private TextElement _lblClock;
        private ScaledBackground _background;

        private ThreadTimer _updateTimer;

        private readonly TextStyle _style = new TextStyle(
            MetroTheme.PhoneFontFamilyNormal,
            MetroTheme.PhoneFontSizeExtraLarge,
            MetroTheme.PhoneForegroundBrush);


        [LockScreenSettings]
        public SimpleLockSettings Settings { get; private set; }

        public SimpleLock()
        {
            CreateSettings();

            CreateVisual();
        }

        protected virtual void CreateSettings()
        {
            Settings = new SimpleLockSettings();
        }

        // ILockScreen
        public virtual void ApplySettings(ILockScreenSettings settings)
        {
            _background.Image = (settings as SimpleLockSettings).Background;
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

        protected virtual string GetText()
        {
            return DateTime.Now.ToString(DateFormat);
        }

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(2000, UpdateTime );
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
                    Location = new Point(- (int)(Math.Round(Math.Abs(Math.Sin(v / 10.0) * v))), 0);
                    Update();
                },
                OnAnimationStop = () => { Location = new Point(0, 0); },
            };
            StoryBoard.BeginPlay(screenAnimation);

            return true;
        }

    }


    public class AdvancedLockSettings : SimpleLockSettings
    {
        [LockScreenParameter]
        public string DateFormat { get; set; }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage, BindingManager bindingManager)
        {
            var controls = base.EditControls(settingsPage, bindingManager);

            var ctrDateFormat = new StringSettingsControl(settingsPage)
            {
                Caption = "Date format",
                //!!Value = Settings.LockScreenImage,
            };
            controls.Add(ctrDateFormat);
            bindingManager.Bind(this, "DateFormat", ctrDateFormat, "Value");

            return controls;
        }
    }


    [LockScreenInfo("Advanced lock screen")]
    public class AdvancedLock : SimpleLock
    {
        [LockScreenSettings]
        public AdvancedLockSettings Settings { get; set; }

        protected override void CreateSettings()
        {
            Settings = new AdvancedLockSettings();
        }

        protected override string GetText()
        {
            return DateTime.Now.ToString(Settings.DateFormat);
        }

    }


}
