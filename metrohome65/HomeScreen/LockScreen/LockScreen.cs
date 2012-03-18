using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen.LockScreen
{
    public sealed class LockScreen : Canvas, IActive
    {
        private readonly TextElement _lblClock;

        private string _dateFormat = "HH:mm\ndddd\nMMMM d";

        private ThreadTimer _updateTimer;


        public LockScreen()
        {
            _lblClock = new TextElement(GetText())
                            {
                                Size = new Size(ScreenConsts.ScreenWidth - 20 - 10, ScreenConsts.ScreenHeight / 2),
                                Location = new Point(20, ScreenConsts.ScreenHeight * 2 / 5),
                                AutoSizeMode = TextElement.AutoSizeModeOptions.None,
                                Style = new TextStyle(
                                    MetroTheme.PhoneFontFamilyNormal,
                                    MetroTheme.PhoneFontSizeExtraLarge,
                                    MetroTheme.PhoneForegroundBrush),
                            };
            AddElement(_lblClock);

            MetroTheme.PropertyChanged += OnThemeSettingsChanged;
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

        private void OnThemeSettingsChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PhoneForegroundBrush")
            {
                _lblClock.Style.Foreground = MetroTheme.PhoneForegroundBrush;
                Update();
            }
        }

    }
}
