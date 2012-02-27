using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroHome65.Routines;
using MetroHome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    [TileInfo("Digital clock")]
    public class DigitalClockWidget : ShortcutWidget, IUpdatable
    {
        private ThreadTimer _updateTimer;

        private readonly Brush _brushCaption;
        private readonly Font _fntTime;
        private readonly Font _fntDate;
        private Boolean _showPoints = true;
        private Boolean _is24Hour = true;

        private readonly int _paddingRight = ScreenRoutines.Scale(20);
        private readonly int _dotWidth = ScreenRoutines.Scale(30);

        public DigitalClockWidget() : base()
        {
            _brushCaption = new SolidBrush(Color.White);
            _fntTime = new Font("Verdana", 36, FontStyle.Regular);
            _fntDate = new Font("Helvetica", 10, FontStyle.Regular);
        }


        protected override Size[] GetSizes()
        {
            var sizes = new Size[] { 
                new Size(4, 2) 
            };
            return sizes;
        }


        /// <summary>
        /// Digital clock style - 12/24 hour
        /// </summary>
        [TileParameter]
        public Boolean Is24Hour
        {
            get { return _is24Hour; }
            set
            {
                if (_is24Hour != value)
                {
                    _is24Hour = value;
                    NotifyPropertyChanged("Is24Hour");
                }
            }
        }


        public override void Paint(Graphics g, Rectangle rect)
        {
            base.Paint(g, rect);

            var sTimeHour = (Is24Hour) ? DateTime.Now.ToString("HH") : DateTime.Now.ToString("hh");
            var sTimeMins = DateTime.Now.ToString("mm");
            var sDate = DateTime.Now.ToString("dddd, MMMM d");

            var timeBoxHour = g.MeasureString(sTimeHour, _fntTime);
            var timeBoxMins = g.MeasureString(sTimeMins, _fntTime);
            var dateBox = g.MeasureString(sDate, _fntDate);

            g.DrawString(sTimeMins, _fntTime, _brushCaption,
                rect.Right - timeBoxMins.Width - _paddingRight,
                rect.Top + (rect.Height - timeBoxMins.Height - dateBox.Height) / 2);
            if (_showPoints)
                g.DrawString(":", _fntTime, _brushCaption,
                    rect.Right - timeBoxHour.Width - _paddingRight - _dotWidth,
                    rect.Top + (rect.Height - timeBoxMins.Height - dateBox.Height) / 2 - ScreenRoutines.Scale(5));
            g.DrawString(sTimeHour, _fntTime, _brushCaption,
                rect.Right - timeBoxMins.Width - _paddingRight - _dotWidth - timeBoxHour.Width,
                rect.Top + (rect.Height - timeBoxMins.Height - dateBox.Height) / 2);

            g.DrawString(sDate, _fntDate, _brushCaption,
                rect.Right - dateBox.Width - _paddingRight,
                rect.Bottom - (rect.Height - timeBoxMins.Height - dateBox.Height) / 2 - dateBox.Height);
        }

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(2000, () => {
                                                 _showPoints = !_showPoints;
                                                 OnWidgetUpdate();
                                             });
                }
                else
                {
                    if (_updateTimer != null)
                        _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        // overriding paint icon method - don't paint icon
        protected override void PaintIcon(Graphics g, Rectangle rect)
        { }

        // overriding paint caption method - don't paint caption
        protected override void PaintCaption(Graphics g, Rectangle rect)
        { }

        public override List<Control> EditControls
        {
            get
            {
                var controls = base.EditControls;

                var flagControl = new Settings_flag { Caption = "24-Hours", Value = Is24Hour };
                controls.Add(flagControl);

                var bindingManager = new BindingManager();
                bindingManager.Bind(this, "Is24Hour", flagControl, "Value");

                // hide control for icon / caption selection
                foreach (var control in controls)
                {
                    if (control.Name.Contains("Icon") || control.Name.Contains("Caption"))
                      control.Height = 0;
                }
               
                return controls;
            }
        }

    }

}
