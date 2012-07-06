using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Metrohome65.Settings.Controls;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.SimpleLock
{
    [LockScreenInfo("Simple lock screen")]
    public sealed class SimpleLock : Canvas, IActive, ILockScreen, INotifyPropertyChanged
    {
        private readonly TextElement _lblClock;
        private string _background;

        private const string DateFormat = "HH:mm\ndddd\nMMMM d";

        private ThreadTimer _updateTimer;

        private readonly TextStyle _style = new TextStyle(
            MetroTheme.PhoneFontFamilyNormal,
            MetroTheme.PhoneFontSizeExtraLarge,
            MetroTheme.PhoneForegroundBrush);


        /// <summary>
        /// Background for lockscreen
        /// </summary>
        [LockScreenParameter]
        public string Background
        {
            get { return _background; }
            set { _background = value; }
        }


        public SimpleLock()
        {
            //!!AddElement(new LockScreenBackground());

            var lineHeight = FleuxApplication.DummyDrawingGraphics.Style(_style).CalculateMultilineTextHeight("0", 100);

            const int leftOffset = 20;
            const int rightOffset = 10;

            _lblClock = new TextElement(GetText())
                            {
                                Style = _style,
                                AutoSizeMode = TextElement.AutoSizeModeOptions.None,
                                Size = new Size(ScreenConsts.ScreenWidth.ToLogic() - leftOffset - rightOffset, lineHeight * 4),
                                Location = new Point(leftOffset, ScreenConsts.ScreenHeight.ToLogic() - lineHeight * 4),
                            };
            AddElement(_lblClock);

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);

            this.TapHandler = OnTap;
        }

        private void UpdateTime()
        {
            _lblClock.Text = GetText();
            // do not call update because change text property calls update inside
        }

        private string GetText()
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

        private void OnSettingsChanged(SettingsChangedMessage settingsChangedMessage)
        {
            if (settingsChangedMessage.PropertyName == "ThemeIsDark")
            {
                _lblClock.Style.Foreground = MetroTheme.PhoneForegroundBrush;
                Update();
            }
        }


        public ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var bindingManager = new BindingManager();
            var controls = new List<UIElement>();

            // lock screen bg image
            var ctrLockScreenImage = new ImageSettingsControl
            {
                Caption = "Lock screen background",
                Value = Background,
            };
            controls.Add(ctrLockScreenImage);
            bindingManager.Bind(this, "Background", ctrLockScreenImage, "Value");

            return controls;
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }

}
