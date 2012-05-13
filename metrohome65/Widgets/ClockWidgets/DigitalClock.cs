using System;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    [TileInfo("Digital clock")]
    public class DigitalClockWidget : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;

        private readonly Brush _brushCaption;
        private readonly Font _fntTime;
        private readonly Font _fntDate;
        private Boolean _showPoints = true;
        private Boolean _is24Hour = true;

        private readonly int _paddingRight = 20;
        private readonly int _dotWidth = 20;
        private readonly int _dotPaddingRight = 8;
        private readonly int _dotPaddingLeft = 6;

        public DigitalClockWidget() 
        {
            _brushCaption = new SolidBrush(MetroTheme.TileTextStyle.Foreground);
            _fntTime = new Font(MetroTheme.PhoneFontFamilySemiBold, 36, FontStyle.Regular);
            _fntDate = new Font(MetroTheme.PhoneFontFamilySemiBold, 10, FontStyle.Regular);
        }


        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(4, 2) 
            };
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


        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);

            var sTimeHour = (Is24Hour) ? DateTime.Now.ToString("HH") : DateTime.Now.ToString("hh");
            var sTimeMins = DateTime.Now.ToString("mm");
            var sDate = DateTime.Now.ToString("dddd, MMMM d");

            var timeBox = g.MeasureString("99", _fntTime);
            var dateBox = g.MeasureString(sDate, _fntDate);

            g.DrawString(sTimeMins, _fntTime, _brushCaption,
                rect.Right - timeBox.Width - _paddingRight,
                rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2);
            if (_showPoints)
                g.DrawString(":", _fntTime, _brushCaption,
                    rect.Right - timeBox.Width - _paddingRight - _dotWidth - _dotPaddingRight,
                    rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2 - ScreenRoutines.Scale(5));
            g.DrawString(sTimeHour, _fntTime, _brushCaption,
                rect.Right - timeBox.Width - _paddingRight - _dotWidth - _dotPaddingRight - _dotPaddingLeft - timeBox.Width,
                rect.Top + (rect.Height - timeBox.Height - dateBox.Height) / 2);

            g.DrawString(sDate, _fntDate, _brushCaption,
                rect.Right - dateBox.Width - _paddingRight,
                rect.Bottom - (rect.Height - timeBox.Height - dateBox.Height) / 2 - dateBox.Height);
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
                                                 ForceUpdate();
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

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            var flagControl = new FlagSettingsControl
                                  {
                                      Caption = "24-Hours", 
                                      Value = Is24Hour,
                                  };
            controls.Add(flagControl);
            bindingManager.Bind(this, "Is24Hour", flagControl, "Value");

            // hide control for icon / caption selection
            foreach (var control in controls)
                if (control.Name.Contains("Icon"))
                {
                    controls.Remove(control);
                    break;
                }
            foreach (var control in controls)
                if (control.Name.Contains("Caption"))
                {
                    controls.Remove(control);
                    break;
                }
           
            return controls;
        }

        
    }

}
