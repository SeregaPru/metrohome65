using System;
using System.Drawing;

namespace MetroHome65.Widgets
{
    [WidgetInfo("Phone")]
    public class PhoneWidget : ShortcutWidget, IWidgetUpdatable
    {
        private System.Windows.Forms.Timer _Timer;
        private int _MissedCount = 0;


        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(2, 2) 
            };
            return sizes;
        }


        protected virtual int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.PhoneMissedCalls;
        }

        public override void Paint(Graphics g, Rectangle Rect)
        {
            PaintIcon(g, new Rectangle(Rect.Left - 15, Rect.Top, Rect.Width, Rect.Height));
            PaintCaption(g, Rect);
            PaintCount(g, Rect);
        }

        private void PaintCount(Graphics g, Rectangle Rect)
        {
            String MissedCountStr = _MissedCount.ToString();

            Font captionFont = new System.Drawing.Font("Helvetica", 24, FontStyle.Regular);
            Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            g.DrawString(MissedCountStr, captionFont, captionBrush,
                Rect.Right - g.MeasureString(MissedCountStr, captionFont).Width - 20,
                (Rect.Bottom + Rect.Top - g.MeasureString(MissedCountStr, captionFont).Height) / 2 - 9);
        }

        public void StartUpdate()
        {
            if (_Timer == null)
            {
                _Timer = new System.Windows.Forms.Timer();
                _Timer.Tick += new EventHandler(OnTimer);
            }
            _Timer.Interval = 2000;
            _Timer.Enabled = true;
        }

        public void StopUpdate()
        {
            if (_Timer != null)
                _Timer.Enabled = false;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            int CurrentMissedCount = GetMissedCount();
            if (CurrentMissedCount != _MissedCount)
            {
                _MissedCount = CurrentMissedCount;
                OnWidgetUpdate();
            }
        }
    }



    [WidgetInfo("SMS")]
    public class SMSWidget : PhoneWidget, IWidgetUpdatable
    {
        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
        }
    }

}
