using System.Drawing;
using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [TileInfo("Phone")]
    public class PhoneWidget : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;
        private int _missedCount = 0;

        private static readonly int PaddingRightCnt = ScreenRoutines.Scale(55); //todo comment
        private static readonly int PaddingRightIco = ScreenRoutines.Scale(160); //todo comment

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(2, 2),
                new Size(4, 2) 
            };
            return sizes;
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
            var missedCountStr = _missedCount.ToString();

            var captionHeight = (Caption == "") ? 0 : (CaptionSize /*+ CaptionBottomOffset*/);

            Font captionFont = new Font("Helvetica", 24, FontStyle.Regular);
            Brush captionBrush = new SolidBrush(Color.White);
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
                        _updateTimer = new ThreadTimer(2000, () => UpdateStatus());
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
