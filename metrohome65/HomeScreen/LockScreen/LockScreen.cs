using System;
using System.Drawing;
using Fleux.Animations;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen.LockScreen
{
    public sealed class LockScreen : Canvas, IActive
    {
        private readonly TextElement _lblClock;

        private string _dateFormat = "HH:mm\ndddd\nMMMM d";

        private ThreadTimer _updateTimer;


        public LockScreen()
        {
            var textHeight = 400;
            var leftOffset = 20;
            var rightOffset = 10;

            AddElement(new LockScreenBackground());

            _lblClock = new TextElement(GetText())
                            {
                                Size = new Size(ScreenConsts.ScreenWidth - leftOffset - rightOffset, textHeight),
                                Location = new Point(leftOffset, ScreenConsts.ScreenHeight - textHeight),
                                AutoSizeMode = TextElement.AutoSizeModeOptions.None,
                                Style = new TextStyle(
                                    MetroTheme.PhoneFontFamilyNormal,
                                    MetroTheme.PhoneFontSizeExtraLarge,
                                    MetroTheme.PhoneForegroundBrush),
                            };
            AddElement(_lblClock);

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);

            this.TapHandler = OnTap;
        }

        private void UpdateTime()
        {
            _lblClock.Text = GetText();
            _lblClock.Update();
        }

        private string GetText()
        {
            return DateTime.Now.ToString(_dateFormat);
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

    }
}
