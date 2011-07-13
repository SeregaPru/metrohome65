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

        private int PaddingRight = 20;
        private int DotWidth = 30; 

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

            String sTime1 = DateTime.Now.ToString("hh");
            String sTime2 = DateTime.Now.ToString("mm");
            String sDate = DateTime.Now.ToString("dddd, MMMM d");

            SizeF TimeBox1 = g.MeasureString(sTime1, _fntTime);
            SizeF TimeBox2 = g.MeasureString(sTime2, _fntTime);
            SizeF DateBox = g.MeasureString(sDate, _fntDate);

            g.DrawString(sTime2, _fntTime, _brushCaption,
                Rect.Right - TimeBox1.Width - PaddingRight,
                Rect.Top + (Rect.Height - TimeBox1.Height - DateBox.Height) / 2);
            if (_ShowPoints)
                g.DrawString(":", _fntTime, _brushCaption,
                    Rect.Right - TimeBox1.Width - PaddingRight - DotWidth,
                    Rect.Top + (Rect.Height - TimeBox1.Height - DateBox.Height) / 2 - 5);
            g.DrawString(sTime1, _fntTime, _brushCaption,
                Rect.Right - TimeBox1.Width - PaddingRight - DotWidth - TimeBox2.Width,
                Rect.Top + (Rect.Height - TimeBox1.Height - DateBox.Height) / 2);

            g.DrawString(sDate, _fntDate, _brushCaption,
                Rect.Right - DateBox.Width - PaddingRight,
                Rect.Bottom - (Rect.Height - TimeBox2.Height - DateBox.Height) / 2 - DateBox.Height);
        }

        public void StartUpdate()
        {
            // update widget just now
            OnWidgetUpdate();

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
