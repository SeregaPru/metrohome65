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

        private const int PaddingRightCnt = 55; //todo comment
        private const int PaddingRightIco = 160; //todo comment

        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(1, 1),
                new Size(2, 1),
                new Size(2, 2),
                new Size(4, 2) 
            };
        }

        public PhoneWidget()
        {
            Caption = "Phone".Localize();
        }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            PaintCount(g, rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle rect)
        {
            var dx = (this.GridSize.Width == 1) ? 2 : 1;

            base.PaintIcon(g, new Rectangle(
                rect.Right - (PaddingRightIco / dx) - 5, 
                rect.Top, 
                (PaddingRightIco / dx) - (PaddingRightCnt / dx), 
                rect.Height));
        }

        private void PaintCount(Graphics g, Rectangle rect)
        {
            if (_missedCount == 0) return; // don't display zero count

            var missedCountStr = _missedCount.ToString(CultureInfo.InvariantCulture);

            var captionHeight = (Caption == "") ? 0 : (CaptionHeight);

            var countFont = new Font(MetroTheme.PhoneFontFamilySemiBold, 28.ToLogic(), FontStyle.Bold);
            Brush countBrush = new SolidBrush(CaptionFont.Foreground);

            var dx = (this.GridSize.Width == 1) ? 2 : 1;
            g.DrawString(missedCountStr, countFont, countBrush,
                rect.Right - PaddingRightCnt / dx - 5,
                rect.Top + (rect.Height - g.MeasureString("0", countFont).Height - captionHeight) / 2);
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
}
