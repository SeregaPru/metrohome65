using System.Drawing;
using System.Globalization;
using Fleux.Core.Scaling;
using Fleux.Styles;
using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [TileInfo("Phone")]
    public class PhoneWidget : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;
        private int _missedCount;

        private static readonly int PaddingRightCnt = /*ScreenRoutines.Scale*/(55); //todo comment
        private static readonly int PaddingRightIco = /*ScreenRoutines.Scale*/(160); //todo comment

        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(2, 2),
                new Size(4, 2) 
            };
        }


        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            PaintCount(g, rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle rect)
        {
            base.PaintIcon(g, new Rectangle(
                rect.Right - PaddingRightIco, rect.Top, PaddingRightIco - PaddingRightCnt, rect.Height));
        }

        private void PaintCount(Graphics g, Rectangle rect)
        {
            if (_missedCount == 0) return; // don't display zero count

            var missedCountStr = _missedCount.ToString(CultureInfo.InvariantCulture);

            var captionHeight = (Caption == "") ? 0 : (CaptionSize);

            var captionFont = new Font(MetroTheme.TileTextStyle.FontFamily, 24.ToLogic(), FontStyle.Regular);
            Brush captionBrush = new SolidBrush(MetroTheme.TileTextStyle.Foreground);
            g.DrawString(missedCountStr, captionFont, captionBrush,
                rect.Right - PaddingRightCnt,
                rect.Top + (rect.Height - g.MeasureString("0", captionFont).Height - captionHeight) / 2);
        }

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(2000, UpdateStatus);
                }
                else
                {
                    if (_updateTimer != null)
                        _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        private void UpdateStatus()
        {
            var currentMissedCount = GetMissedCount();
            if (currentMissedCount != _missedCount)
            {
                _missedCount = currentMissedCount;
                ForceUpdate();
            }
        }

        protected virtual int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.PhoneMissedCalls;
        }
    }



    [TileInfo("SMS")]
    public class SMSWidget : PhoneWidget
    {
        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
        }
    }



    [TileInfo("E-mail")]
    public class EMailWidget : PhoneWidget
    {
        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingTotalEmailUnread;
        }
    }

}
