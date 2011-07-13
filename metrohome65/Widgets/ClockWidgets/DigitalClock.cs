using System;
using System.Drawing;

namespace MetroHome65.Widgets
{
    [WidgetInfo("Digital clock")]
    public class DigitalClockWidget : TransparentWidget, IWidgetUpdatable
    {
        private System.Windows.Forms.Timer _Timer;
        private Brush _brushCaption;
        private Font _fntTime;
        private Font _fntDate;
        private Boolean _ShowPoints = true;

        public DigitalClockWidget() : base()
        {
            _brushCaption = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            _fntTime = new System.Drawing.Font("Verdana", 36, FontStyle.Regular);
            _fntDate = new System.Drawing.Font("Helvetica", 12, FontStyle.Regular);
        }


        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(4, 2) 
            };
            return sizes;
        }


        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);

            String sTime = DateTime.Now.ToString("hh" + (_ShowPoints ? ":" : " ") + "mm");
            String sDate = DateTime.Now.DayOfWeek.ToString() + ", " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Day.ToString();

            SizeF TimeBox = g.MeasureString(sTime, _fntTime);
            SizeF DateBox = g.MeasureString(sDate, _fntDate);

            g.DrawString(sTime, _fntTime, _brushCaption,
                Rect.Left + (Rect.Width - TimeBox.Width) / 2,
                Rect.Top + (Rect.Height - TimeBox.Height - DateBox.Height) / 2);

            g.DrawString(sDate, _fntDate, _brushCaption,
                Rect.Left + (Rect.Width - DateBox.Width) / 2,
                Rect.Bottom - (Rect.Height - TimeBox.Height - DateBox.Height) / 2 - DateBox.Height);
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
            _ShowPoints = !_ShowPoints;
            OnWidgetUpdate();
        }

    }

}
