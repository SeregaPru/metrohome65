using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using MetroHome65.Routines;
using MetroHome65.Settings.Controls;

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
        private Boolean _Is24Hour = true;

        private int PaddingRight = ScreenRoutines.Scale(20);
        private int DotWidth = ScreenRoutines.Scale(30); 

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


        /// <summary>
        /// Digital clock style - 12/24 hour
        /// </summary>
        [WidgetParameter]
        public Boolean Is24Hour
        {
            get { return _Is24Hour; }
            set
            {
                if (_Is24Hour != value)
                {
                    _Is24Hour = value;
                    NotifyPropertyChanged("Is24Hour");
                }
            }
        }


        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);

            String sTimeHour = (Is24Hour) ? DateTime.Now.ToString("HH") : DateTime.Now.ToString("hh");
            String sTimeMins = DateTime.Now.ToString("mm");
            String sDate = DateTime.Now.ToString("dddd, MMMM d");

            SizeF TimeBoxHour = g.MeasureString(sTimeHour, _fntTime);
            SizeF TimeBoxMins = g.MeasureString(sTimeMins, _fntTime);
            SizeF DateBox = g.MeasureString(sDate, _fntDate);

            g.DrawString(sTimeMins, _fntTime, _brushCaption,
                Rect.Right - TimeBoxMins.Width - PaddingRight,
                Rect.Top + (Rect.Height - TimeBoxMins.Height - DateBox.Height) / 2);
            if (_ShowPoints)
                g.DrawString(":", _fntTime, _brushCaption,
                    Rect.Right - TimeBoxHour.Width - PaddingRight - DotWidth,
                    Rect.Top + (Rect.Height - TimeBoxMins.Height - DateBox.Height) / 2 - ScreenRoutines.Scale(5));
            g.DrawString(sTimeHour, _fntTime, _brushCaption,
                Rect.Right - TimeBoxMins.Width - PaddingRight - DotWidth - TimeBoxHour.Width,
                Rect.Top + (Rect.Height - TimeBoxMins.Height - DateBox.Height) / 2);

            g.DrawString(sDate, _fntDate, _brushCaption,
                Rect.Right - DateBox.Width - PaddingRight,
                Rect.Bottom - (Rect.Height - TimeBoxMins.Height - DateBox.Height) / 2 - DateBox.Height);
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


        public override List<Control> EditControls
        {
            get
            {
                List<Control> Controls = base.EditControls;

                Settings_flag FlagControl = new Settings_flag();
                FlagControl.Caption = "24-Hours";
                FlagControl.Value = Is24Hour;
                Controls.Add(FlagControl);

                BindingManager BindingManager = new BindingManager();
                BindingManager.Bind(this, "Is24Hour", FlagControl, "Value");

                return Controls;
            }
        }

    }

}
