using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Routines;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen.LockScreen
{
    public sealed class LockScreen : Canvas, IActive
    {
        private readonly TextElement _lblClock;

        private string _dateFormat = "HH:mm\ndddd\nMMMM d";

        private ThreadTimer _updateTimer;


        public LockScreen(MainSettings mainSettings)
        {
            _lblClock = new TextElement("00:00")
                            {
                                Size = new Size(ScreenConsts.ScreenWidth - 20 - 10, ScreenConsts.ScreenHeight / 2),
                                Location = new Point(20, ScreenConsts.ScreenHeight * 2 / 5),
                                AutoSizeMode = TextElement.AutoSizeModeOptions.None,
                                Style = new TextStyle(
                                    MetroTheme.PhoneFontFamilySemiBold,
                                    MetroTheme.PhoneFontSizeExtraLarge,
                                    mainSettings.FontColor),
                            };
            AddElement(_lblClock);

            mainSettings.PropertyChanged += OnMainSettingsChanged;
        }

        private void UpdateTime()
        {
            _lblClock.Text = DateTime.Now.ToString(_dateFormat);
            _lblClock.Update();
        }

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(2000, () => UpdateTime() );
                }
                else
                {
                    if (_updateTimer != null)
                        _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        private void OnMainSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            var mainSettings = sender as MainSettings;
            if (mainSettings == null) return;

            if (e.PropertyName == "FontColor")
            {
                _lblClock.Style.Foreground = mainSettings.FontColor;
                Update();
            }
        }


    }
}
