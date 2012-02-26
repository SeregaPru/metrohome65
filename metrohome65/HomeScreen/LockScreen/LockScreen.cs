using System;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen
{
    public sealed class LockScreen : Canvas, IActive
    {
        private readonly TextElement _lblClock;

        private string _dateFormat = "HH:mm\ndddd\nMMMM d";

        private Timer _updateTimer;


        public LockScreen()
        {
            _lblClock = new TextElement("00:00")
                            {
                                Size = new Size(ScreenConsts.ScreenWidth - 20 - 10, ScreenConsts.ScreenHeight / 2),
                                Location = new Point(20, ScreenConsts.ScreenHeight / 2),
                                AutoSizeMode = TextElement.AutoSizeModeOptions.None,
                                Style = new TextStyle(
                                    MetroTheme.PhoneFontFamilySemiBold,
                                    MetroTheme.PhoneFontSizeExtraLarge,
                                    Color.White),
                            };
            AddElement(_lblClock);
        }

        private void UpdateTime()
        {
            _lblClock.Text = DateTime.Now.ToString(_dateFormat);
            _lblClock.Update();
        }

        public bool Active
        {
            get { return (_updateTimer != null) && _updateTimer.Enabled; }
            set
            {
                if (value)
                {
                    StartUpdate();
                }
                else
                {
                    StopUpdate();
                }
            }
        }

        public void StartUpdate()
        {
            if (_updateTimer == null)
            {
                _updateTimer = new Timer() { Interval = 2000 };
            }
            _updateTimer.Tick += (s, e) => UpdateTime();
            _updateTimer.Enabled = true;
        }

        public void StopUpdate()
        {
            if (_updateTimer != null)
                _updateTimer.Enabled = false;
        }

    }
}
