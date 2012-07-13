using System;
using System.Drawing;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;

namespace MetroHome65.SimpleLock
{

    [LockScreenInfo("Simple lock screen")]
    public class SimpleLock : Canvas, IActive, ILockScreen
    {
        private const string DateFormat = "HH:mm\ndddd\nMMMM d";

        private TextElement _lblClock;

        private ThreadTimer _updateTimer;

        private readonly TextStyle _style = new TextStyle(
            MetroTheme.PhoneFontFamilyNormal,
            MetroTheme.PhoneFontSizeExtraLarge,
            MetroTheme.PhoneForegroundBrush);


        public SimpleLock()
        {
            CreateVisual();
        }

        // ILockScreen
        public virtual void ApplySettings(ILockScreenSettings settings) { }

        private void CreateVisual()
        {
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

    }

}
