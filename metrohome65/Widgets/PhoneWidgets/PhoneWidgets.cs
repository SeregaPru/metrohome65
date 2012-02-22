using System;
using System.Drawing;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [WidgetInfo("Phone")]
    public class PhoneWidget : ShortcutWidget, IUpdatable
    {
        private System.Windows.Forms.Timer _timer;
        private int _missedCount = 0;

        private static readonly int PaddingRightCnt = ScreenRoutines.Scale(50); //todo comment
        private static readonly int PaddingRightIco = ScreenRoutines.Scale(160); //todo comment

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(2, 2),
                new Size(4, 2) 
            };
            return sizes;
        }


        public override void Paint(Graphics g, Rectangle rect)
        {
            base.Paint(g, rect);
            PaintCount(g, rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle rect)
        {
            base.PaintIcon(g, new Rectangle(
                rect.Right - PaddingRightIco, rect.Top, PaddingRightIco - PaddingRightCnt, rect.Height));
        }

        private void PaintCount(Graphics g, Rectangle Rect)
        {
            var missedCountStr = _missedCount.ToString();

            var captionHeight = (Caption == "") ? 0 : (CaptionSize /*+ CaptionBottomOffset*/);

            Font captionFont = new Font("Helvetica", 24, FontStyle.Regular);
            Brush captionBrush = new SolidBrush(Color.White);
            g.DrawString(missedCountStr, captionFont, captionBrush,
                Rect.Right - PaddingRightCnt,
                Rect.Top + (Rect.Height - g.MeasureString("0", captionFont).Height - captionHeight) / 2);
        }

        public bool Active
        {
            get { return (_timer != null); }
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
            if (_timer == null)
            {
                _timer = new System.Windows.Forms.Timer() { Interval = 2000 };
                _timer.Tick += OnTimer;
            }
            _timer.Enabled = true;
        }

        public void StopUpdate()
        {
            if (_timer != null)
                _timer.Enabled = false;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            var CurrentMissedCount = GetMissedCount();
            if (CurrentMissedCount != _missedCount)
            {
                _missedCount = CurrentMissedCount;
                OnWidgetUpdate();
            }
        }

        protected virtual int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.PhoneMissedCalls;
        }
    }



    [WidgetInfo("SMS")]
    public class SMSWidget : PhoneWidget, IUpdatable
    {
        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
        }
    }



    [WidgetInfo("E-mail")]
    public class EMailWidget : PhoneWidget, IUpdatable
    {
        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingTotalEmailUnread;
        }
    }

}
