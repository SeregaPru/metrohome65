using System;
using System.Drawing;
using System.Threading;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen
{
    public sealed class LockScreen : Canvas, IActive
    {
        private readonly TextElement _lblClock;

        private string _dateFormat = "HH:mm\ndddd\nMMMM d";

        private Thread _updateTimer;
        private Boolean _active;


        public LockScreen()
        {
            _lblClock = new TextElement("00:00")
                            {
                                Size = new Size(ScreenConsts.ScreenWidth - 20 - 10, ScreenConsts.ScreenHeight / 2),
                                Location = new Point(20, ScreenConsts.ScreenHeight * 2 / 5),
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
            while (_active)
            {
                _lblClock.Text = DateTime.Now.ToString(_dateFormat);
                _lblClock.Update();

                for (var i = 0; i < 2000; i += 100)
                {
                    if (!_active) return;
                    Thread.Sleep(100);
                }
            }
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
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
                _updateTimer = new Thread( () => UpdateTime() );
                _updateTimer.Start();
            }
        }

        public void StopUpdate()
        {
            if (_updateTimer != null)
                _updateTimer.Join();
            _updateTimer = null;
        }

    }
}
